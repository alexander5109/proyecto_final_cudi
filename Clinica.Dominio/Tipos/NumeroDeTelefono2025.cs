using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;

namespace Clinica.Dominio.Tipos;

public readonly record struct NumeroDeTelefono2025 {
	private readonly string _value;
	private NumeroDeTelefono2025(string value) => _value = value;

	public static Result<NumeroDeTelefono2025> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<NumeroDeTelefono2025>.Error("El teléfono no puede estar vacío.");
		if (!Regex.IsMatch(input, @"^\+?\d{6,15}$"))
			return new Result<NumeroDeTelefono2025>.Error("Teléfono inválido.");

		return new Result<NumeroDeTelefono2025>.Ok(new(input.Trim()));
	}

	public override string ToString() => _value;
}
