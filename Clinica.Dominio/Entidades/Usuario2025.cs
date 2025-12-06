using System.Security.Cryptography;
using System.Text;
using Clinica.Dominio.Comun;


namespace Clinica.Dominio.Entidades;

public readonly record struct UsuarioId(int Valor);
public readonly record struct NombreUsuario(string Valor);

public readonly record struct ContraseñaHasheada(string Valor) {
	public static ContraseñaHasheada CrearFromRaw(string raw) {
		var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
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

public sealed record Usuario2025(
	UsuarioId Id,
	NombreUsuario NombreUsuario,
	ContraseñaHasheada PasswordHash,
	UsuarioEnumRole EnumRole
) {
	public static Result<Usuario2025> CrearResult(UsuarioId id, string? nombreUsuario, string? passwordHash, UsuarioEnumRole enumRole) {
		if (string.IsNullOrEmpty(nombreUsuario)) {
			return new Result<Usuario2025>.Error("No se puede crear un usuario con el nombre vacio");
		}
		if (string.IsNullOrEmpty(passwordHash)) {
			return new Result<Usuario2025>.Error("No se puede crear un usuario sin contraseña");
		}
		return new Result<Usuario2025>.Ok(new Usuario2025(id, new NombreUsuario(nombreUsuario), new ContraseñaHasheada(passwordHash), enumRole));
	}

	public bool PasswordMatch(string raw) => PasswordHash.IgualA(raw);
}
