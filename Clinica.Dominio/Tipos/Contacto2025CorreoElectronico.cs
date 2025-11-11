using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;
namespace Clinica.Dominio.Tipos;

public record struct Contacto2025CorreoElectronico(
	string Value
	){
	public static Result<Contacto2025CorreoElectronico> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<Contacto2025CorreoElectronico>.Error("El correo no puede estar vacío.");

		if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return new Result<Contacto2025CorreoElectronico>.Error("Correo electrónico inválido.");

		return new Result<Contacto2025CorreoElectronico>.Ok(new(input.Trim()));
	}
}
