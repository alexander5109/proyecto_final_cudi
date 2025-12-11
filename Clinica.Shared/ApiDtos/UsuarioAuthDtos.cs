using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;

namespace Clinica.Shared.ApiDtos;

public static class UsuarioAuthDtos {



	public sealed record UsuarioAutenticadoDto(
		UsuarioId Id,
		string UserName,
		UsuarioRoleCodigo EnumRole
	);

	public record UsuarioLoginResponseDto(string Username, UsuarioRoleCodigo EnumRole, string Token);
	public record UsuarioSignUpDto(
		string UserName,
		string Nombre,
		string Apellido,
		string PasswordRaw,
		UsuarioRoleCodigo EnumRole,
		string Email,
		string Telefono
	);


	public record UsuarioLoginRequestDto(string Username, string UserPassword);

	public record UsuarioDto(
		string UserName,
		string Nombre,
		string Apellido,
		string PasswordHash,
		UsuarioRoleCodigo EnumRole,
		string Email,
		string Telefono
	) {
		public UsuarioDto() : this("", "", "", "", default, "", "") { }
	}

	public static UsuarioDto ToDto(this Usuario2025 entidad) {
		return new UsuarioDto(
			entidad.UserName.Valor,
			entidad.NombreCompleto.NombreValor,
			entidad.NombreCompleto.ApellidoValor,
			entidad.PasswordHash.Valor,
			entidad.EnumRole,
			entidad.Email.Valor,
			entidad.Telefono.Valor
		);
	}

	public static Result<Usuario2025> ToDomain(this UsuarioDto dto)
		=> Usuario2025.CrearResult(
			UserName.CrearResult(dto.UserName),
			NombreCompleto2025.CrearResult(dto.Nombre, dto.Apellido),
			ContraseñaHasheada.CrearResult(dto.PasswordHash),
			dto.EnumRole.CrearResult(),
			Email2025.CrearResult(dto.Email),
			Telefono2025.CrearResult(dto.Telefono)
		);

}