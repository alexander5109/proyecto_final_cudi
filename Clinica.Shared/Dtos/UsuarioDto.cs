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

		public Result<UsuarioBase2025> ToDomain()
			=> UsuarioBase2025.Crear(Id, NombreUsuario, PasswordHash, EnumRole);

		public static UsuarioDto FromDomain(UsuarioBase2025 entidad) {
			UsuarioEnumRole enumrole = entidad switch {
				Usuario2025Nivel1Admin => UsuarioEnumRole.Nivel1Admin,
				Usuario2025Nivel2Secretaria => UsuarioEnumRole.Nivel2Secretaria,
				_ => throw new Exception("Entidad de dominio no reconocida por infraestructura")
			};

			return new UsuarioDto(entidad.UserId, entidad.UserName.Valor, entidad.UserPassword.Valor, enumrole);
		}
	}
}
