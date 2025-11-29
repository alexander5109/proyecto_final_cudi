using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.DtosEntidades;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using static Clinica.Infrastructure.DtosEntidades.DtosEntidades;

namespace Clinica.Infrastructure.DataAccess;

public class SQLServerConnectionFactory(string connectionString) {
	public IDbConnection CrearConexion() {
		return new SqlConnection(connectionString);
	}
}


public class BaseDeDatosRepositorio(SQLServerConnectionFactory factory, string jwtKey) {

	public async Task<Result<UsuarioBase2025>> ValidarCredenciales(string username, string password) {
		Result<UsuarioBase2025> resultadoUsuario = await SelectUsuarioWhereNombre(new NombreUsuario(username));

		return resultadoUsuario.Match(
			usuarioOk => usuarioOk.PasswordMatch(password),
			notFound => resultadoUsuario
		);
	}

	public string EmitirJwt(UsuarioBase2025 usuario) {
		JwtSecurityTokenHandler handler = new();
		byte[] key = Encoding.ASCII.GetBytes(jwtKey);

		List<Claim> claims = [
			new("userid", usuario.UserId.Valor.ToString()),
			new("username", usuario.UserName.Valor),
			new("role", usuario switch {
				Usuario2025Nivel1Admin => "Admin",
				Usuario2025Nivel2Secretaria => "Secretaria",
				_ => "Desconocido"
			})
		];

		SecurityTokenDescriptor tokenDescriptor = new() {
			Subject = new ClaimsIdentity(claims),

			Expires = DateTime.UtcNow.AddHours(8),

			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature
			)
		};

