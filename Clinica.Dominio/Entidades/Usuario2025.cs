using System.Security.Cryptography;
using System.Text;
using Clinica.Dominio.Comun;


namespace Clinica.Dominio.Entidades;

public readonly record struct UsuarioId(int Valor) {
	public static Result<UsuarioId> CrearResult(int? id) =>
		id is int idGood
		? new Result<UsuarioId>.Ok(new UsuarioId(idGood))
		: new Result<UsuarioId>.Error("El id no puede ser nulo.");
}

public readonly record struct NombreUsuario(string Valor);

public readonly record struct ContraseñaHasheada(string Valor) {
	public static ContraseñaHasheada CrearFromRaw(string raw) {
		byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		return new(Convert.ToHexString(hash));
	}

	public bool IgualA(string raw) {
		byte[] rawHash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		byte[] storedHash = Convert.FromHexString(Valor);
		return CryptographicOperations.FixedTimeEquals(rawHash, storedHash);
	}
}

public enum UsuarioEnumRole : byte {
	Nivel1Superadmin = 1,
	Nivel2Administrativo = 2,
	Nivel3Secretaria = 3,
	Nivel4Medico = 4
}

public record Usuario2025Agg(Usuario2025 Usuario, UsuarioId Id) {

	public static Usuario2025Agg Crear(
		UsuarioId id,
		Usuario2025 instance
	)
		=> new Usuario2025Agg(instance, id);

	//public static Result<Usuario2025Agg> CrearResult(
	//	UsuarioId id,
	//	Result<Usuario2025> instanceResult
	//)
	//	=> new Usuario2025Agg(instance,id);

	//public static Result<Usuario2025Agg> CrearResult(
	//	Result<UsuarioId> idResult,
	//	Result<Usuario2025> instanceResult
	//)
	//	=> 
	//	//from id in idResult
	//	   from instance in instanceResult
	//	   select new Usuario2025Agg(
	//		   instance,
	//		   id
	//	   );

}


public sealed record Usuario2025(
	//UsuarioId Id,
	NombreUsuario NombreUsuario,
	ContraseñaHasheada PasswordHash,
	UsuarioEnumRole EnumRole
) {
	public static Result<Usuario2025> CrearResult(string? nombreUsuario, string? passwordHash, UsuarioEnumRole enumRole) {
		if (string.IsNullOrEmpty(nombreUsuario)) {
			return new Result<Usuario2025>.Error("No se puede crear un usuario con el nombre vacio");
		}
		if (string.IsNullOrEmpty(passwordHash)) {
			return new Result<Usuario2025>.Error("No se puede crear un usuario sin contraseña");
		}
		return new Result<Usuario2025>.Ok(new Usuario2025(new NombreUsuario(nombreUsuario), new ContraseñaHasheada(passwordHash), enumRole));
	}

	public bool PasswordMatch(string raw) => PasswordHash.IgualA(raw);
}
