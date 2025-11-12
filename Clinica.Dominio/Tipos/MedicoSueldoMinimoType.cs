using Clinica.Dominio.Comun;
using System.Globalization;
using System.Runtime.CompilerServices;
namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoSueldoMinimoType(
	decimal Valor
);
public static class MedicoSueldoMinimo2025 {
	public static readonly decimal MINIMO = 200_000;
	public static readonly decimal MAXIMO = 5_000_000;


	public static Result<MedicoSueldoMinimoType> Crear(decimal input) {
		if (input < 0)
			return new Result<MedicoSueldoMinimoType>.Error("El sueldo no puede ser negativo.");
		if (input < MINIMO)
			return new Result<MedicoSueldoMinimoType>.Error($"El sueldo mínimo razonable es {MINIMO.ToString("N0")}.");
		if (input > MAXIMO)
			return new Result<MedicoSueldoMinimoType>.Error($"El sueldo ingresado ({input.ToString("N0")}) es excesivamente alto.");
		return new Result<MedicoSueldoMinimoType>.Ok(new MedicoSueldoMinimoType(input));
	}
	public static Result<MedicoSueldoMinimoType> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<MedicoSueldoMinimoType>.Error("El sueldo no puede estar vacío.");

		var normalized = input.Trim();

		if (!decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var valor) &&
			!decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.CurrentCulture, out valor)) {
			return new Result<MedicoSueldoMinimoType>.Error($"Valor inválido: '{input}'. Debe ser un número.");
		}

		if (valor < 0)
			return new Result<MedicoSueldoMinimoType>.Error("El sueldo no puede ser negativo.");

		if (valor < MINIMO)
			return new Result<MedicoSueldoMinimoType>.Error($"El sueldo mínimo razonable es {MINIMO.ToString("N0")}.");

		if (valor > MAXIMO)
			return new Result<MedicoSueldoMinimoType>.Error($"El sueldo ingresado ({valor.ToString("N0")}) es excesivamente alto.");

		return new Result<MedicoSueldoMinimoType>.Ok(new MedicoSueldoMinimoType(valor));
	}

	public static string AString(this MedicoSueldoMinimoType sueldo) => sueldo.Valor.ToString("C", CultureInfo.CurrentCulture);
}
