using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;
using Dapper;
using static Clinica.Shared.ApiDtos.MedicoDtos;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.DbModels.DbModels;

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



	Task<Result<MedicoDto>> IRepositorioMedicos.UpdateMedicoWhereId(
		MedicoId id, 
		Medico2025 instance
	)
		=> TryAsync<MedicoDto>(async conn => {
			// 1) Convertimos a DTO una sola vez
			MedicoDto dto = instance.ToDto();

			// 2) Ejecutamos el SP y obtenemos @@ROWCOUNT
			int rowsAffected = await conn.ExecuteScalarAsync<int>(
				"sp_UpdatePacienteWhereId",
				dto,
				commandType: CommandType.StoredProcedure
			);

			// 3) Si no se actualizó nada → error lógico
			if (rowsAffected == 0)
				throw new Exception($"No se actualizó ningún médico con Id={id.Valor}");

			// 4) Devolvemos el dto actualizado
			return dto;
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
