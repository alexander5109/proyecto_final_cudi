using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.TypeHandlers;
using Dapper;
using Microsoft.Data.SqlClient;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.ApiDtos.TurnoDtos;

namespace Clinica.Infrastructure.Repositorios;


public abstract class RepositorioBase(SQLServerConnectionFactory factory) {
	protected readonly SQLServerConnectionFactory Factory = factory;



	public Task<Result<TurnoId2025>> InsertTurnoReturnId(Turno2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<TurnoId2025>(
			"sp_InsertTurnoReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		));



	public Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWherePacienteId(PacienteId2025 id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnosWherePacienteId",
				new { PacienteId = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});
	public Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId2025 id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnosWhereMedicoId",
				new { MedicoId = id.Valor, },
				commandType: CommandType.StoredProcedure);
		});

    protected async Task<Result<T>> TryResultAsync<T>(Func<IDbConnection, Task<Result<T>>> action) {
		try {
			using IDbConnection conn = Factory.CrearConexion();
			return await action(conn);
		} catch (SqlException ex) {
			return new Result<T>.Error($"Error SQL: {ex.Message}");
		} catch (Exception ex) {
			return new Result<T>.Error($"Error en repositorio: Inesperado {ex.Message}");
		}
	}


	protected async Task<Result<T>> TryAsync<T>(Func<IDbConnection, Task<T>> action) {
		try {
			using IDbConnection conn = Factory.CrearConexion();
			T result = await action(conn);
			return new Result<T>.Ok(result);
		} catch (SqlException ex) when (ex.Number == 547) {
			return new Result<T>.Error("No se puede eliminar porque tiene referencias asociadas.");
		} catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) {
			return new Result<T>.Error("Duplicado: ya existe un registro con estos valores.");
		} catch (SqlException ex) {
			return new Result<T>.Error($"Error SQL: {ex.Message}");
		} catch (Exception ex) {
			return new Result<T>.Error($"Error inesperado en repositorio: {ex.Message}");
		}
	}

	protected async Task<Result<TDomain>> TryAsyncAndMap<TDto, TDomain>(
		Func<IDbConnection, Task<TDto?>> query,
		Func<TDto?, Result<TDomain>> mapper
	) {
		try {
			using IDbConnection conn = Factory.CrearConexion();
			TDto? dto = await query(conn);
			return mapper(dto);
		} catch (SqlException ex) when (ex.Number == 547) {
			return new Result<TDomain>.Error("No se puede eliminar porque tiene referencias asociadas.");
		} catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) {
			return new Result<TDomain>.Error("Duplicado: ya existe un registro con estos valores.");
		} catch (SqlException ex) {
			return new Result<TDomain>.Error($"Error SQL ({ex.Number}): {ex.Message}");
		} catch (Exception ex) {
			return new Result<TDomain>.Error($"Error inesperado: {ex.Message}");
		}
	}


	protected async Task<Result<Unit>> TryAsyncVoid(Func<IDbConnection, Task> action) {
		try {
			using IDbConnection conn = Factory.CrearConexion();
			await action(conn);
			return new Result<Unit>.Ok(Unit.Valor);
		} catch (SqlException ex) when (ex.Number == 547) {
			return new Result<Unit>.Error("Error en repositorio: No se puede eliminar porque tiene referencias asociadas.");
		} catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) {
			return new Result<Unit>.Error("Error en repositorio: Duplicado: ya existe un registro con estos valores.");
		} catch (SqlException ex) {
			return new Result<Unit>.Error($"Error en repositorio: Error SQL ({ex.Number}): {ex.Message}");
		} catch (Exception ex) {
			return new Result<Unit>.Error($"Error en repositorio: Inesperado: {ex.Message}");
		}
	}


}



public class SQLServerConnectionFactory(string connectionString) {
	public IDbConnection CrearConexion() {

		SqlMapper.AddTypeHandler(new TurnoIdHandler());
		SqlMapper.AddTypeHandler(new PacienteIdHandler());
		SqlMapper.AddTypeHandler(new MedicoIdHandler());
		SqlMapper.AddTypeHandler(new UsuarioIdHandler());
		SqlMapper.AddTypeHandler(new HorarioIdHandler());
		SqlMapper.AddTypeHandler(new AtencionIdHandler());

		return new SqlConnection(connectionString);
	}
}
