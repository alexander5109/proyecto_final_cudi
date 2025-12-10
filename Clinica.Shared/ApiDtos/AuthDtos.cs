using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Shared.ApiDtos;

public static class AuthDtos {


	public record UsuarioDto(
		string NombreUsuario,
		string PasswordHash,
		UsuarioEnumRole EnumRole
	) {
		// Constructor sin parámetros requerido por Dapper / serializadores
		public UsuarioDto() : this("", "", default) { }
	}
	public static Result<Usuario2025> ToDomain(this UsuarioDto usuario)
		=> Usuario2025.CrearResult(usuario.NombreUsuario, usuario.PasswordHash, usuario.EnumRole);

	public static UsuarioDto ToDto(this Usuario2025 entidad) {
		return new UsuarioDto(entidad.NombreUsuario.Valor, entidad.PasswordHash.Valor, entidad.EnumRole);
	}

	public record UsuarioLoginRequestDto(string Username, string UserPassword);
	public record UsuarioLoginResponseDto(string Username, UsuarioEnumRole EnumRole, string Token);

}