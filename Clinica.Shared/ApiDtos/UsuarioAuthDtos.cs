using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;

namespace Clinica.Shared.ApiDtos;

public static class UsuarioAuthDtos {



	// ==========================================================
	// LOGIN
	// ==========================================================

	public record UsuarioLoginResponseDto(
		string Username,
		UsuarioRoleEnum EnumRole,
		string Token
		//MedicoId? MedicoRelacionadoId
	);
	public record UsuarioLoginRequestDto(
		string Username,
		string UserPassword
	);


	// ==========================================================
	// SIGNUP
	// ==========================================================
	public record UsuarioSignUpDto(
		string UserName,
		string Nombre,
		string Apellido,
		string PasswordRaw,
		UsuarioRoleEnum EnumRole,
		string Email,
		string Telefono
	);


	// ==========================================================
	// EDITAR USUARIO
	// ==========================================================

	public record UsuarioEditarDto(
		string UserName,
		string Nombre,
		string Apellido,
		string? NuevaPassword, // 👈 opcional
		UsuarioRoleEnum EnumRole,
		string Email,
		string Telefono,
		MedicoId? MedicoRelacionadoId
	);
	public static Result<Usuario2025Edicion> ToDomain(this UsuarioEditarDto dto) {
		Result<ContraseñaHasheada2025?> contraseña =
			string.IsNullOrWhiteSpace(dto.NuevaPassword)
				? new Result<ContraseñaHasheada2025?>.Ok(null)
				: ContraseñaHasheada2025
					.CrearResultFromRaw(dto.NuevaPassword)
					.Map(x => (ContraseñaHasheada2025?)x);
		return Usuario2025Edicion.CrearResult(
			UserName2025.CrearResult(dto.UserName),
			NombreCompleto2025.CrearResult(dto.Nombre, dto.Apellido),
			contraseña,
			dto.EnumRole.CrearResult(),
			Email2025.CrearResult(dto.Email),
			Telefono2025.CrearResult(dto.Telefono),
			dto.MedicoRelacionadoId

		);
	}
	// ==========================================================
	// CREAR USUARIO
	// ==========================================================
	public record UsuarioCrearDto(
		string UserName,
		string Nombre,
		string Apellido,
		string PasswordHash,
		UsuarioRoleEnum EnumRole,
		string Email,
		string Telefono,
		MedicoId? MedicoRelacionadoId
	) {
		public UsuarioCrearDto() : this("", "", "", "", default, "", "", default) { }
	}

	public static UsuarioCrearDto ToDto(this Usuario2025 entidad) {
		return new UsuarioCrearDto(
			entidad.UserName.Valor,
			entidad.NombreCompleto.NombreValor,
			entidad.NombreCompleto.ApellidoValor,
			entidad.PasswordHash.Valor,
			entidad.EnumRole,
			entidad.Email.Valor,
			entidad.Telefono.Valor,
			entidad.MedicoRelacionadoId
		);
	}

	public static Result<Usuario2025> ToDomain(this UsuarioCrearDto dto)
		=> Usuario2025.CrearResult(
			UserName2025.CrearResult(dto.UserName),
			NombreCompleto2025.CrearResult(dto.Nombre, dto.Apellido),
			ContraseñaHasheada2025.CrearResult(dto.PasswordHash),
			dto.EnumRole.CrearResult(),
			Email2025.CrearResult(dto.Email),
			Telefono2025.CrearResult(dto.Telefono),
			dto.MedicoRelacionadoId
		);

}