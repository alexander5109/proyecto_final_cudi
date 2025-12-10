using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using Dapper;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.ApiDtos.PacienteDtos;

namespace Clinica.Infrastructure.Repositorios;


public class RepositorioPacientes(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioPacientes {
	Task<Result<IEnumerable<PacienteDbModel>>> IRepositorioPacientes.SelectPacientes()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<PacienteDbModel>(
			"sp_SelectPacientes",
			commandType: CommandType.StoredProcedure
			);
		});
	Task<Result<PacienteDbModel?>> IRepositorioPacientes.SelectPacienteWhereId(PacienteId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<PacienteDbModel>(
				"sp_SelectPacienteWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});



	//Task<Result<PacienteDbModel?>> IRepositorioPacientes.SelectPacienteWhereTurnoId(TurnoId id)
	//	=> TryAsync(async conn => {
	//		return await conn.QuerySingleOrDefaultAsync<PacienteDbModel>(
	//			"sp_SelectPacienteWhereTurnoId",
	//			new { Id = id.Valor },
	//			commandType: CommandType.StoredProcedure
	//		);
	//	});

	Task<Result<PacienteId>> IRepositorioPacientes.InsertPacienteReturnId(Paciente2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertPacienteReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new PacienteId(newId));


	Task<Result<IEnumerable<TurnoDbModel>>> IRepositorioPacientes.SelectTurnosWherePacienteId(PacienteId id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnosWherePacienteId",
				new { PacienteId = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<Unit>> IRepositorioPacientes.UpdatePacienteWhereId(PacienteId id, Paciente2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdatePacienteWhereId",
				instance.ToModel(id),
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<Unit>> IRepositorioPacientes.DeletePacienteWhereId(PacienteId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeletePacienteWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});




}