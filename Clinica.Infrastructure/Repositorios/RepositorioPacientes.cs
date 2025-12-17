using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Dapper;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;

namespace Clinica.Infrastructure.Repositorios;


public class RepositorioPacientes(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioPacientes {
	Task<Result<IEnumerable<PacienteDbModel>>> IRepositorioPacientes.SelectPacientes()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<PacienteDbModel>(
			"sp_SelectPacientes",
			commandType: CommandType.StoredProcedure
			);
		});
	Task<Result<PacienteDbModel?>> IRepositorioPacientes.SelectPacienteWhereId(PacienteId2025 id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<PacienteDbModel>(
				"sp_SelectPacienteWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});



	//Task<Result<PacienteDbModel?>> IRepositorioPacientes.SelectPacienteWhereTurnoId(TurnoId2025 id)
	//	=> TryAsync(async conn => {
	//		return await conn.QuerySingleOrDefaultAsync<PacienteDbModel>(
	//			"sp_SelectPacienteWhereTurnoId",
	//			new { Id = id.Valor },
	//			commandType: CommandType.StoredProcedure
	//		);
	//	});

	Task<Result<PacienteId2025>> IRepositorioPacientes.InsertPacienteReturnId(Paciente2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<PacienteId2025>(
			"sp_InsertPacienteReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		));



	Task<Result<PacienteDbModel>> IRepositorioPacientes.UpdatePacienteWhereId(
		PacienteId2025 id,
		Paciente2025 instance
	)
		=> TryAsync(async conn => {
			// 1) Convertimos a DTO una sola vez
			PacienteDbModel dto = instance.ToModel(id);

			// 2) Ejecutamos el SP y obtenemos @@ROWCOUNT
			int rowsAffected = await conn.ExecuteScalarAsync<int>(
				"sp_UpdatePacienteWhereId",
				dto,
				commandType: CommandType.StoredProcedure
			);

			// 3) Si no se actualizó nada → error lógico
			if (rowsAffected == 0)
				throw new Exception($"No se actualizó ningún paciente con Id={id.Valor}");

			// 4) Devolvemos el dto actualizado
			return dto;
		});



	Task<Result<Unit>> IRepositorioPacientes.DeletePacienteWhereId(PacienteId2025 id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeletePacienteWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});




}