using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Shared.Dtos;

public static partial class DomainDtos {
	public class UsuarioDto {
		// Constructor vacío para Dapper
		public UsuarioDto() { }

		// Constructor completo (opcional)
		public UsuarioDto(UsuarioId id, string nombreUsuario, string passwordHash, UsuarioEnumRole enumRole) {
			Id = id;
			NombreUsuario = nombreUsuario;
			PasswordHash = passwordHash;
			EnumRole = enumRole;
		}

		public UsuarioId Id { get; set; }
		public string NombreUsuario { get; set; } = "";
		public string PasswordHash { get; set; } = "";
		public UsuarioEnumRole EnumRole { get; set; }

		public Result<Usuario2025> ToDomain()
			=> Usuario2025.Crear(Id, NombreUsuario, PasswordHash, EnumRole);

		public static UsuarioDto FromDomain(Usuario2025 entidad) {
			return new UsuarioDto(entidad.UserId, entidad.UserName.Valor, entidad.UserPassword.Valor, entidad.EnumRole);
		}
	}
}
