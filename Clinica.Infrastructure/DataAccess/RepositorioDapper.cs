using System.Data;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;
using Dapper;
using Microsoft.Data.SqlClient;
using static Clinica.Dominio.IRepositorios.QueryModels;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.Infrastructure.DataAccess;

public class RepositorioDapper(SQLServerConnectionFactory factory) : IRepositorio {
	private async Task<Result<T>> TryAsync<T>(Func<IDbConnection, Task<T>> action) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			T? result = await action(conn);
			return new Result<T>.Ok(result);
		} catch (SqlException ex) {
			return new Result<T>.Error($"Error SQL: {ex.Message}");
		} catch (Exception ex) {
			return new Result<T>.Error($"Error inesperado: {ex.Message}");
		}
	}

	private async Task<Result<Unit>> TryAsyncVoid(Func<IDbConnection, Task> action) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			await action(conn);
			return new Result<Unit>.Ok(Unit.Valor);
		} catch (SqlException ex) {
			return new Result<Unit>.Error($"Error SQL: {ex.Message}");
		} catch (Exception ex) {
			return new Result<Unit>.Error($"Error inesperado: {ex.Message}");
		}
	}


	Task<Result<Usuario2025>> IRepositorioDomain.SelectUsuarioWhereIdAsDomain(UsuarioId id)
		=> TryAsync(async conn => {
			UsuarioDbModel? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			) ?? throw new Exception($"Usuario con Id={id.Valor} no encontrado.");
			Result<Usuario2025> map = dto.ToDomain();
			if (map.IsError)
				throw new Exception($"Error creando Usuario2025 desde DTO (Id={id.Valor}): {map.UnwrapAsError()}");
			return map.UnwrapAsOk();
		});


	Task<Result<Usuario2025>> IRepositorioDomain.SelectUsuarioWhereNombreAsDomain(NombreUsuario nombre)
		=> TryAsync(async conn => {
			UsuarioDbModel? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereNombre",
				new { NombreUsuario = nombre.Valor },
				commandType: CommandType.StoredProcedure
			) ?? throw new Exception($"Usuario con NombreUsuario={nombre.Valor} no encontrado.");
			Result<Usuario2025> map = dto.ToDomain();
			if (map.IsError)
				throw new Exception($"Error creando Usuario2025 desde UsuarioDbModel (NombreUsuario={nombre.Valor}): {map.UnwrapAsError()}");

			return map.UnwrapAsOk();
		});







	Task<Result<Unit>> IRepositorioMedicos.DeleteMedicoWhereId(MedicoId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteMedicoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<MedicoId>> IRepositorioMedicos.InsertMedicoReturnId(Medico2025 instance)
		=> TryAsync(async conn => {
			return new MedicoId(await conn.ExecuteScalarAsync<int>(
				"sp_InsertMedicoReturnId",
				instance.ToModel(),
				commandType: CommandType.StoredProcedure
			));
		});

	Task<Result<IEnumerable<MedicoDbModel>>> IRepositorioMedicos.SelectMedicos()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoDbModel>(
				"sp_SelectMedicos",
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<IEnumerable<MedicoDbModel>>> IRepositorioMedicos.SelectMedicosWhereEspecialidadCode(EspecialidadCodigo2025 code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoDbModel>(
				"sp_SelectMedicosWhereEspecialidadCode",
				new { EspecialidadCodigoInterno = code },
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<MedicoDbModel?>> IRepositorioMedicos.SelectMedicoWhereId(MedicoId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<MedicoDbModel>(
				"sp_SelectMedicoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<Unit>> IRepositorioMedicos.UpdateMedicoWhereId(Medico2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateMedico",
				instance.ToModel(),
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<IEnumerable<PacienteDbModel>>> IRepositorioPacientes.SelectPacientes()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<PacienteDbModel>(
				"sp_SelectPacientes",
				commandType: CommandType.StoredProcedure
			);
		});
	Task<Result<PacienteDbModel?>> IRepositorioPacientes.SelectPacienteWhereId(PacienteId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<PacienteDbModel>(
				"sp_SelectPacienteWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<PacienteId>> IRepositorioPacientes.InsertPacienteReturnId(Paciente2025 instance)
		=> TryAsync(async conn => {
			return new PacienteId(await conn.ExecuteScalarAsync<int>(
				"sp_InsertPacienteReturnId",
				instance.ToModel(),
				commandType: CommandType.StoredProcedure
			));
		});

	Task<Result<Unit>> IRepositorioPacientes.UpdatePacienteWhereId(Paciente2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdatePaciente",
				instance.ToModel(),
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<Unit>> IRepositorioPacientes.DeletePacienteWhereId(PacienteId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeletePacienteWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});






	Task<Result<Unit>> IRepositorioDomain.UpdateTurnoWhereId(Turno2025 instance) => ((IRepositorioTurnos)this).UpdateTurnoWhereId(instance);

	Task<Result<Unit>> IRepositorioTurnos.UpdateTurnoWhereId(Turno2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateTurnoWhereId",
				instance.ToModel(),
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<Unit>> IRepositorioUsuarios.UpdateUsuarioWhereId(Usuario2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateUsuarioWhereId",
				instance.ToModel(),
				commandType: CommandType.StoredProcedure
			);
		});




	Task<Result<TurnoId>> IRepositorioDomain.InsertTurnoReturnId(Turno2025 instance) => ((IRepositorioTurnos)this).InsertTurnoReturnId(instance);

	Task<Result<TurnoId>> IRepositorioTurnos.InsertTurnoReturnId(Turno2025 instance)
		=> TryAsync(async conn => {
			DynamicParameters parameters = new(instance.ToModel());
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
			await conn.ExecuteAsync(
				"sp_InsertTurnoReturnId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
			int newId = parameters.Get<int>("@NewId");
			return new TurnoId(newId);
		});





	Task<Result<IEnumerable<MedicoId>>> IRepositorioDomain.SelectMedicosIdWhereEspecialidadCode(EspecialidadCodigo2025 code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoId>(
				"sp_SelectMedicosIdWhereEspecialidadCode",
				new { EspecialidadCodigoInterno = code },
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<IEnumerable<TurnoQM>>> IRepositorioDomain.SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoQM>(
				"sp_SelectTurnosProgramadosBetweenFechasWhereMedicoId",
				new {
					MedicoId = medicoId.Valor,
					FechaDesde = fechaDesde,
					FechaHasta = fechaHasta
				},
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<IEnumerable<HorarioMedicoQM>>> IRepositorioDomain.SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<HorarioMedicoQM>(
				"sp_SelectHorariosVigentesBetweenFechasWhereMedicoId",
				new {
					MedicoId = medicoId.Valor,
					FechaDesde = fechaDesde.Date,
					FechaHasta = fechaHasta.Date
				},
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<TurnoDbModel?>> IRepositorioTurnos.SelectTurnoWhereId(TurnoId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<TurnoDbModel>(
				"sp_SelectTurnoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<Unit>> IRepositorioTurnos.DeleteTurnoWhereId(TurnoId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteTurnoWhereId",
				new { Id = id.Valor, },
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<Unit>> IRepositorioUsuarios.DeleteUsuarioWhereId(UsuarioId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteUsuarioWhereId",
				new { Id = id.Valor, },
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<IEnumerable<TurnoDbModel>>> IRepositorioTurnos.SelectTurnosWherePacienteId(PacienteId id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnosWherePacienteId",
				new { PacienteId = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<IEnumerable<TurnoDbModel>>> IRepositorioTurnos.SelectTurnos()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnos",
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<IEnumerable<TurnoDbModel>>> IRepositorioTurnos.SelectTurnosWhereMedicoId(MedicoId id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnosWhereMedicoId",
				new { Id = id.Valor, },
				commandType: CommandType.StoredProcedure);
		});

	Task<Result<UsuarioDbModel?>> IRepositorioUsuarios.SelectUsuarioWhereId(UsuarioId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<UsuarioId>> IRepositorioUsuarios.InsertUsuarioReturnId(Usuario2025 instance)
		=> TryAsync(async conn => {
			DynamicParameters parameters = new(instance.ToModel());
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
			await conn.ExecuteAsync(
				"sp_InsertUsuarioReturnId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
			int newId = parameters.Get<int>("@NewId");
			return new UsuarioId(newId);   // ← solo devolvés el valor
		});


	Task<Result<IEnumerable<UsuarioDbModel>>> IRepositorioUsuarios.SelectUsuarios()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<UsuarioDbModel>(
				"sp_SelectUsuarios",
				commandType: CommandType.StoredProcedure
			);
		});

}