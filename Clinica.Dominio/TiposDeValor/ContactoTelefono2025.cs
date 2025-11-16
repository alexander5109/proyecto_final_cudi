using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;
namespace Clinica.Dominio.TiposDeValor;
public readonly record struct ContactoTelefono2025(
	string Valor
){
	public static Result<ContactoTelefono2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ContactoTelefono2025>.Error("El teléfono no puede estar vacío.");
		if (!Regex.IsMatch(input, @"^\+?\d{6,15}$"))
			return new Result<ContactoTelefono2025>.Error("Teléfono inválido.");

		return new Result<ContactoTelefono2025>.Ok(new ContactoTelefono2025(input.Trim()));
	}
}