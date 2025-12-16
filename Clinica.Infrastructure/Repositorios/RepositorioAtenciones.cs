using System.Data;
using Clinica.Dominio.FunctionalToolkit;
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
	public Task<Result<AtencionId>> InsertAtencionReturnId(AtencionDbModel instance)
		=> TryAsync(async conn => {
            AtencionId newId = await conn.ExecuteScalarAsync<AtencionId>(
				"sp_InsertAtencionReturnId",
				new {
					Id = instance.Id.Valor,
					TurnoId = instance.TurnoId.Valor,
					PacienteId = instance.PacienteId.Valor,
					MedicoId = instance.MedicoId.Valor,
					Observaciones = instance.Observaciones
				},
				commandType: CommandType.StoredProcedure
			);

			return newId;
		});

	// -----------------------------
	// Actualizar observaciones
	// -----------------------------
	public Task<Result<Unit>> UpdateObservacionesWhereId(AtencionId id, string observaciones)
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
	public Task<Result<IEnumerable<AtencionDbModel>>> SelectAtencionesWherePacienteId(PacienteId id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<AtencionDbModel>(
				"sp_SelectAtencionesWherePacienteId",
				new { PacienteId = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	// -----------------------------
	// Seleccionar por turno
	// -----------------------------
	public Task<Result<AtencionDbModel?>> SelectAtencionWhereTurnoId(TurnoId id)
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
	public Task<Result<Unit>> DeleteAtencionWhereId(AtencionId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteAtencionWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});
}
