using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;

namespace Clinica.Dominio.Types;

public readonly record struct NumeroDeTelefono {
	private readonly string _value;
	private NumeroDeTelefono(string value) => _value = value;

	public static Result<NumeroDeTelefono> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<NumeroDeTelefono>.Error("El teléfono no puede estar vacío.");
		if (!Regex.IsMatch(input, @"^\+?\d{6,15}$"))
			return new Result<NumeroDeTelefono>.Error("Teléfono inválido.");

		return new Result<NumeroDeTelefono>.Ok(new(input.Trim()));
	}

	public override string ToString() => _value;
}
