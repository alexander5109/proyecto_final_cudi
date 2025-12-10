using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using Dapper;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.ApiDtos.MedicoDtos;

namespace Clinica.Infrastructure.Repositorios;


public class RepositorioMedicos(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioMedicos {



    Task<Result<MedicoDbModel?>> IRepositorioMedicos.SelectMedicoWhereId(MedicoId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<MedicoDbModel>(
				"sp_SelectMedicoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<MedicoId>> IRepositorioMedicos.InsertMedicoReturnId(Medico2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertMedicoReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new MedicoId(newId));



	Task<Result<Unit>> IRepositorioMedicos.UpdateMedicoWhereId(MedicoId id, Medico2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateMedico",
				instance.ToModel(),
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<Unit>> IRepositorioMedicos.DeleteMedicoWhereId(MedicoId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteMedicoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});
	Task<Result<IEnumerable<MedicoDbModel>>> IRepositorioMedicos.SelectMedicos()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoDbModel>(
				"sp_SelectMedicos",
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<IEnumerable<MedicoDbModel>>> IRepositorioMedicos.SelectMedicosWhereEspecialidadCodigo(EspecialidadCodigo code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoDbModel>(
				"sp_SelectMedicosWhereEspecialidadCodigo",
				new { EspecialidadCodigo = code },
				commandType: CommandType.StoredProcedure
			);
		});

}
