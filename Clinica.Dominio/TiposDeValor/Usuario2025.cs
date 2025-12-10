using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct UsuarioId(int Valor) {
	public static Result<UsuarioId> CrearResult(int? id) =>
		id is int idGood
		? new Result<UsuarioId>.Ok(new UsuarioId(idGood))
		: new Result<UsuarioId>.Error("El id no puede ser nulo.");
}


public enum UsuarioEnumRole : byte {
	Nivel1Superadmin = 1,
	Nivel2Administrativo = 2,
	Nivel3Secretaria = 3,
	Nivel4Medico = 4
}



public record Usuario2025Agg(UsuarioId Id, Usuario2025 Usuario) {

	public static Usuario2025Agg Crear(
		UsuarioId id,
		Usuario2025 instance
	) => new Usuario2025Agg(id, instance);

	public static Result<Usuario2025Agg> CrearResult(Result<UsuarioId> idResult, Result<Usuario2025> instanceResult)
		=> from id in idResult
		   from instance in instanceResult
		   select new Usuario2025Agg(
			   id,
			   instance
		   );

}


public sealed record Usuario2025(
	//UsuarioId Id,
	UserName UserName,
	NombreCompleto2025 NombreCompleto,
	ContraseñaHasheada PasswordHash,
	UsuarioEnumRole EnumRole,
	ContactoEmail2025 Email,
	ContactoTelefono2025 Telefono
) : IComoTexto {

	public string ATexto() => $"Usuario: {NombreCompleto.ATexto()} (Rol:{EnumRole}";

	public static Result<Usuario2025> CrearResult(
		Result<UserName> userNameResult,
		Result<NombreCompleto2025> nombreCompletoResult,
		Result<ContraseñaHasheada> passwordHashResult,
		Result<UsuarioEnumRole> enumRoleResult,
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



	public bool PasswordMatch(string raw) => PasswordHash.IgualA(raw);
}
