using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.IRepositorios;
using Dapper;
using static Clinica.Shared.ApiDtos.HorarioDtos;
using static Clinica.Shared.ApiDtos.MedicoDtos;
using static Clinica.Shared.DbModels.DbModels;

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


	Task<Result<HorarioDbModel>> IRepositorioHorarios.UpdateHorarioWhereId(HorarioId id, Horario2025 instance)
		=> TryAsync(async conn => {
			// 1) Convertimos a DTO una sola vez
			HorarioDbModel dto = instance.ToModel(id);

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






















	//----------------------------------------------------------------------------------merging------------------------------------------------------//




	//Task<Result<HorarioDbModel?>> IRepositorioHorarios.SelectHorarioWhereId(HorarioId id)
	//	=> TryAsync(async conn => {
	//		return await conn.QuerySingleOrDefaultAsync<HorarioDbModel>(
	//			"sp_SelectHorarioWhereId",
	//			new { Id = id.Valor },
	//			commandType: CommandType.StoredProcedure
	//		);
	//	});

	//Task<Result<IEnumerable<HorarioDbModel>>> IRepositorioHorarios.SelectHorarios()
	//	=> TryAsync(async conn => {
	//		return await conn.QueryAsync<HorarioDbModel>(
	//			"sp_SelectHorarios",
	//			commandType: CommandType.StoredProcedure
	//		);
	//	});


	//Task<Result<IEnumerable<HorarioFranja2025WithMedicoId>>> IRepositorioHorarios.SelectHorarios()
	//	=> TryAsync(async conn => {
	//		return await conn.QueryAsync<HorarioFranja2025WithMedicoId>(
	//			"sp_SelectHorarios",
	//			commandType: CommandType.StoredProcedure
	//		);
	//	});

	//Task<Result<IEnumerable<HorarioFranja2025WithMedicoId>>> IRepositorioHorarios.SelectHorariosWhereMedicoId(MedicoId medicoId)
	//	=> TryAsync(async conn => {
	//		return await conn.QueryAsync<HorarioFranja2025WithMedicoId>(
	//			"sp_SelectHorariosWhereMedicoId",
	//			new { Id = medicoId.Valor },
	//			commandType: CommandType.StoredProcedure
	//		);
	//	});

	Task<Result<Unit>> IRepositorioHorarios.UpsertHorariosWhereMedicoId(
		HorariosMedicos2025Agg agg
	) =>
		TryAsync(async conn => {
			HorariosMedicosUpsertDbModel dto = agg.ToUpsertDto();

			DynamicParameters parameters = new();
			parameters.Add("@MedicoId", dto.MedicoId);
			parameters.Add(
				"@Franjas",
				dto.Franjas.AsTableValuedParameter("dbo.HorarioFranjaUDTT")
			);

			await conn.ExecuteAsync(
				"sp_UpsertHorariosWhereMedicoId",
				parameters,
				commandType: CommandType.StoredProcedure
			);

			return Unit.Valor;
		});



}