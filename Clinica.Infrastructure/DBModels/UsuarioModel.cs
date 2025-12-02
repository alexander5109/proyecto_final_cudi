using System.ComponentModel.DataAnnotations;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Shared.Dtos;

public static partial class DbModels {
	public record UsuarioModel(
		UsuarioId Id,
		string NombreUsuario,
		string PasswordHash,
		UsuarioEnumRole EnumRole
	) {
		// Constructor sin parámetros requerido por Dapper / serializadores
		public UsuarioModel() : this(default!, "", "", default) { }
	}
	public static Result<UsuarioBase2025> ToDomain(this UsuarioModel usuario)
		=> UsuarioBase2025.Crear(usuario.Id, usuario.NombreUsuario, usuario.PasswordHash, usuario.EnumRole);

	public static UsuarioModel ToModel(this UsuarioBase2025 entidad) {
		UsuarioEnumRole enumrole = entidad switch {
			Usuario2025Nivel1Admin => UsuarioEnumRole.Nivel1Admin,
			Usuario2025Nivel2Secretaria => UsuarioEnumRole.Nivel2Secretaria,
			_ => throw new Exception("Entidad de dominio no reconocida por infraestructura")
		};

		return new UsuarioModel(entidad.UserId, entidad.UserName.Valor, entidad.UserPassword.Valor, enumrole);
	}
}
