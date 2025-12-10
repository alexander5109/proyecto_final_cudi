using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public record UsuarioDbModel(
		UsuarioId Id,
		string NombreUsuario,
		string PasswordHash,
		UsuarioEnumRole EnumRole
	) {
		public UsuarioDbModel() : this(default!, "", "", default) { }
	}

	public static UsuarioDbModel ToModel(this Usuario2025 usuario, UsuarioId id) {
		return new UsuarioDbModel(
			id,
			usuario.NombreUsuario.Valor,
			usuario.PasswordHash.Valor,
			usuario.EnumRole
		);
	}


	public static Result<Usuario2025Agg> ToDomainAgg(this UsuarioDbModel usuario)
		=> Usuario2025Agg.CrearResult(
			UsuarioId.CrearResult(usuario.Id.Valor),
			Usuario2025.CrearResult(usuario.NombreUsuario, usuario.PasswordHash, usuario.EnumRole)
		);
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
}
