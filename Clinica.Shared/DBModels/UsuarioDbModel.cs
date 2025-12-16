using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public sealed record UsuarioPersistido(
		UsuarioId Id,
		UserName2025 UserName,
		ContraseñaHasheada2025 Password,
		NombreCompleto2025 Nombre,
		UsuarioRoleEnum Role,
		Email2025 Email,
		Telefono2025 Telefono
	) {
		public UsuarioPersistido Aplicar(Usuario2025Edicion edicion) {
			return this with {
				UserName = edicion.UserName,
				Nombre = edicion.NombreCompleto,
				Role = edicion.EnumRole,
				Email = edicion.Email,
				Telefono = edicion.Telefono,
				Password = edicion.NuevaContraseña ?? this.Password
			};
		}


	}



	public sealed record UsuarioAutenticadoDbModel(
		UsuarioId Id,
		string UserName,
		UsuarioRoleEnum EnumRole
	);


	public record UsuarioDbModel(
		UsuarioId Id,
		string UserName,
		string? PasswordHash,
		string Nombre,
		string Apellido,
		string Telefono,
		string Email,
		UsuarioRoleEnum EnumRole,
		MedicoId? MedicoRelacionadoId
	) {
		public UsuarioDbModel() : this(default, "", "", "", "", "", "", default, default) { }
	}

	public static UsuarioDbModel ToModel(this Usuario2025Edicion edicion, UsuarioId id)
		=> new(
			Id: id,
			UserName: edicion.UserName.Valor,
			PasswordHash: edicion.NuevaContraseña?.Valor,
			Nombre: edicion.NombreCompleto.NombreValor,
			Apellido: edicion.NombreCompleto.ApellidoValor,
			Telefono: edicion.Telefono.Valor,
			Email: edicion.Email.Valor,
			EnumRole: edicion.EnumRole,
			MedicoRelacionadoId: edicion.MedicoRelacionadoId
		);


	public static UsuarioDbModel ToModel(this Usuario2025Agg aggrg) {
		return new UsuarioDbModel(
			aggrg.Id,
			aggrg.Usuario.UserName.Valor,
			aggrg.Usuario.PasswordHash.Valor,
			aggrg.Usuario.NombreCompleto.NombreValor,
			aggrg.Usuario.NombreCompleto.ApellidoValor,
			aggrg.Usuario.Telefono.Valor,
			aggrg.Usuario.Email.Valor,
			aggrg.Usuario.EnumRole,
			aggrg.Usuario.MedicoRelacionadoId
		);
	}
	public static UsuarioDbModel ToModel(this Usuario2025EdicionAgg aggrg) {
		return new UsuarioDbModel(
			aggrg.Id,
			aggrg.Usuario.UserName.Valor,
			aggrg.Usuario.NuevaContraseña?.Valor,
			aggrg.Usuario.NombreCompleto.NombreValor,
			aggrg.Usuario.NombreCompleto.ApellidoValor,
			aggrg.Usuario.Telefono.Valor,
			aggrg.Usuario.Email.Valor,
			aggrg.Usuario.EnumRole,
			aggrg.Usuario.MedicoRelacionadoId
		);
	}
	public static UsuarioDbModel ToModel(this Usuario2025 instance, UsuarioId id) {
		return new UsuarioDbModel(
			id,
			instance.UserName.Valor,
			instance.PasswordHash.Valor,
			instance.NombreCompleto.NombreValor,
			instance.NombreCompleto.ApellidoValor,
			instance.Telefono.Valor,
			instance.Email.Valor,
			instance.EnumRole,
			instance.MedicoRelacionadoId
		);
	}

	//public static Result<Usuario2025Agg> ToDomainAgg(this UsuarioDbModel dbModel)
	//	=> Usuario2025Agg.CrearResult(
	//		UsuarioId.CrearResult(dbModel.Id.Valor),
	//		Usuario2025.CrearResult(
	//			UserName2025.CrearResult(dbModel.UserName),
	//			NombreCompleto2025.CrearResult(dbModel.Nombre, dbModel.Apellido),
	//			ContraseñaHasheada2025.CrearResult(dbModel.PasswordHash),
	//			dbModel.EnumRole.CrearResult(),
	//			Email2025.CrearResult(dbModel.Email),
	//			Telefono2025.CrearResult(dbModel.Telefono)
	//		)
	//	);
}
