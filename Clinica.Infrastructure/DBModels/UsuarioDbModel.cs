using System.ComponentModel.DataAnnotations;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

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
		=> Usuario2025.Crear(usuario.Id, usuario.NombreUsuario, usuario.PasswordHash, usuario.EnumRole);

	public static UsuarioDbModel ToModel(this Usuario2025 entidad) {
		return new UsuarioDbModel(entidad.UserId, entidad.UserName.Valor, entidad.UserPassword.Valor, entidad.EnumRole);
	}
}