		SecurityToken token = handler.CreateToken(tokenDescriptor);
		return handler.WriteToken(token);
	}

	//-----------------------INSERT AND RETURN ID------------------
	public async Task<Result<UsuarioId>> InsertUsuarioReturnId(
	NombreUsuario nombre,
	PasswordHasheado password,
	byte enumRole) {
		try {
			using IDbConnection conn = factory.CrearConexion();

			DynamicParameters parameters = new();
			parameters.Add("@NombreUsuario", nombre.Valor);
			parameters.Add("@PasswordHash", password.Valor);
			parameters.Add("@EnumRole", enumRole);
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

			await conn.ExecuteAsync("sp_InsertUsuarioReturnId", parameters, commandType: CommandType.StoredProcedure);

			return new Result<UsuarioId>.Ok(new UsuarioId(parameters.Get<int>("@NewId")));
		} catch (SqlException ex) {
			return new Result<UsuarioId>.Error($"Error SQL insertando usuario: {ex.Message}");
		} catch (Exception ex) {
			return new Result<UsuarioId>.Error($"Error inesperado: {ex.Message}");
		}
	}

	public async Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 turno) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			DynamicParameters parameters = new();
			parameters.Add("@FechaDeCreacion", turno.FechaDeCreacion.Valor);
			parameters.Add("@PacienteId", turno.PacienteId.Valor);
			parameters.Add("@MedicoId", turno.MedicoId.Valor);
			parameters.Add("@EspecialidadCodigo", turno.Especialidad.CodigoInterno.Valor);
			parameters.Add("@FechaHoraAsignadaDesde", turno.FechaHoraAsignadaDesdeValor);
			parameters.Add("@FechaHoraAsignadaHasta", turno.FechaHoraAsignadaHastaValor);
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
			await conn.ExecuteAsync("sp_InsertTurnoReturnId", parameters, commandType: CommandType.StoredProcedure);

			int newId = parameters.Get<int>("@NewId");
			return new Result<TurnoId>.Ok(new TurnoId(newId));
		} catch (SqlException ex) {
			return new Result<TurnoId>.Error($"Error SQL insertando turno: {ex.Message}");
		} catch (Exception ex) {
			return new Result<TurnoId>.Error($"Error inesperado: {ex.Message}");
		}
	}


	//-----------------------UPDATE WHERE ID------------------
	public async Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 turno) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			await conn.ExecuteAsync(
				"sp_UpdateTurnoWhereId",
				new {
					TurnoId = turno.Id.Valor,
					OutcomeEstado = turno.OutcomeEstadoOption.Codigo.Valor,
					OutcomeFecha = turno.OutcomeFechaOption.Valor,
					OutcomeComentario = turno.OutcomeComentarioOption.Valor
				},
				commandType: CommandType.StoredProcedure
			);
			return new Result<Unit>.Ok(new Unit());
		} catch (SqlException ex) {
			return new Result<Unit>.Error($"Error SQL actualizando turno: {ex.Message}");
		} catch (Exception ex) {
			return new Result<Unit>.Error($"Error inesperado: {ex.Message}");
		}
	}

	//-----------------------SELECT * FROM TABLES WHERE CONDITION------------------

	public async Task<Result<UsuarioBase2025>> SelectUsuarioWhereNombre(NombreUsuario nombre) {
		try {
			using IDbConnection conn = factory.CrearConexion();

			UsuarioDto? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDto>(
				"sp_SelectUsuarioWhereNombre",
				new { NombreUsuario = nombre.Valor },
				commandType: CommandType.StoredProcedure
			);

			if (dto is null)
				return new Result<UsuarioBase2025>.Error("Usuario no encontrado");

			return new Result<UsuarioBase2025>.Ok(dto.ToDomain());
		} catch (SqlException ex) {
			return new Result<UsuarioBase2025>.Error($"Error SQL consultando usuario: {ex.Message}");
		} catch (Exception ex) {
			return new Result<UsuarioBase2025>.Error($"Error inesperado: {ex.Message}");
		}
	}

	public async Task<Result<UsuarioBase2025>> SelectUsuarioWhereId(UsuarioId id) {
		try {
			using IDbConnection conn = factory.CrearConexion();

			UsuarioDto? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDto>(
				"sp_SelectUsuarioWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);

			if (dto is null)
				return new Result<UsuarioBase2025>.Error("Usuario no encontrado");

			return new Result<UsuarioBase2025>.Ok(dto.ToDomain());
		} catch (SqlException ex) {
			return new Result<UsuarioBase2025>.Error($"Error SQL consultando usuario por id: {ex.Message}");
		} catch (Exception ex) {
			return new Result<UsuarioBase2025>.Error($"Error inesperado: {ex.Message}");
		}
	}


	public async Task<IEnumerable<HorarioMedicoDto>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta) {
		using IDbConnection conn = factory.CrearConexion();

		return await conn.QueryAsync<HorarioMedicoDto>(
			"sp_SelectHorariosVigentesBetweenFechasWhereMedicoId",
			new {
				MedicoId = medicoId.Valor,
				FechaDesde = fechaDesde.Date,
				FechaHasta = fechaHasta.Date
			},
			commandType: CommandType.StoredProcedure
		);
	}

	public async Task<IEnumerable<TurnoDto>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta) {
		using IDbConnection conn = factory.CrearConexion();

		return await conn.QueryAsync<TurnoDto>(
			"sp_SelectTurnosProgramadosBetweenFechasWhereMedicoId",
			new {
				MedicoId = medicoId.Valor,
				FechaDesde = fechaDesde,
				FechaHasta = fechaHasta
			},
			commandType: CommandType.StoredProcedure
		);
	}

	public async Task<IEnumerable<MedicoDto>> SelectMedicosWhereEspecialidad(EspecialidadMedica2025 especialidad) {
		using IDbConnection conn = factory.CrearConexion();

		return await conn.QueryAsync<MedicoDto>(
			"sp_SelectMedicosWhereEspecialidad",
			new {
				EspecialidadCodigoInterno = especialidad.CodigoInterno.Valor
			},
			commandType: CommandType.StoredProcedure
		);
	}



	//-----------------------SELECT * FROM TABLES------------------

	public async Task<IEnumerable<MedicoDto>> SelectMedicos() {
		using IDbConnection conn = factory.CrearConexion();
		return await conn.QueryAsync<MedicoDto>(
			"sp_SelectMedicos",
			commandType: CommandType.StoredProcedure
		);
	}
	public async Task<IEnumerable<PacienteDto>> SelectPacientes() {
		using IDbConnection conn = factory.CrearConexion();
		return await conn.QueryAsync<PacienteDto>(
			"sp_SelectPacientes",
			commandType: CommandType.StoredProcedure
		);
	}
	public async Task<IEnumerable<TurnoDto>> SelectTurnos() {
		using IDbConnection conn = factory.CrearConexion();
		return await conn.QueryAsync<TurnoDto>(
			"sp_SelectTurnos",
			commandType: CommandType.StoredProcedure
		);
	}

}
