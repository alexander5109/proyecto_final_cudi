using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using Dapper;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.ApiDtos.HorarioDtos;

namespace Clinica.Infrastructure.Repositorios;


public class RepositorioHorarios(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioHorarios {
	Task<Result<HorarioId>> IRepositorioHorarios.InsertHorarioReturnId(Horario2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertHorarioReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new HorarioId(newId));



	Task<Result<Unit>> IRepositorioHorarios.DeleteHorarioWhereId(HorarioId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteHorarioWhereId",
				new { Id = id.Valor, },
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<IEnumerable<HorarioDbModel>>> IRepositorioHorarios.SelectHorarios()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<HorarioDbModel>(
				"sp_SelectHorarios",
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<HorarioDbModel?>> IRepositorioHorarios.SelectHorarioWhereId(HorarioId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<HorarioDbModel>(
				"sp_SelectHorarioWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<Unit>> IRepositorioHorarios.UpdateHorarioWhereId(HorarioId id, Horario2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateHorarioWhereId",
				instance.ToModel(id),
				commandType: CommandType.StoredProcedure
			);
		});



}