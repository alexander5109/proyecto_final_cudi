using Clinica.Dominio.Comun;
using System.Globalization;

namespace Clinica.Dominio.Tipos;

public readonly record struct HorarioDiaSemanaType(
	DayOfWeek Value
);
public static class HorarioDiaSemana2025 {
	public static string NombreDia(this HorarioDiaSemanaType diaSemana)
		=> CultureInfo.GetCultureInfo("es-ES")
			.DateTimeFormat.GetDayName(diaSemana.Value);

	public static Result<HorarioDiaSemanaType> Create(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<HorarioDiaSemanaType>.Error("El nombre del día no puede estar vacío.");

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
			return new Result<HorarioDiaSemanaType>.Ok(new HorarioDiaSemanaType(day));

		// Intento adicional: cultura
		try {
			var culture = new CultureInfo("es-ES");
			for (int i = 0; i < 7; i++) {
				if (culture.DateTimeFormat.GetDayName((DayOfWeek)i).Equals(normalized, StringComparison.InvariantCultureIgnoreCase))
					return new Result<HorarioDiaSemanaType>.Ok(new HorarioDiaSemanaType((DayOfWeek)i));
			}
		} catch { }

		return new Result<HorarioDiaSemanaType>.Error($"'{input}' no corresponde a un día válido.");
	}
}
