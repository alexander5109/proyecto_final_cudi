using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;
using Dapper;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.Infrastructure.Repositorios;

public class RepositorioAtenciones(SQLServerConnectionFactory factory)
	: RepositorioBase(factory), IRepositorioAtenciones {
	// -----------------------------
	// Insertar atención
	// -----------------------------

	Task<Result<AtencionId2025>> IRepositorioAtenciones.InsertAtencionReturnId(Atencion2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<AtencionId2025>(
			"sp_InsertAtencionReturnId",
			instance.ToModel(),
			commandType: CommandType.StoredProcedure
		));



	// -----------------------------
	// Actualizar observaciones
	// -----------------------------
	public Task<Result<Unit>> UpdateObservacionesWhereId(AtencionId2025 id, string observaciones)
		=> TryAsyncVoid(async conn => {
			int rows = await conn.ExecuteAsync(
				"sp_UpdateAtencionesWhereId",
				new { Id = id.Valor, Observaciones = observaciones },
				commandType: CommandType.StoredProcedure
			);

			if (rows == 0)
				throw new Exception("No se actualizó ninguna atención.");
		});

	// -----------------------------
	// Seleccionar todas las atenciones
	// -----------------------------
	public Task<Result<IEnumerable<AtencionDbModel>>> SelectAtenciones()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<AtencionDbModel>(
				"sp_SelectAtenciones",
				commandType: CommandType.StoredProcedure
			);
		});

	// -----------------------------
	// Seleccionar por paciente
	// -----------------------------
	public Task<Result<IEnumerable<AtencionDbModel>>> SelectAtencionesWherePacienteId(PacienteId2025 id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<AtencionDbModel>(
				"sp_SelectAtencionesWherePacienteId",
				new { PacienteId = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	// -----------------------------
	// Seleccionar por medico
	// -----------------------------
	public Task<Result<IEnumerable<AtencionDbModel>>> SelectAtencionesWhereMedicoId(MedicoId2025 id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<AtencionDbModel>(
				"sp_SelectAtencionesWhereMedicoId",
				new { MedicoId = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	// -----------------------------
	// Seleccionar por turno
	// -----------------------------
	public Task<Result<AtencionDbModel?>> SelectAtencionWhereTurnoId(TurnoId2025 id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<AtencionDbModel>(
				"sp_SelectAtencionesWhereTurnoId",
				new { TurnoId = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	// -----------------------------
	// Eliminar atención
	// -----------------------------
	public Task<Result<Unit>> DeleteAtencionWhereId(AtencionId2025 id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteAtencionWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});
}
