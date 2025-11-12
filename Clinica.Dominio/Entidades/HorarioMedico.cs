using System.Globalization;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;

public readonly record struct HorarioMedico(
	DayOfWeek DiaSemana,
	TimeOnly Desde,
	TimeOnly Hasta
) {
	public static Result<HorarioMedico> Crear(DayOfWeek dia, TimeOnly desde, TimeOnly hasta) {
		if (!Enum.IsDefined(typeof(DayOfWeek), dia))
			return new Result<HorarioMedico>.Error("El día de la semana es inválido.");

		if (desde >= hasta)
			return new Result<HorarioMedico>.Error("La hora de inicio debe ser anterior a la hora de fin.");

		return new Result<HorarioMedico>.Ok(new HorarioMedico(dia, desde, hasta));
	}

	public bool SeSolapaCon(HorarioMedico otro)
		=> DiaSemana == otro.DiaSemana && Desde < otro.Hasta && otro.Desde < Hasta;

	public static Result<DayOfWeek> ParseDia(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<DayOfWeek>.Error("El nombre del día no puede estar vacío.");

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
			return new Result<DayOfWeek>.Ok(day);

		// Intento adicional: usar la cultura actual (ej. es-AR)
		try {
			var culture = new CultureInfo("es-ES");
			var dateTimeFormat = culture.DateTimeFormat;
			for (int i = 0; i < 7; i++) {
				if (dateTimeFormat.GetDayName((DayOfWeek)i).ToLowerInvariant() == normalized)
					return new Result<DayOfWeek>.Ok((DayOfWeek)i);
			}
		} catch { }

		return new Result<DayOfWeek>.Error($"'{input}' no corresponde a un día válido.");
	}

	public string NombreDia =>
		CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetDayName(DiaSemana).CapitalizeFirst();

	public override string ToString() => $"{NombreDia} {Desde:HH:mm}–{Hasta:HH:mm}";
}

internal static class StringExtensions {
	public static string CapitalizeFirst(this string input) {
		if (string.IsNullOrEmpty(input)) return input;
		return char.ToUpper(input[0]) + input.Substring(1);
	}
}
