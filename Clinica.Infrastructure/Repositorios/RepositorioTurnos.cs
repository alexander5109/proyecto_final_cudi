using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using Dapper;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.ApiDtos.TurnoDtos;

namespace Clinica.Infrastructure.Repositorios;


public class RepositorioTurnos(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioTurnos {




	Task<Result<TurnoId>> IRepositorioTurnos.InsertTurnoReturnId(Turno2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertTurnoReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new TurnoId(newId));



	Task<Result<Turno2025Agg>> IRepositorioTurnos.UpdateTurnoWhereId(TurnoId id, Turno2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateTurnoWhereId",
				instance.ToModel(),
				commandType: CommandType.StoredProcedure
			);
		}).MapAsync(x => Turno2025Agg.Crear(id, instance));






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

