using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;

namespace Clinica.Dominio.Tipos;

public readonly record struct CorreoElectronico(
	string Value
) {
	public static Result<CorreoElectronico> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<CorreoElectronico>.Error("El correo no puede estar vacío.");
		if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return new Result<CorreoElectronico>.Error("Correo electrónico inválido.");

		return new Result<CorreoElectronico>.Ok(new(input.Trim()));
	}

	// Conversión implícita hacia string
	public static implicit operator string(CorreoElectronico c) => c.Value;
}


