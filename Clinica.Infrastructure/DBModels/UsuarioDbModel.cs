using Clinica.Dominio.Comun;

namespace Clinica.Shared.Dtos;

public static partial class DbModels {
	public record UsuarioDbModel(
		UsuarioId Id,
		string NombreUsuario,
		string PasswordHash,
		UsuarioEnumRole EnumRole
	) {
		// Constructor sin parámetros requerido por Dapper / serializadores
		public UsuarioDbModel() : this(default!, "", "", default) { }
	}
	public static Result<Usuario2025> ToDomain(this UsuarioDbModel usuario)
		=> Usuario2025.CrearResult(usuario.Id, usuario.NombreUsuario, usuario.PasswordHash, usuario.EnumRole);

	public static UsuarioDbModel ToModel(this Usuario2025 entidad) {
		return new UsuarioDbModel(entidad.Id, entidad.NombreUsuario.Valor, entidad.PasswordHash.Valor, entidad.EnumRole);
	}
}
