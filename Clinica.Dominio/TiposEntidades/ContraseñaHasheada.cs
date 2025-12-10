using System.Security.Cryptography;
using System.Text;
using Clinica.Dominio.FunctionalToolkit;


namespace Clinica.Dominio.TiposDeValor;

public readonly record struct ContraseñaHasheada(string Valor) {

	public static ContraseñaHasheada CrearFromRaw(string raw) {
		byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		return new(Convert.ToHexString(hash));
	}

	public static Result<ContraseñaHasheada> CrearResult(string? hash) {
		if (string.IsNullOrWhiteSpace(hash))
			return new Result<ContraseñaHasheada>.Error("El hash no puede estar vacío.");

		// Esto solo valida formato hex
		if (hash.Length != 64 && hash.Length != 128)
			return new Result<ContraseñaHasheada>.Error("El hash tiene un largo inválido.");

		try {
			_ = Convert.FromHexString(hash);
		} catch {
			return new Result<ContraseñaHasheada>.Error("El hash no tiene formato hex válido.");
		}

		return new Result<ContraseñaHasheada>.Ok(new ContraseñaHasheada(hash));
	}

	public bool IgualA(string raw) {
		byte[] rawHash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		byte[] storedHash = Convert.FromHexString(Valor);
		return CryptographicOperations.FixedTimeEquals(rawHash, storedHash);
	}
}
