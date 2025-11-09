using Clinica.Dominio.Comun;
using System.Text.RegularExpressions;

namespace Clinica.Dominio.Types;

public readonly struct DniArgentino {
	private readonly string _value;

	private DniArgentino(string value) => _value = value;

	public static Result<DniArgentino> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<DniArgentino>.Error("El DNI no puede estar vacío.");

		var normalized = input.Trim();

		if (!Regex.IsMatch(normalized, @"^\d{1,8}$"))
			return new Result<DniArgentino>.Error("El DNI debe contener hasta 8 dígitos numéricos.");

		return new Result<DniArgentino>.Ok(new DniArgentino(normalized));
	}

	public override string ToString() => _value;

	public static implicit operator string(DniArgentino d) => d._value;
}


