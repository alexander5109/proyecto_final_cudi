using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
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
		UsuarioRoleCodigo EnumRole,
		string Email,
		string Telefono
	) {
		public UsuarioDbModel() : this(default, "", "", "", "", default, "", "") { }
	}

	public static UsuarioDbModel ToModel(this Usuario2025Agg aggrg) {
		return new UsuarioDbModel(
			aggrg.Id,
			aggrg.Usuario.UserName.Valor,
			aggrg.Usuario.NombreCompleto.NombreValor,
			aggrg.Usuario.NombreCompleto.ApellidoValor,
			aggrg.Usuario.PasswordHash.Valor,
			aggrg.Usuario.EnumRole,
			aggrg.Usuario.Email.Valor,
			aggrg.Usuario.Telefono.Valor
		);
	}
	public static UsuarioDbModel ToModel(this Usuario2025 instance, UsuarioId id) {
		return new UsuarioDbModel(
			id,
			instance.UserName.Valor,
			instance.NombreCompleto.NombreValor,
			instance.NombreCompleto.ApellidoValor,
			instance.PasswordHash.Valor,
			instance.EnumRole,
			instance.Email.Valor,
			instance.Telefono.Valor
		);
	}

	public static Result<Usuario2025Agg> ToDomainAgg(this UsuarioDbModel dbModel)
		=> Usuario2025Agg.CrearResult(
			UsuarioId.CrearResult(dbModel.Id.Valor),
			Usuario2025.CrearResult(
				UserName.CrearResult(dbModel.UserName),
				NombreCompleto2025.CrearResult(dbModel.Nombre, dbModel.Apellido),
				ContraseñaHasheada.CrearResult(dbModel.PasswordHash),
				dbModel.EnumRole.CrearResult(),
				Email2025.CrearResult(dbModel.Email),
				Telefono2025.CrearResult(dbModel.Telefono)
			)
		);
}
