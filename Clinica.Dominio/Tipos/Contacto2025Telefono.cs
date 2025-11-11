using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;
namespace Clinica.Dominio.Tipos;
public readonly record struct Contacto2025Telefono(
	string Value
) {
	public static Result<Contacto2025Telefono> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<Contacto2025Telefono>.Error("El teléfono no puede estar vacío.");
		if (!Regex.IsMatch(input, @"^\+?\d{6,15}$"))
			return new Result<Contacto2025Telefono>.Error("Teléfono inválido.");

		return new Result<Contacto2025Telefono>.Ok(new Contacto2025Telefono(input.Trim()));
	}
}
