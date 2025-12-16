using System.Security.Cryptography;
using System.Text;
using Clinica.Dominio.FunctionalToolkit;


namespace Clinica.Dominio.TiposDeValor;

public readonly record struct ContraseñaHasheada2025(string Valor) {
	private const int MinLength = 6;
	private const int MaxLength = 64;

	public static Result<ContraseñaHasheada2025> CrearResult(string hash) {
		//if (string.IsNullOrWhiteSpace(hash))
		//	return new Result<ContraseñaHasheada2025>.Error("El hash no puede estar vacío.");

		if (hash.Length != 64 && hash.Length != 128)
			return new Result<ContraseñaHasheada2025>.Error("El hash tiene un largo inválido.");

		try {
			_ = Convert.FromHexString(hash);
		} catch {
			return new Result<ContraseñaHasheada2025>.Error("El hash no tiene formato hex válido.");
		}

		return new Result<ContraseñaHasheada2025>.Ok(new(hash));
	}

	// ⭐ NUEVO
	public static Result<ContraseñaHasheada2025> CrearResultFromRaw(string? raw) {
		if (string.IsNullOrWhiteSpace(raw))
			return new Result<ContraseñaHasheada2025>.Error(
				"La contraseña no puede estar vacía."
			);

		if (raw.Length < MinLength)
			return new Result<ContraseñaHasheada2025>.Error(
				$"La contraseña debe tener al menos {MinLength} caracteres."
			);

		if (raw.Length > MaxLength)
			return new Result<ContraseñaHasheada2025>.Error(
				$"La contraseña no puede superar los {MaxLength} caracteres."
			);

		byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		string hex = Convert.ToHexString(hash);

		return new Result<ContraseñaHasheada2025>.Ok(
			new ContraseñaHasheada2025(hex)
		);
	}

	public static bool RawIdenticalToHashed(string raw, string hashed) {
		byte[] rawHash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		byte[] storedHash = Convert.FromHexString(hashed);
		return CryptographicOperations.FixedTimeEquals(rawHash, storedHash);
	}
}
