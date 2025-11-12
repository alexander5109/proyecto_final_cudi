using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;

namespace Clinica.Dominio.Tipos;

public readonly record struct DniArgentinoType(
	string Value
);
public static class DniArgentino2025 {
	public static Result<DniArgentinoType> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<DniArgentinoType>.Error("El DNI no puede estar vacío.");
		var normalized = input.Trim();
		if (!Regex.IsMatch(normalized, @"^\d{1,8}$"))
			return new Result<DniArgentinoType>.Error("El DNI debe contener hasta 8 dígitos numéricos.");
		return new Result<DniArgentinoType>.Ok(new DniArgentinoType(normalized));
	}
}
