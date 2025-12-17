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



	Task<Result<MedicoDbModel?>> IRepositorioMedicos.SelectMedicoWhereId(MedicoId2025 id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<MedicoDbModel>(
				"sp_SelectMedicoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});
	//Task<Result<MedicoDbModel?>> IRepositorioMedicos.SelectMedicoWithHorarioWhereId(MedicoId2025 id)
	//	=> TryAsync(async conn => {
	//		return await conn.QuerySingleOrDefaultAsync<MedicoDbModel>(
	//			"sp_SelectMedicoWithHorariosWhereId",
	//			new { Id = id.Valor },
	//			commandType: CommandType.StoredProcedure
	//		);
	//	});

	Task<Result<MedicoId2025>> IRepositorioMedicos.InsertMedicoReturnId(Medico2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<MedicoId2025>(
			"sp_InsertMedicoReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		));




	//private static DataTable ToHorarioTable(IEnumerable<HorarioDto> horarios) {
	//	var table = new DataTable();

	//	table.Columns.Add("DiaSemana", typeof(byte));
	//	table.Columns.Add("HoraDesde", typeof(TimeSpan));
	//	table.Columns.Add("HoraHasta", typeof(TimeSpan));
	//	table.Columns.Add("VigenteDesde", typeof(DateTime));
	//	table.Columns.Add("VigenteHasta", typeof(DateTime));

	//	foreach (var h in horarios) {
	//		table.Rows.Add(
	//			(byte)h.DiaSemana,          // DayOfWeek → TINYINT
	//			h.HoraDesde,
	//			h.HoraHasta,
	//			h.VigenteDesde.Date,
	//			h.VigenteHasta == default ? DBNull.Value : h.VigenteHasta.Date
	//		);
	//	}

	//	return table;
	//}



	Task<Result<MedicoDbModel>> IRepositorioMedicos.UpdateMedicoWhereId(
		MedicoId2025 id,
		Medico2025 instance
	)
		=> TryAsync(async conn => {
			// 1) Convertimos a DTO una sola vez
			MedicoDbModel dto = instance.ToModel(id);

			// 2) Ejecutamos el SP y obtenemos @@ROWCOUNT
			int rowsAffected = await conn.ExecuteScalarAsync<int>(
				"sp_UpdateMedicoWhereId",
				dto,
				commandType: CommandType.StoredProcedure
			);

			// 3) Si no se actualizó nada → error lógico
			if (rowsAffected == 0)
				throw new Exception($"No se actualizó ningún médico con Id={id.Valor}");

			// 4) Devolvemos el dto actualizado
			return dto;
		});


	Task<Result<Unit>> IRepositorioMedicos.DeleteMedicoWhereId(MedicoId2025 id)
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
	//Task<Result<IEnumerable<MedicoDbModel>>> IRepositorioMedicos.SelectMedicosWithHorarios()
	//	=> TryAsync(async conn => {
	//		return await conn.QueryAsync<MedicoDbModel>(
	//			"sp_SelectMedicosWithHorario",
	//			commandType: CommandType.StoredProcedure
	//		);
	//	});

	Task<Result<IEnumerable<MedicoDbModel>>> IRepositorioMedicos.SelectMedicosWhereEspecialidadCodigo(EspecialidadEnum code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoDbModel>(
				"sp_SelectMedicosWhereEspecialidadCodigo",
				new { EspecialidadCodigo = code },
				commandType: CommandType.StoredProcedure
			);
		});

}
