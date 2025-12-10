using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public record UsuarioDbModel(
		UsuarioId Id,
		string UserName,
		string Nombre,
		string Apellido,
		string PasswordHash,
		UsuarioEnumRole EnumRole,
		string Email,
		string Telefono
	) {
		public UsuarioDbModel() : this(default, "", "", "", "", default, "", "") { }
	}

	public static UsuarioDbModel ToModel(this Usuario2025 entidad, UsuarioId id) {
		return new UsuarioDbModel(
			id,
			entidad.UserName.Valor,
			entidad.NombreCompleto.NombreValor,
			entidad.NombreCompleto.ApellidoValor,
			entidad.PasswordHash.Valor,
			entidad.EnumRole,
			entidad.Email.Valor,
			entidad.Telefono.Valor
		);
	}

	public static Result<Usuario2025Agg> ToDomainAgg(this UsuarioDbModel usuario)
		=> Usuario2025Agg.CrearResult(
			UsuarioId.CrearResult(usuario.Id.Valor),
			Usuario2025.CrearResult(
				UserName.CrearResult(usuario.UserName),
				NombreCompleto2025.CrearResult(usuario.Nombre, usuario.Apellido),
				ContraseñaHasheada.CrearResult(usuario.PasswordHash),
				usuario.EnumRole.CrearResult(),
				ContactoEmail2025.CrearResult(usuario.Email),
				ContactoTelefono2025.CrearResult(usuario.Telefono)
			)
		);
}
