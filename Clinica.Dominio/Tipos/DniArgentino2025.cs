using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;

namespace Clinica.Dominio.Tipos;

public readonly struct DniArgentino2025 {
	private readonly string _value;

	private DniArgentino2025(string value) => _value = value;

	public static Result<DniArgentino2025> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<DniArgentino2025>.Error("El DNI no puede estar vacío.");

		var normalized = input.Trim();

		if (!Regex.IsMatch(normalized, @"^\d{1,8}$"))
			return new Result<DniArgentino2025>.Error("El DNI debe contener hasta 8 dígitos numéricos.");

		return new Result<DniArgentino2025>.Ok(new DniArgentino2025(normalized));
	}

	public override string ToString() => _value;

	public static implicit operator string(DniArgentino2025 d) => d._value;
}


