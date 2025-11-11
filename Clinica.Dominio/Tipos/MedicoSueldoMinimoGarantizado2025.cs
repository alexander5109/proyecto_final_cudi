using Clinica.Dominio.Comun;
using System.Globalization;
namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoSueldoMinimoGarantizado2025(
	double Value
) {
	public static readonly double MINIMO = 200_000;
	public static readonly double MAXIMO = 5_000_000;

	public static Result<MedicoSueldoMinimoGarantizado2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<MedicoSueldoMinimoGarantizado2025>.Error("El sueldo no puede estar vacío.");

		var normalized = input.Trim();

		if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var valor) &&
			!double.TryParse(normalized, NumberStyles.Float, CultureInfo.CurrentCulture, out valor)) {
			return new Result<MedicoSueldoMinimoGarantizado2025>.Error($"Value inválido: '{input}'. Debe ser un número.");
		}

		if (valor < 0)
			return new Result<MedicoSueldoMinimoGarantizado2025>.Error("El sueldo no puede ser negativo.");

		if (valor < MINIMO)
			return new Result<MedicoSueldoMinimoGarantizado2025>.Error($"El sueldo mínimo razonable es {MINIMO.ToString("N0")}.");

		if (valor > MAXIMO)
			return new Result<MedicoSueldoMinimoGarantizado2025>.Error($"El sueldo ingresado ({valor.ToString("N0")}) es excesivamente alto.");

		return new Result<MedicoSueldoMinimoGarantizado2025>.Ok(new MedicoSueldoMinimoGarantizado2025(valor));
	}

	public override string ToString() => Value.ToString("C", CultureInfo.CurrentCulture);
}
