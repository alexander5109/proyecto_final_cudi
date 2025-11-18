using Clinica.Dominio.Comun;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Clinica.Dominio.TiposDeValor;


public readonly record struct DiaSemana2025(
	DayOfWeek Valor
	//, string NombreDia
) {

	public static Result<DiaSemana2025> Crear(DayOfWeek input) {
		return new Result<DiaSemana2025>.Ok(new DiaSemana2025(input));
	}

	public static Result<DiaSemana2025> Crear(DiaSemana2025 input) {
		return new Result<DiaSemana2025>.Ok(input);
	}
	public static Result<DiaSemana2025> Crear(int input) {
		if (input < 0 || input > 6)
			return new Result<DiaSemana2025>.Error("El número del día de la semana debe estar entre 0 (domingo) y 6 (sábado).");
		return new Result<DiaSemana2025>.Ok(new DiaSemana2025((DayOfWeek)input));
	}
	public static Result<DiaSemana2025> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<DiaSemana2025>.Error("El nombre del día no puede estar vacío.");
		string normalized = input.Trim().ToLowerInvariant();

		if (DayOfWeekExtentions.DiasDeLaSemanaToEnum.TryGetValue(normalized, out var day))
			return new Result<DiaSemana2025>.Ok(new DiaSemana2025(day));

		// Intento adicional: cultura
		try {
			var culture = new CultureInfo("es-ES");
			for (int i = 0; i < 7; i++) {
				if (culture.DateTimeFormat.GetDayName((DayOfWeek)i).Equals(normalized, StringComparison.InvariantCultureIgnoreCase))
					return new Result<DiaSemana2025>.Ok(new DiaSemana2025((DayOfWeek)i));
			}
		} catch { }

		return new Result<DiaSemana2025>.Error($"'{input}' no corresponde a un día válido.");
	}

	public static readonly DiaSemana2025[] Los7DiaSemana2025 = [
		new(DayOfWeek.Monday), //Value 1
		new(DayOfWeek.Tuesday), //Value 2
		new(DayOfWeek.Wednesday),//Value 3
		new(DayOfWeek.Thursday),//Value 4
		new(DayOfWeek.Friday),//Value 5
		new(DayOfWeek.Saturday), //Value 6
		new(DayOfWeek.Sunday), //Value 0
	];

	public static readonly string[] Los7StringDias = [
		DayOfWeek.Monday.AEspañol(), //Value 1
		DayOfWeek.Tuesday.AEspañol(), //Value 2
		DayOfWeek.Wednesday.AEspañol(),//Value 3
		DayOfWeek.Thursday.AEspañol(),//Value 4
		DayOfWeek.Friday.AEspañol(),//Value 5
		DayOfWeek.Saturday.AEspañol(), //Value 6
		DayOfWeek.Sunday.AEspañol(), //Value 0
	];

	public static readonly DayOfWeek[] Los7EnumDias = [
		DayOfWeek.Monday, //Value 1
		DayOfWeek.Tuesday, //Value 2
		DayOfWeek.Wednesday,//Value 3
		DayOfWeek.Thursday,//Value 4
		DayOfWeek.Friday,//Value 5
		DayOfWeek.Saturday, //Value 6
		DayOfWeek.Sunday, //Value 0
	];
}
public static class DayOfWeekExtentions {
	public static Dictionary<string, DayOfWeek> DiasDeLaSemanaToEnum = new(StringComparer.OrdinalIgnoreCase) {
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
	public static string AEspañol(this DayOfWeek dia) => dia switch {
		DayOfWeek.Monday => "Lunes",
		DayOfWeek.Tuesday => "Martes",
		DayOfWeek.Wednesday => "Miércoles",
		DayOfWeek.Thursday => "Jueves",
		DayOfWeek.Friday => "Viernes",
		DayOfWeek.Saturday => "Sábado",
		DayOfWeek.Sunday => "Domingo",
		_ => throw new ArgumentOutOfRangeException(nameof(dia), dia, null)
	};


}