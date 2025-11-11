using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;
namespace Clinica.Dominio.Tipos;
public record struct Contacto2025Telefono(
	string Value
) : IValidate<Contacto2025Telefono> 
{
	public static Result<Contacto2025Telefono> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<Contacto2025Telefono>.Error("El teléfono no puede estar vacío.");
		if (!Regex.IsMatch(input, @"^\+?\d{6,15}$"))
			return new Result<Contacto2025Telefono>.Error("Teléfono inválido.");

		return new Result<Contacto2025Telefono>.Ok(new Contacto2025Telefono(input.Trim()));
	}

	//public Result<Contacto2025Telefono> Validate() {
	//	if (string.IsNullOrWhiteSpace(Value))
	//		return new Result<Contacto2025Telefono>.Error("El teléfono no puede estar vacío.");
	//	if (!Regex.IsMatch(Value, @"^\+?\d{6,15}$"))
	//		return new Result<Contacto2025Telefono>.Error("Teléfono inválido.");

	//	return new Result<Contacto2025Telefono>.Ok(this);
	//}

	// Conversión implícita hacia string
	//public static implicit operator string?(Contacto2025Telefono c) => c.Value;
}
