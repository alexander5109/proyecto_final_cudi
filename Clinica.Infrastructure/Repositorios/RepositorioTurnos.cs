using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Dapper;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.ApiDtos.TurnoDtos;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;

namespace Clinica.Infrastructure.Repositorios;


public class RepositorioTurnos(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioTurnos {




	Task<Result<TurnoId>> IRepositorioTurnos.InsertTurnoReturnId(Turno2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertTurnoReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new TurnoId(newId));



	Task<Result<TurnoDto>> IRepositorioTurnos.UpdateTurnoWhereId(
		TurnoId id,
		Turno2025 instance
	)
		=> TryAsync<TurnoDto>(async conn => {
            // Enviar parámetros al SP
            TurnoDto parametros = instance.ToDto();

			// Ejecutar SP que devuelve int RowsAffected
			int rowsAffected = await conn.ExecuteScalarAsync<int>(
				"sp_UpdateTurnoWhereId",
				parametros,
				commandType: CommandType.StoredProcedure
			);

			if (rowsAffected == 0)
				throw new Exception("No se actualizó ningún turno.");
			// → TryAsync lo convertirá en Result.Error automáticamente

			// Si hubo cambios, devolver el DTO actualizado
			// (tenemos todo en la instancia, ya que la DB solo actualiza)
			return parametros;
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


	Task<Result<IEnumerable<TurnoDbModel>>> IRepositorioTurnos.SelectTurnos()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnos",
				commandType: CommandType.StoredProcedure
			);
		});





}

