using System.Data;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using static Clinica.Dominio.IRepositorios.QueryModels;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.Infrastructure.DataAccess;

public class RepositorioDapper(SQLServerConnectionFactory factory) : IRepositorioDomain {
	private async Task<Result<T>> TryAsync<T>(Func<IDbConnection, Task<T>> action) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			T? result = await action(conn);
			return new Result<T>.Ok(result);
		} catch (SqlException ex) {
			return new Result<T>.Error($"Error SQL: {ex.Message}");
		} catch (Exception ex) {
			return new Result<T>.Error($"Error inesperado: {ex.Message}");
		}
	}

	private async Task<Result<Unit>> TryAsyncVoid(Func<IDbConnection, Task> action) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			await action(conn);
			return new Result<Unit>.Ok(Unit.Valor);
		} catch (SqlException ex) {
			return new Result<Unit>.Error($"Error SQL: {ex.Message}");
		} catch (Exception ex) {
			return new Result<Unit>.Error($"Error inesperado: {ex.Message}");
		}
	}

	//--------------DOMAIN QUERY MODELS-----------//
	public Task<Result<IEnumerable<HorarioMedicoQM>>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<HorarioMedicoQM>(
				"sp_SelectHorariosVigentesBetweenFechasWhereMedicoId",
				new { MedicoId = medicoId.Valor, FechaDesde = fechaDesde.Date, FechaHasta = fechaHasta.Date },
				commandType: CommandType.StoredProcedure
			);
		});
	public Task<Result<IEnumerable<MedicoId>>> SelectMedicosWhereEspecialidadCode(EspecialidadCodigo2025 code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoId>("sp_SelectMedicosWhereEspecialidadCode", new { EspecialidadCodigoInterno = code }, commandType: CommandType.StoredProcedure);
		});

	public Task<Result<IEnumerable<TurnoQM>>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoQM>("sp_SelectTurnosProgramadosBetweenFechasWhereMedicoId",
			new { MedicoId = medicoId.Valor, FechaDesde = fechaDesde, FechaHasta = fechaHasta }, commandType: CommandType.StoredProcedure);
		});










	//public Task<Result<IEnumerable<PacienteListDto>>> SelectPacientesList() => TryAsync(conn => conn.QueryAsync<PacienteListDto>("sp_SelectPacientesListView", commandType: CommandType.StoredProcedure));







	//-----------------------SELECT one ------------------
	public Task<Result<Usuario2025>> SelectUsuarioWhereName(NombreUsuario nombre)
		=> TryAsync(async conn => {
			UsuarioDbModel? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereNombre",
				new { NombreUsuario = nombre.Valor },
				commandType: CommandType.StoredProcedure
			) ?? throw new Exception("Usuario no encontrado");
			Result<Usuario2025> r = dto.ToDomain();
			if (r.IsError)
				throw new Exception($"Error creando Usuario2025 desde DTO: {r.UnwrapAsError()}");

			return r.UnwrapAsOk(); // <- T = Usuario2025
		});


	public Task<Result<Usuario2025>> SelectUsuarioWhereId(UsuarioId id)
		=> TryAsync(async conn => {
			UsuarioDbModel? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);

			if (dto is null)
				throw new Exception("Usuario no encontrado");

			Result<Usuario2025> r = dto.ToDomain();
			if (r.IsError)
				throw new Exception($"Error creando Usuario2025 desde DTO: {r.UnwrapAsError()}");
			return r.UnwrapAsOk(); // devuelve el valor crudo (Usuario2025)
		});

	public Task<Result<Medico2025>> SelectMedicoWhereId(MedicoId id)
		=> TryAsync(async conn => {
			MedicoDbModel? dto = await conn.QuerySingleOrDefaultAsync<MedicoDbModel>(
				"sp_SelectMedicoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);

			if (dto is null)
				throw new Exception("Nivel3Medico no encontrado");

			Result<Medico2025> r = dto.ToDomain();
			if (r.IsError)
				throw new Exception($"Error creando MedicoDbModel desde DTO: {r.UnwrapAsError()}");
			return r.UnwrapAsOk(); // devuelve el valor crudo (MedicoDbModel)
		});

	public Task<Result<Paciente2025>> SelectPacienteWhereId(PacienteId id)
		=> TryAsync(async conn => {
			PacienteDbModel? dto = await conn.QuerySingleOrDefaultAsync<PacienteDbModel>(
				"sp_SelectPacienteWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);

			if (dto is null)
				throw new Exception("Nivel4Paciente no encontrado");

			Result<Paciente2025> r = dto.ToDomain();
			if (r.IsError)
				throw new Exception($"Error creando PacienteDbModel desde DTO: {r.UnwrapAsError()}");
			return r.UnwrapAsOk(); // devuelve el valor crudo (PacienteDbModel)
		});

	public Task<Result<Turno2025>> SelectTurnoWhereId(TurnoId id)
		=> TryAsync(async conn => {
			TurnoDbModel? dto = await conn.QuerySingleOrDefaultAsync<TurnoDbModel>(
				"sp_SelectTurnoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);

			if (dto is null)
				throw new Exception("Turno no encontrado");

			Result<Turno2025> r = dto.ToDomain();
			if (r.IsError)
				throw new Exception($"Error creando TurnoDbModel desde DTO: {r.UnwrapAsError()}");
			return r.UnwrapAsOk(); // devuelve el valor crudo (TurnoDbModel)
		});



	//-----------------------SELECT * ------------------
	public Task<Result<IEnumerable<Paciente2025>>> SelectPacientes()
		=> TryAsync(async conn => {
			IEnumerable<PacienteDbModel> dtos = await conn.QueryAsync<PacienteDbModel>("sp_SelectPacientes", commandType: CommandType.StoredProcedure);
			List<Paciente2025> instances = [];
			foreach (var dto in dtos) {
				Result<Paciente2025> r = dto.ToDomain();
				if (r.IsError)
					throw new Exception($"Error creando Paciente2025 desde DTO: {r.UnwrapAsError()}");
				instances.Add(r.UnwrapAsOk());
			}
			return (IEnumerable<Paciente2025>)instances;
		});
	public Task<Result<IEnumerable<Medico2025>>> SelectMedicos()
		=> TryAsync(async conn => {
			IEnumerable<MedicoDbModel> dtos = await conn.QueryAsync<MedicoDbModel>("sp_SelectMedicos", commandType: CommandType.StoredProcedure);
			List<Medico2025> instances = [];
			foreach (var dto in dtos) {
				Result<Medico2025> r = dto.ToDomain();
				if (r.IsError)
					throw new Exception($"Error creando Medico2025 desde DTO: {r.UnwrapAsError()}");
				instances.Add(r.UnwrapAsOk());
			}
			return (IEnumerable<Medico2025>)instances;
		});






	public Task<Result<IEnumerable<Turno2025>>> SelectTurnos()
		=> TryAsync(async conn => {
			IEnumerable<TurnoDbModel> dtos = await conn.QueryAsync<TurnoDbModel>("sp_SelectPacientes", commandType: CommandType.StoredProcedure);
			List<Turno2025> instances = [];
			foreach (var dto in dtos) {
				Result<Turno2025> r = dto.ToDomain();
				if (r.IsError)
					throw new Exception($"Error creando Turno2025 desde DTO: {r.UnwrapAsError()}");
				instances.Add(r.UnwrapAsOk());
			}
			return (IEnumerable<Turno2025>)instances;
		});

	//public Task<Result<IEnumerable<Turno2025>>> SelectTurnosWherePacienteId(PacienteId id)
	//	=> TryAsync(async conn => {
	//		IEnumerable<TurnoDbModel> dtos = await conn.QueryAsync<TurnoDbModel>("sp_SelectTurnosWherePacienteId",
	//		new { PacienteId = id.Valor }, commandType: CommandType.StoredProcedure);
	//		List<Turno2025> instances = [];
	//		foreach (var dto in dtos) {
	//			Result<Turno2025> r = dto.ToDomain();
	//			if (r.IsError)
	//				throw new Exception($"Error creando Turno2025 desde DTO: {r.UnwrapAsError()}");
	//			instances.Add(r.UnwrapAsOk());
	//		}
	//		return (IEnumerable<Turno2025>)instances;
	//	});

	public Task<Result<IEnumerable<Result<Turno2025>>>> SelectTurnosWherePacienteId(PacienteId id)
		=> factory.TryAsync(async conn => {
			var rows = await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnosWherePacienteId",
				new { PacienteId = id.Valor },
				commandType: CommandType.StoredProcedure
			);
			return rows.ToDomainList(r => r.ToDomain());
		});


	public Task<Result<IEnumerable<Turno2025>>> SelectTurnosWhereMedicoId(MedicoId id)
		=> TryAsync(async conn => {
			IEnumerable<TurnoDbModel> dtos = await conn.QueryAsync<TurnoDbModel>("sp_SelectTurnosWhereMedicoIdId",
			new { PacienteId = id }, commandType: CommandType.StoredProcedure);
			List<Turno2025> instances = [];
			foreach (var dto in dtos) {
				Result<Turno2025> r = dto.ToDomain();
				if (r.IsError)
					throw new Exception($"Error creando Turno2025 desde DTO: {r.UnwrapAsError()}");
				instances.Add(r.UnwrapAsOk());
			}
			return (IEnumerable<Turno2025>)instances;
		});











	public Task<Result<MedicoId>> InsertMedicoReturnId(Medico2025 medico)
		=> TryAsync(async conn => {
			var parameters = new {
				Nombre = medico.NombreCompleto.NombreValor,
				Apellido = medico.NombreCompleto.ApellidoValor,
				Dni = medico.Dni.Valor,
				ProvinciaCodigo = medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
				Domicilio = medico.Domicilio.DireccionValor,
				Localidad = medico.Domicilio.Localidad.NombreValor,
				EspecialidadCodigoInterno = medico.EspecialidadUnica.CodigoInternoValor,
				Telefono = medico.Telefono.Valor,
				Email = medico.Email.Valor,
				Guardia = medico.HaceGuardiasValor,
				FechaIngreso = medico.FechaIngreso.Valor
			};
			return new MedicoId(await conn.ExecuteScalarAsync<int>(
				"sp_InsertMedicoReturnId",
				parameters,
				commandType: CommandType.StoredProcedure
			));
		});


	public Task<Result<PacienteId>> InsertPacienteReturnId(Paciente2025 paciente)
		=> TryAsync(async conn => {
			var parameters = new {
				Dni = paciente.Dni.Valor,
				Nombre = paciente.NombreCompleto.NombreValor,
				Apellido = paciente.NombreCompleto.ApellidoValor,
				FechaIngreso = paciente.FechaIngreso.Valor,
				Email = paciente.Contacto.Email.Valor,
				Telefono = paciente.Contacto.Telefono.Valor,
				FechaNacimiento = paciente.FechaNacimiento.Valor,
				Domicilio = paciente.Domicilio.DireccionValor,
				Localidad = paciente.Domicilio.Localidad.NombreValor,
				ProvinciaCodigo = paciente.Domicilio.Localidad.Provincia.CodigoInternoValor,
			};
			return new PacienteId(await conn.ExecuteScalarAsync<int>(
				"sp_InsertPacienteReturnId",
				parameters,
				commandType: CommandType.StoredProcedure
			));
		});

	public async Task<Result<UsuarioId>> InsertUsuarioReturnId(Usuario2025 usuario)
		=> await TryAsync(async conn => {
			DynamicParameters parameters = new();
			parameters.Add("@NombreUsuario", usuario.UserName.Valor);
			parameters.Add("@PasswordHash", usuario.UserPassword.Valor);
			parameters.Add("@EnumRole", usuario.EnumRole);
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
			await conn.ExecuteAsync(
				"sp_InsertUsuarioReturnId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
			int newId = parameters.Get<int>("@NewId");
			return new UsuarioId(newId);   // ← solo devolvés el valor
		});

	public async Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 turno)
		=> await TryAsync(async conn => {
			DynamicParameters parameters = new();
			parameters.Add("@FechaDeCreacion", turno.FechaDeCreacion.Valor);
			parameters.Add("@PacienteId", turno.PacienteId.Valor);
			parameters.Add("@MedicoId", turno.MedicoId.Valor);
			parameters.Add("@EspecialidadCodigo", turno.Especialidad.CodigoInternoValor);
			parameters.Add("@FechaHoraAsignadaDesde", turno.FechaHoraAsignadaDesdeValor);
			parameters.Add("@FechaHoraAsignadaHasta", turno.FechaHoraAsignadaHastaValor);
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
			await conn.ExecuteAsync(
				"sp_InsertTurnoReturnId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
			int newId = parameters.Get<int>("@NewId");
			return new TurnoId(newId);
		});





	public Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id)
		=> TryAsyncVoid(async conn => {
			var parameters = new {
				Id = id.Valor,
			};
			await conn.ExecuteAsync(
				"sp_DeleteTurnoWhereId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
		});

	public Task<Result<Unit>> DeletePacienteWhereId(PacienteId id)
		=> TryAsyncVoid(async conn => {
			var parameters = new {
				Id = id.Valor,
			};
			await conn.ExecuteAsync(
				"sp_DeletePacienteWhereId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
		});

	public Task<Result<Unit>> DeleteMedicoWhereId(MedicoId id)
		=> TryAsyncVoid(async conn => {
			var parameters = new {
				Id = id.Valor,
			};
			await conn.ExecuteAsync(
				"sp_DeleteMedicoWhereId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
		});

	public Task<Result<Unit>> DeleteUsuarioWhereId(UsuarioId id)
		=> TryAsyncVoid(async conn => {
			var parameters = new {
				Id = id.Valor,
			};
			await conn.ExecuteAsync(
				"sp_DeleteUsuarioWhereId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
		});



	public Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 turno)
		=> TryAsyncVoid(async conn => {
			var parameters = new {
				Id = turno.Id.Valor,
				OutcomeEstado = turno.OutcomeEstado.Codigo,
				OutcomeFecha = turno.OutcomeFechaOption.Valor,
				OutcomeComentario = turno.OutcomeComentarioOption.Valor
			};
			await conn.ExecuteAsync(
				"sp_UpdateTurnoWhereId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
		});
	public Task<Result<Unit>> UpdateMedicoWhereId(Medico2025 medico)
		=> TryAsyncVoid(async conn => {
			var parameters = new {
				Id = medico.Id.Valor,
				Nombre = medico.NombreCompleto.NombreValor,
				Apellido = medico.NombreCompleto.ApellidoValor,
				Dni = medico.Dni.Valor,
				ProvinciaCodigo = medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
				Domicilio = medico.Domicilio.DireccionValor,
				Localidad = medico.Domicilio.Localidad.NombreValor,
				EspecialidadCodigoInterno = medico.EspecialidadUnica.CodigoInternoValor,
				Telefono = medico.Telefono.Valor,
				Email = medico.Email.Valor,
				Guardia = medico.HaceGuardiasValor,
				FechaIngreso = medico.FechaIngreso.Valor
			};

			await conn.ExecuteAsync(
				"sp_UpdateMedico",
				parameters,
				commandType: CommandType.StoredProcedure
			);
		});

	public Task<Result<Unit>> UpdatePacienteWhereId(Paciente2025 paciente)
		=> TryAsyncVoid(async conn => {
			var parameters = new {
				Id = paciente.Id.Valor,
				Dni = paciente.Dni.Valor,
				Nombre = paciente.NombreCompleto.NombreValor,
				Apellido = paciente.NombreCompleto.ApellidoValor,
				FechaIngreso = paciente.FechaIngreso.Valor,
				Email = paciente.Contacto.Email.Valor,
				Telefono = paciente.Contacto.Telefono.Valor,
				FechaNacimiento = paciente.FechaNacimiento.Valor,
				Domicilio = paciente.Domicilio.DireccionValor,
				Localidad = paciente.Domicilio.Localidad.NombreValor,
				ProvinciaCodigo = paciente.Domicilio.Localidad.Provincia.CodigoInternoValor
			};

			await conn.ExecuteAsync(
				"sp_UpdatePaciente",
				parameters,
				commandType: CommandType.StoredProcedure
			);
		});


	//public static async Task<Result<Unit>> UpdatePacienteWhereId(Usuario2025 usuario, IRepositorioDomain repositorio, Paciente2025 paciente) {
	//	if (usuario.EnumRole is not UsuarioEnumRole.Nivel1Admin) {
	//		return new Result<Unit>.Error("No cuenta con permisos para actualizar pacientes.");
	//	}
	//	return await repositorio.UpdatePacienteWhereId(paciente);
	//}
	//public static async Task<Result<PacienteId>> InsertPaciente(Usuario2025 usuario, IRepositorioDomain repositorio, Paciente2025 paciente) {
	//	if (usuario.EnumRole is not UsuarioEnumRole.Nivel1Admin and not UsuarioEnumRole.Nivel2Secretaria) {
	//		return new Result<PacienteId>.Error("No cuenta con permisos para crear pacientes.");
	//	}
	//	return await repositorio.InsertPacienteReturnId(paciente);
	//}
	//public static async Task<Result<Unit>> DeletePacienteWhereId(Usuario2025 usuario, IRepositorioDomain repositorio, PacienteId id) {
	//	if (usuario.EnumRole is not UsuarioEnumRole.Nivel1Admin) {
	//		return new Result<Unit>.Error("No cuenta con permisos para eliminar pacientes.");
	//	}
	//	return await repositorio.DeletePacienteWhereId(id);
	//}
}