using Clinica.Dominio.Comun;
using System.Globalization;
using System.Runtime.CompilerServices;
namespace Clinica.Dominio.TiposDeValor;

public readonly record struct MedicoSueldoMinimo2025(
	double Valor
) {
	public const double MINIMO = 200_000;
	public const double MAXIMO = 5_000_000;
	public static Result<MedicoSueldoMinimo2025> Crear(double? input) {
		return input switch {
			null => new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede estar vacío."),
			< 0 => new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede ser negativo."),
			< MINIMO => new Result<MedicoSueldoMinimo2025>.Error($"El sueldo mínimo razonable es {MINIMO:N0}."),
			> MAXIMO => new Result<MedicoSueldoMinimo2025>.Error($"El sueldo ingresado ({input}) es excesivamente alto."),
			_ => new Result<MedicoSueldoMinimo2025>.Ok(new MedicoSueldoMinimo2025(input.Value))
		};
	}
	public static Result<MedicoSueldoMinimo2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede estar vacío.");

		var normalized = input.Trim();

		if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var valor) &&
			!double.TryParse(normalized, NumberStyles.Float, CultureInfo.CurrentCulture, out valor)) {
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
