using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.TiposDeEntidad;

public sealed record Usuario2025Edicion(
	UserName2025 UserName,
	NombreCompleto2025 NombreCompleto,
	ContraseñaHasheada2025? NuevaContraseña, // intención explícita
	UsuarioRoleEnum EnumRole,
	Email2025 Email,
	Telefono2025 Telefono
) {

	public static Result<Usuario2025Edicion> CrearResult(
		Result<UserName2025> userNameResult,
		Result<NombreCompleto2025> nombreCompletoResult,
		Result<ContraseñaHasheada2025?> passwordHashResult,
		Result<UsuarioRoleEnum> enumRoleResult,
		Result<Email2025> telefonoResult,
		Result<Telefono2025> emailResult
	)
		=> userNameResult.BindWithPrefix("Error en UserName2025:\n", userName
		=> nombreCompletoResult.BindWithPrefix("Error en NombreCompleto:\n", nombreCompleto
		=> passwordHashResult.BindWithPrefix("Error en PasswordHash:\n", passwordHash
		=> enumRoleResult.BindWithPrefix("Error en el codigo de rol:\n", enumRole
		=> telefonoResult.BindWithPrefix("Error en Telefono:\n", telefono
		=> emailResult.BindWithPrefix("Error en Email:\n", email
		=> new Result<Usuario2025Edicion>.Ok(
			new Usuario2025Edicion(
				userName,
				nombreCompleto,
				passwordHash,
				enumRole,
				telefono,
				email
			)
		)))))));

}


public sealed record Usuario2025(
	//UsuarioId Id,
	UserName2025 UserName,
	NombreCompleto2025 NombreCompleto,
	ContraseñaHasheada2025 PasswordHash,
	UsuarioRoleEnum EnumRole,
	Email2025 Email,
	Telefono2025 Telefono
) : IComoTexto {

	public string ATexto() => $"Usuario: {NombreCompleto.ATexto()} (Rol:{EnumRole}";

	public static Result<Usuario2025> CrearResult(
		Result<UserName2025> userNameResult,
		Result<NombreCompleto2025> nombreCompletoResult,
		Result<ContraseñaHasheada2025> passwordHashResult,
		Result<UsuarioRoleEnum> enumRoleResult,
		Result<Email2025> telefonoResult,
		Result<Telefono2025> emailResult
	)
		=> userNameResult.BindWithPrefix("Error en UserName2025:\n", userName
		=> nombreCompletoResult.BindWithPrefix("Error en NombreCompleto:\n", nombreCompleto
		=> passwordHashResult.BindWithPrefix("Error en PasswordHash:\n", passwordHash
		=> enumRoleResult.BindWithPrefix("Error en el codigo de rol:\n", enumRole
		=> telefonoResult.BindWithPrefix("Error en Telefono:\n", telefono
		=> emailResult.BindWithPrefix("Error en Email:\n", email
		=> new Result<Usuario2025>.Ok(
			new Usuario2025(
				userName,
				nombreCompleto,
				passwordHash,
				enumRole,
				telefono,
				email
			)
		)))))));

}
