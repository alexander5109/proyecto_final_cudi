using System.Collections.Generic;
using System.Data;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.Dtos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Clinica.Infrastructure.Persistencia;


public class BaseDeDatosRepositorio(IDbConnectionFactory factory) {
	private static DataTable BuildIntList(IEnumerable<int> valores) {
		DataTable table = new();
		table.Columns.Add("Valor", typeof(int));

		foreach (var v in valores)
			table.Rows.Add(v);

		return table;
	}


	//-----------------------INSERT AND RETURN ID------------------

	public async Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 turno) {
		try {
			using IDbConnection conn = factory.CreateConnection();
			DynamicParameters parameters = new();
			parameters.Add("@FechaDeCreacion", turno.FechaDeCreacion.Valor);
			parameters.Add("@PacienteId", turno.PacienteId.Valor);
			parameters.Add("@MedicoId", turno.MedicoId.Valor);
			parameters.Add("@EspecialidadCodigo", turno.Especialidad.CodigoInterno.Valor);
			parameters.Add("@FechaHoraAsignadaDesde", turno.FechaHoraAsignadaDesde);
			parameters.Add("@FechaHoraAsignadaHasta", turno.FechaHoraAsignadaHasta);
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

			await conn.ExecuteAsync("sp_InsertTurnoReturnId", parameters, commandType: CommandType.StoredProcedure);

			int newId = parameters.Get<int>("@NewId");
			return new Result<TurnoId>.Ok(new TurnoId(newId));
		} catch (SqlException ex) {
			return new Result<TurnoId>.Error($"Error SQL insertando turno: {ex.Message}");
		} catch (Exception ex) {
			return new Result<TurnoId>.Error($"Error inesperado: {ex.Message}");
		}
	}


	//-----------------------UPDATE WHERE ID------------------
	public async Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 turno) {
		try {
			using IDbConnection conn = factory.CreateConnection();
			await conn.ExecuteAsync(
				"sp_UpdateTurnoWhereId",
				new {
					TurnoId = (int)turno.Id.Valor,
					OutcomeEstado = (byte)turno.OutcomeEstado,
					OutcomeFecha = (DateTime)turno.OutcomeFecha.Value,
					OutcomeComentario = (string?)turno.OutcomeComentario.Value
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

	//-----------------------SELECT * FROM TABLES WHERE CONDITION------------------



	public async Task<IEnumerable<HorarioMedicoDto>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta) {
		using IDbConnection conn = factory.CreateConnection();

		return await conn.QueryAsync<HorarioMedicoDto>(
			"sp_SelectHorariosVigentesBetweenFechasWhereMedicoId",
			new {
				MedicoId = medicoId.Valor,
				FechaDesde = fechaDesde.Date,
				FechaHasta = fechaHasta.Date
			},
			commandType: CommandType.StoredProcedure
		);
	}

	public async Task<IEnumerable<TurnoDto>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta) {
		using IDbConnection conn = factory.CreateConnection();

		return await conn.QueryAsync<TurnoDto>(
			"sp_SelectTurnosProgramadosBetweenFechasWhereMedicoId",
			new {
				MedicoId = medicoId.Valor,
				FechaDesde = fechaDesde,
				FechaHasta = fechaHasta
			},
			commandType: CommandType.StoredProcedure
		);
	}

	public async Task<IEnumerable<MedicoDto>> SelectMedicosWhereEspecialidad(EspecialidadMedica2025 especialidad) {
		using IDbConnection conn = factory.CreateConnection();

		return await conn.QueryAsync<MedicoDto>(
			"sp_SelectMedicosWhereEspecialidad",
			new {
				EspecialidadCodigoInterno = especialidad.CodigoInterno.Valor
			},
			commandType: CommandType.StoredProcedure
		);
	}



	//-----------------------SELECT * FROM TABLES------------------

	public async Task<IEnumerable<MedicoDto>> SelectMedicos() {
		using IDbConnection conn = factory.CreateConnection();
		return await conn.QueryAsync<MedicoDto>(
			"sp_SelectMedicos",
			commandType: CommandType.StoredProcedure
		);
	}
	public async Task<IEnumerable<PacienteDto>> SelectPacientes() {
		using IDbConnection conn = factory.CreateConnection();
		return await conn.QueryAsync<PacienteDto>(
			"sp_SelectPacientes",
			commandType: CommandType.StoredProcedure
		);
	}
	public async Task<IEnumerable<TurnoDto>> SelectTurnos() {
		using IDbConnection conn = factory.CreateConnection();
		return await conn.QueryAsync<TurnoDto>(
			"sp_SelectTurnos",
			commandType: CommandType.StoredProcedure
		);
	}

}
