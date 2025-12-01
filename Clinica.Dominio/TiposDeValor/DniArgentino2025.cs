using System.Text.RegularExpressions;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct DniArgentino2025(
	string Valor
) {
	public static Result<DniArgentino2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<DniArgentino2025>.Error("El DNI no puede estar vacío.");
        string normalized = input.Trim();
		if (!Regex.IsMatch(normalized, @"^\d{1,8}$"))
			return new Result<DniArgentino2025>.Error("El DNI debe contener hasta 8 dígitos numéricos.");
		return new Result<DniArgentino2025>.Ok(new DniArgentino2025(normalized));
	}
}
