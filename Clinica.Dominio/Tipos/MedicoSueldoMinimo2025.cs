using Clinica.Dominio.Comun;
using System.Globalization;
using System.Runtime.CompilerServices;
namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoSueldoMinimo2025(
	decimal Valor
){
	public static readonly decimal MINIMO = 200_000;
	public static readonly decimal MAXIMO = 5_000_000;
	public static Result<MedicoSueldoMinimo2025> Crear(decimal input) {
		if (input < 0)
			return new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede ser negativo.");
		if (input < MINIMO)
			return new Result<MedicoSueldoMinimo2025>.Error($"El sueldo mínimo razonable es {MINIMO.ToString("N0")}.");
		if (input > MAXIMO)
			return new Result<MedicoSueldoMinimo2025>.Error($"El sueldo ingresado ({input.ToString("N0")}) es excesivamente alto.");
		return new Result<MedicoSueldoMinimo2025>.Ok(new MedicoSueldoMinimo2025(input));
	}
	public static Result<MedicoSueldoMinimo2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede estar vacío.");

		var normalized = input.Trim();

		if (!decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var valor) &&
			!decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.CurrentCulture, out valor)) {
			return new Result<MedicoSueldoMinimo2025>.Error($"Valor inválido: '{input}'. Debe ser un número.");
		}

		if (valor < 0)
			return new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede ser negativo.");

		if (valor < MINIMO)
			return new Result<MedicoSueldoMinimo2025>.Error($"El sueldo mínimo razonable es {MINIMO.ToString("N0")}.");

		if (valor > MAXIMO)
			return new Result<MedicoSueldoMinimo2025>.Error($"El sueldo ingresado ({valor.ToString("N0")}) es excesivamente alto.");

		return new Result<MedicoSueldoMinimo2025>.Ok(new MedicoSueldoMinimo2025(valor));
	}

}
