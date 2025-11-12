using Clinica.Dominio.Comun;
using System.Globalization;

namespace Clinica.Dominio.Tipos;

public readonly record struct DiaSemanaType(
	DayOfWeek Valor
);
public static class DiaSemana2025 {
	public static string AString(this DiaSemanaType diaSemana) => CultureInfo.GetCultureInfo("es-ES") .DateTimeFormat.GetDayName(diaSemana.Valor);

	public static Result<DiaSemanaType> Crear(int input) {
		if (input < 0 || input > 6)
			return new Result<DiaSemanaType>.Error("El número del día de la semana debe estar entre 0 (domingo) y 6 (sábado).");
		return new Result<DiaSemanaType>.Ok(new DiaSemanaType((DayOfWeek)input));
	}

	public static Result<DiaSemanaType> Crear(string input) {

		if (string.IsNullOrWhiteSpace(input))
			return new Result<DiaSemanaType>.Error("El nombre del día no puede estar vacío.");

		string normalized = input.Trim().ToLowerInvariant();

		var mapping = new Dictionary<string, DayOfWeek>(StringComparer.OrdinalIgnoreCase) {
			["domingo"] = DayOfWeek.Sunday,
			["lunes"] = DayOfWeek.Monday,
			["martes"] = DayOfWeek.Tuesday,
			["miercoles"] = DayOfWeek.Wednesday,
			["miércoles"] = DayOfWeek.Wednesday,
			["jueves"] = DayOfWeek.Thursday,
			["viernes"] = DayOfWeek.Friday,
			["sabado"] = DayOfWeek.Saturday,
			["sábado"] = DayOfWeek.Saturday,
			// Inglés
			["sunday"] = DayOfWeek.Sunday,
			["monday"] = DayOfWeek.Monday,
			["tuesday"] = DayOfWeek.Tuesday,
			["wednesday"] = DayOfWeek.Wednesday,
			["thursday"] = DayOfWeek.Thursday,
			["friday"] = DayOfWeek.Friday,
			["saturday"] = DayOfWeek.Saturday
		};

		if (mapping.TryGetValue(normalized, out var day))
			return new Result<DiaSemanaType>.Ok(new DiaSemanaType(day));

		// Intento adicional: cultura
		try {
			var culture = new CultureInfo("es-ES");
			for (int i = 0; i < 7; i++) {
				if (culture.DateTimeFormat.GetDayName((DayOfWeek)i).Equals(normalized, StringComparison.InvariantCultureIgnoreCase))
					return new Result<DiaSemanaType>.Ok(new DiaSemanaType((DayOfWeek)i));
			}
		} catch { }

		return new Result<DiaSemanaType>.Error($"'{input}' no corresponde a un día válido.");
	}
}
