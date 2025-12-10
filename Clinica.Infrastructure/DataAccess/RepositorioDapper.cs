using System.Data;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;
using Clinica.Shared.ApiDtos;
using Dapper;
using Microsoft.Data.SqlClient;
using static Clinica.Dominio.IRepositorios.QueryModels;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.ApiDtos.PacienteDtos;

namespace Clinica.Infrastructure.DataAccess;

public class RepositorioDapper(SQLServerConnectionFactory factory) : IRepositorio {

	private async Task<Result<T>> TryResultAsync<T>(Func<IDbConnection, Task<Result<T>>> action) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			return await action(conn);
		} catch (SqlException ex) {
			return new Result<T>.Error($"Error SQL: {ex.Message}");
		} catch (Exception ex) {
			return new Result<T>.Error($"Error inesperado: {ex.Message}");
		}
	}


	private async Task<Result<T>> TryAsync<T>(Func<IDbConnection, Task<T>> action) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			T result = await action(conn);
			return new Result<T>.Ok(result);
		} catch (SqlException ex) when (ex.Number == 547) {
			return new Result<T>.Error("No se puede eliminar porque tiene referencias asociadas.");
		} catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) {
			return new Result<T>.Error("Duplicado: ya existe un registro con estos valores.");
		} catch (SqlException ex) {
			return new Result<T>.Error($"Error SQL: {ex.Message}");
		} catch (Exception ex) {
			return new Result<T>.Error($"Error inesperado: {ex.Message}");
		}
	}

	private async Task<Result<TDomain>> TryAsyncAndMap<TDto, TDomain>(
		Func<IDbConnection, Task<TDto?>> query,
		Func<TDto?, Result<TDomain>> mapper
	) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			TDto? dto = await query(conn);
			return mapper(dto);
		} catch (SqlException ex) when (ex.Number == 547) {
			return new Result<TDomain>.Error("No se puede eliminar porque tiene referencias asociadas.");
		} catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) {
			return new Result<TDomain>.Error("Duplicado: ya existe un registro con estos valores.");
		} catch (SqlException ex) {
			return new Result<TDomain>.Error($"Error SQL ({ex.Number}): {ex.Message}");
		} catch (Exception ex) {
			return new Result<TDomain>.Error($"Error inesperado: {ex.Message}");
		}
	}


	private async Task<Result<Unit>> TryAsyncVoid(Func<IDbConnection, Task> action) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			await action(conn);
			return new Result<Unit>.Ok(Unit.Valor);
		} catch (SqlException ex) when (ex.Number == 547) {
			return new Result<Unit>.Error("No se puede eliminar porque tiene referencias asociadas.");
		} catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) {
			return new Result<Unit>.Error("Duplicado: ya existe un registro con estos valores.");
		} catch (SqlException ex) {
			return new Result<Unit>.Error($"Error SQL ({ex.Number}): {ex.Message}");
		} catch (Exception ex) {
			return new Result<Unit>.Error($"Error inesperado: {ex.Message}");
		}
	}







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


	Task<Result<IEnumerable<MedicoId>>> IRepositorioDomainServiciosPrivados.SelectMedicosIdWhereEspecialidadCodigo(EspecialidadCodigo code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoId>(
				"sp_SelectMedicosIdWhereEspecialidadCodigo",
				new { EspecialidadCodigo = code },
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<MedicoDbModel?>> IRepositorioMedicos.SelectMedicoWhereId(MedicoId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<MedicoDbModel>(
				"sp_SelectMedicoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});


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



	Task<Result<Unit>> IRepositorioPacientes.DeletePacienteWhereId(PacienteId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeletePacienteWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});












	Task<Result<Turno2025Agg>> IRepositorioDomainServiciosPrivados.UpdateTurnoWhereId(TurnoId id, Turno2025 instance) => ((IRepositorioTurnos)this).UpdateTurnoWhereId(id, instance);

	Task<Result<Turno2025Agg>> IRepositorioTurnos.UpdateTurnoWhereId(TurnoId id, Turno2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateTurnoWhereId",
				instance.ToModel(),
				commandType: CommandType.StoredProcedure
			);
		}).MapAsync(x => Turno2025Agg.Crear(id, instance));

	Task<Result<Unit>> IRepositorioMedicos.UpdateMedicoWhereId(MedicoId id, Medico2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateMedico",
				instance.ToModel(),
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

	Task<Result<Unit>> IRepositorioPacientes.UpdatePacienteWhereId(PacienteId id, Paciente2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdatePacienteWhereId",
				instance.ToModel(id),
				commandType: CommandType.StoredProcedure
			);
		});


	Task<Result<Unit>> IRepositorioUsuarios.UpdateUsuarioWhereId(UsuarioId id, Usuario2025 instance)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateUsuarioWhereId",
				instance.ToModel(id),
				commandType: CommandType.StoredProcedure
			);
		});































	Task<Result<IEnumerable<TurnoQM>>> IRepositorioDomainServiciosPrivados.SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoQM>(
				"sp_SelectTurnosProgramadosBetweenFechasWhereMedicoId",
				new {
					MedicoId = medicoId.Valor,
					FechaDesde = fechaDesde,
					FechaHasta = fechaHasta
				},
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<IEnumerable<HorarioMedicoQM>>> IRepositorioDomainServiciosPrivados.SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<HorarioMedicoQM>(
				"sp_SelectHorariosVigentesBetweenFechasWhereMedicoId",
				new {
					MedicoId = medicoId.Valor,
					FechaDesde = fechaDesde.Date,
					FechaHasta = fechaHasta.Date
				},
				commandType: CommandType.StoredProcedure
			);
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


	Task<Result<Unit>> IRepositorioUsuarios.DeleteUsuarioWhereId(UsuarioId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteUsuarioWhereId",
				new { Id = id.Valor, },
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<IEnumerable<TurnoDbModel>>> IRepositorioPacientes.SelectTurnosWherePacienteId(PacienteId id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnosWherePacienteId",
				new { PacienteId = id.Valor },
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

	Task<Result<IEnumerable<TurnoDbModel>>> IRepositorioMedicos.SelectTurnosWhereMedicoId(MedicoId id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnosWhereMedicoId",
				new { MedicoId = id.Valor, },
				commandType: CommandType.StoredProcedure);
		});

	Task<Result<UsuarioDbModel?>> IRepositorioUsuarios.SelectUsuarioWhereId(UsuarioId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});












	Task<Result<TurnoId>> IRepositorioDomainServiciosPrivados.InsertTurnoReturnId(Turno2025 instance) => ((IRepositorioTurnos)this).InsertTurnoReturnId(instance);

	Task<Result<TurnoId>> IRepositorioTurnos.InsertTurnoReturnId(Turno2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertTurnoReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new TurnoId(newId));






	Task<Result<MedicoId>> IRepositorioMedicos.InsertMedicoReturnId(Medico2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertMedicoReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new MedicoId(newId));


	Task<Result<PacienteId>> IRepositorioPacientes.InsertPacienteReturnId(Paciente2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertPacienteReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new PacienteId(newId));


	Task<Result<UsuarioId>> IRepositorioUsuarios.InsertUsuarioReturnId(Usuario2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertUsuarioReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new UsuarioId(newId));

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




	Task<Result<IEnumerable<UsuarioDbModel>>> IRepositorioUsuarios.SelectUsuarios()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<UsuarioDbModel>(
				"sp_SelectUsuarios",
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







	Task<Result<Usuario2025Agg>> IRepositorioDomainServiciosPrivados.SelectUsuarioWhereIdAsDomain(UsuarioId id)
		=> TryAsync(async conn => {
			UsuarioDbModel? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			) ?? throw new Exception($"Usuario con Id={id.Valor} no encontrado.");
			Result<Usuario2025> map = dto.ToDomain();
			if (map.IsError)
				throw new Exception($"Erro de dominio: Usuario con Id={id.Valor} no cumple las reglas del dominio: \n{map.UnwrapAsError()}");
			return Usuario2025Agg.Crear(id, map.UnwrapAsOk());
		});


	Task<Result<Usuario2025Agg>> IRepositorioDomainServiciosPrivados.SelectUsuarioWhereNombreAsDomain(NombreUsuario nombre)
		=> TryAsync(async conn => {
			UsuarioDbModel? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereNombre",
				new { NombreUsuario = nombre.Valor },
				commandType: CommandType.StoredProcedure
			) ?? throw new Exception($"Usuario con NombreUsuario={nombre.Valor} no encontrado.");
			Result<Usuario2025> map = dto.ToDomain();
			if (map.IsError)
				throw new Exception($"Erro de dominio:Usuario con NombreUsuario={nombre.Valor} no cumple las reglas del dominio: \n{map.UnwrapAsError()}");
			return Usuario2025Agg.Crear(dto.Id, map.UnwrapAsOk());
		});


	Task<Result<Turno2025Agg>> IRepositorioDomainServiciosPrivados
		.SelectTurnoWhereIdAsDomain(TurnoId id)
		=> TryAsyncAndMap<TurnoDbModel, Turno2025Agg>(
			query: conn => conn.QuerySingleOrDefaultAsync<TurnoDbModel>(
				"sp_SelectTurnoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			),

			mapper: dto => {
				if (dto is null)
					return new Result<Turno2025Agg>.Error(
						$"Turno con id {id} no encontrado."
					);

				// ToDomainAgg() → Result<Turno2025>
				return dto.ToDomain().BindWithPrefix(
					$"Error de dominio en turno {id}: ",
					turnoOk => new Result<Turno2025Agg>.Ok(Turno2025Agg.Crear(id, turnoOk))
				);
			}
		);


}