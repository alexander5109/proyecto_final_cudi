using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;
namespace Clinica.Dominio.Tipos;

public record class Contacto2025CorreoElectronico(
	string Value
) : IValidate<Contacto2025CorreoElectronico> {

	public static Result<Contacto2025CorreoElectronico> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<Contacto2025CorreoElectronico>.Error("El correo no puede estar vacío.");

		if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return new Result<Contacto2025CorreoElectronico>.Error("Correo electrónico inválido.");

		return new Result<Contacto2025CorreoElectronico>.Ok(new(input.Trim()));
	}
	public Result<Contacto2025CorreoElectronico> Validate() {
		if (string.IsNullOrWhiteSpace(Value))
			return new Result<Contacto2025CorreoElectronico>.Error("El correo no puede estar vacío.");

		if (!Regex.IsMatch(Value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return new Result<Contacto2025CorreoElectronico>.Error("Correo electrónico inválido.");

		return new Result<Contacto2025CorreoElectronico>.Ok(this);
	}

	// Conversión implícita hacia string
	public static implicit operator string?(Contacto2025CorreoElectronico c) => c.Value;
}
