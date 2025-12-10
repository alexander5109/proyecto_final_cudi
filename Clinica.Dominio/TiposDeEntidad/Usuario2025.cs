using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.TiposDeEntidad;


public sealed record Usuario2025(
	//UsuarioId Id,
	UserName UserName,
	NombreCompleto2025 NombreCompleto,
	ContraseñaHasheada PasswordHash,
	UsuarioRoleCodigo EnumRole,
	ContactoEmail2025 Email,
	ContactoTelefono2025 Telefono
) : IComoTexto {

	public string ATexto() => $"Usuario: {NombreCompleto.ATexto()} (Rol:{EnumRole}";

	public static Result<Usuario2025> CrearResult(
		Result<UserName> userNameResult,
		Result<NombreCompleto2025> nombreCompletoResult,
		Result<ContraseñaHasheada> passwordHashResult,
		Result<UsuarioRoleCodigo> enumRoleResult,
		Result<ContactoEmail2025> telefonoResult,
		Result<ContactoTelefono2025> emailResult
	)
		=> userNameResult.BindWithPrefix("Error en UserName:\n", userName
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
