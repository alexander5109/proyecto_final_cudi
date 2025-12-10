using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;

namespace Clinica.Shared.ApiDtos;

public static class UsuarioAuthDtos {

	public record UsuarioLoginResponseDto(string Username, UsuarioEnumRole EnumRole, string Token);
	public record UsuarioSignUpDto(
		string UserName,
		string Nombre,
		string Apellido,
		string PasswordRaw,
		UsuarioEnumRole EnumRole,
		string Email,
		string Telefono
	);


	public record UsuarioLoginRequestDto(string Username, string UserPassword);

	public record UsuarioPerfilDto(
		string UserName,
		string Nombre,
		string Apellido,
		string PasswordHash,
		UsuarioEnumRole EnumRole,
		string Email,
		string Telefono
	) {
		public UsuarioPerfilDto() : this("", "", "", "", default, "", "") { }
	}
	public static Result<Usuario2025> ToDomain(this UsuarioPerfilDto usuario)
		=> Usuario2025.CrearResult(
			UserName.CrearResult(usuario.UserName),
			NombreCompleto2025.CrearResult(usuario.Nombre, usuario.Apellido),
			ContraseñaHasheada.CrearResult(usuario.PasswordHash),
			usuario.EnumRole.CrearResult(),
			ContactoEmail2025.CrearResult(usuario.Email),
			ContactoTelefono2025.CrearResult(usuario.Telefono)
		);

	public static UsuarioPerfilDto ToDto(this Usuario2025 entidad) {
		return new UsuarioPerfilDto(
			entidad.UserName.Valor,
			entidad.NombreCompleto.NombreValor,
			entidad.NombreCompleto.ApellidoValor,
			entidad.PasswordHash.Valor,
			entidad.EnumRole,
			entidad.Email.Valor,
			entidad.Telefono.Valor
		);
	}


}