using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;
namespace Clinica.Dominio.Tipos;
public readonly record struct ContactoEmail2025(
	string Valor
){
	public static Result<ContactoEmail2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ContactoEmail2025>.Error("El correo no puede estar vacío.");

		if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return new Result<ContactoEmail2025>.Error("Correo electrónico inválido.");

		return new Result<ContactoEmail2025>.Ok(new(input.Trim()));
	}
}
