using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;
namespace Clinica.Dominio.Tipos;
public readonly record struct ContactoEmailType(
	string Value
);

public static class ContactoEmail2025{
	public static Result<ContactoEmailType> Crear(this string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ContactoEmailType>.Error("El correo no puede estar vacío.");

		if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return new Result<ContactoEmailType>.Error("Correo electrónico inválido.");

		return new Result<ContactoEmailType>.Ok(new(input.Trim()));
	}
}
