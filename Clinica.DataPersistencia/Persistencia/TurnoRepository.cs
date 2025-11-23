using System.Data;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace Clinica.Infrastructure.Persistencia;

public class TurnoRepository(IDbConnectionFactory factory) {
	private readonly IDbConnectionFactory _factory = factory;

	public async Task<IEnumerable<TurnoDto>> GetAll() {
		using IDbConnection conn = _factory.CreateConnection();

		return await conn.QueryAsync<TurnoDto>(
			"sp_ReadTurnosAll",
			commandType: CommandType.StoredProcedure
		);
	}

	public async Task<IEnumerable<TurnoDto>> GetTurnosPorMedicos(IEnumerable<int> medicosIds) {
		using IDbConnection conn = _factory.CreateConnection();

		return await conn.QueryAsync<TurnoDto>(
			"sp_ReadTurnosAll",
			commandType: CommandType.StoredProcedure
		);
	}

	public async Task<Result<TurnoId>> InsertTurno(Turno2025 turno) {
		try {
			using IDbConnection conn = _factory.CreateConnection();
			var parameters = new DynamicParameters();
			parameters.Add("@FechaDeCreacion", turno.FechaDeCreacion);
			parameters.Add("@PacienteId", turno.PacienteId.Value);
			parameters.Add("@MedicoId", turno.MedicoId.Value);
			parameters.Add("@EspecialidadCodigo", turno.Especialidad.CodigoInterno.Valor);
			parameters.Add("@FechaHoraAsignadaDesde", turno.FechaHoraAsignadaDesde);
			parameters.Add("@FechaHoraAsignadaHasta", turno.FechaHoraAsignadaHasta);
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
			await conn.ExecuteAsync("sp_CreateTurno", parameters, commandType: CommandType.StoredProcedure);
			int newId = parameters.Get<int>("@NewId");
			return new Result<TurnoId>.Ok(new TurnoId(newId));
		} catch (SqlException ex) {
			return new Result<TurnoId>.Error($"Error SQL insertando turno: {ex.Message}");
		} catch (Exception ex) {
			return new Result<TurnoId>.Error($"Error inesperado: {ex.Message}");
		}
	}

	public async Task<Result<Unit>> UpdateTurno(Turno2025 turno){
		try {
			using IDbConnection conn = _factory.CreateConnection();
			await conn.ExecuteAsync(
				"sp_UpdateTurno",
				new {
					TurnoId = (int) turno.Id.Valor, 
					OutcomeEstado = (byte)turno.OutcomeEstado, 
					OutcomeFecha = (DateTime) turno.OutcomeFecha.Value, 
					OutcomeComentario = (string?) turno.OutcomeComentario.Value
				},
				commandType: CommandType.StoredProcedure
			);
			return new Result<Unit>.Ok(new Unit());
		} catch (SqlException ex) {
			return new Result<Unit>.Error($"Error SQL actualizando turno: {ex.Message}");
		} catch (Exception ex) {
			return new Result<Unit>.Error($"Error inesperado: {ex.Message}");
		}
	}

}
