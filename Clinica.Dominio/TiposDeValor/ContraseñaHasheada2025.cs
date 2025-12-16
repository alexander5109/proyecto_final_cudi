using System.Security.Cryptography;
using System.Text;
using Clinica.Dominio.FunctionalToolkit;


namespace Clinica.Dominio.TiposDeValor;

public readonly record struct ContraseñaHasheada2025(string Valor) {

	public static Result<ContraseñaHasheada2025> CrearResult(string? hash) {
		if (string.IsNullOrWhiteSpace(hash))
			return new Result<ContraseñaHasheada2025>.Error("El hash no puede estar vacío.");

		// Esto solo valida formato hex
		if (hash.Length != 64 && hash.Length != 128)
			return new Result<ContraseñaHasheada2025>.Error("El hash tiene un largo inválido.");

		try {
			_ = Convert.FromHexString(hash);
		} catch {
			return new Result<ContraseñaHasheada2025>.Error("El hash no tiene formato hex válido.");
		}

		return new Result<ContraseñaHasheada2025>.Ok(new ContraseñaHasheada2025(hash));
	}

	public static ContraseñaHasheada2025 CrearFromRaw(string raw) {
		byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		return new(Convert.ToHexString(hash));
	}

	static public bool RawIdenticalToHashed(string raw, string hashed) {
		byte[] rawHash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		byte[] storedHash = Convert.FromHexString(hashed);
		return CryptographicOperations.FixedTimeEquals(rawHash, storedHash);
	}
}
