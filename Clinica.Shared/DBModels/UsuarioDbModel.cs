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
		=> Usuario2025.CrearResult(usuario.NombreUsuario, usuario.PasswordHash, usuario.EnumRole);
	//public static Result<Usuario2025Agg> ToDomainAgg(this UsuarioDbModel usuario)
	//	=> Usuario2025Agg.CrearResult(usuario.Id, Usuario2025.CrearResult(usuario.NombreUsuario, usuario.PasswordHash, usuario.EnumRole));

	//public static UsuarioDbModel ToModel(this Usuario2025Agg aggrg) {
	//	return new UsuarioDbModel(
	//		aggrg.Id,
	//		aggrg.Usuario.NombreUsuario.Valor,
	//		aggrg.Usuario.PasswordHash.Valor,
	//		aggrg.Usuario.EnumRole
	//	);
	//}

	public static UsuarioDbModel ToModel(this Usuario2025 usuario, UsuarioId id) {
		return new UsuarioDbModel(
			id,
			usuario.NombreUsuario.Valor,
			usuario.PasswordHash.Valor,
			usuario.EnumRole
		);
	}
}
