using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;
namespace Clinica.Dominio.Tipos;
public readonly record struct ContactoTelefonoType(
	string Value
);
public static class ContactoTelefono2025{
	public static Result<ContactoTelefonoType> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ContactoTelefonoType>.Error("El teléfono no puede estar vacío.");
		if (!Regex.IsMatch(input, @"^\+?\d{6,15}$"))
			return new Result<ContactoTelefonoType>.Error("Teléfono inválido.");

		return new Result<ContactoTelefonoType>.Ok(new ContactoTelefonoType(input.Trim()));
	}
}
