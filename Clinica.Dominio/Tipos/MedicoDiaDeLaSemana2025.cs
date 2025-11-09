using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoDiaDeLaSemana2025(
	DayOfWeek Value
) {
	public static Result<MedicoDiaDeLaSemana2025> Crear(DayOfWeek dia) {
		if (!Enum.IsDefined(typeof(DayOfWeek), dia))
			return new Result<MedicoDiaDeLaSemana2025>.Error("Día de la semana inválido.");

		return new Result<MedicoDiaDeLaSemana2025>.Ok(new(dia));
	}

	public static Result<MedicoDiaDeLaSemana2025> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<MedicoDiaDeLaSemana2025>.Error("El nombre del día no puede estar vacío.");

		string normalized = input.Trim().ToLowerInvariant();

		var mapping = new Dictionary<string, DayOfWeek>(StringComparer.OrdinalIgnoreCase) {
			// Español
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
			return new Result<MedicoDiaDeLaSemana2025>.Ok(new(day));

		return new Result<MedicoDiaDeLaSemana2025>.Error($"'{input}' no corresponde a un día válido.");
	}

	// --- Conversiones implícitas / explícitas ---
	public static implicit operator DayOfWeek(MedicoDiaDeLaSemana2025 dia) => dia.Value;
	public static implicit operator MedicoDiaDeLaSemana2025(DayOfWeek dia) => new(dia);

	// Conversión explícita desde string, porque puede fallar
	public static explicit operator MedicoDiaDeLaSemana2025(string input) {
		var result = Crear(input);
		return result is Result<MedicoDiaDeLaSemana2025>.Ok ok
			? ok.Value
			: throw new InvalidCastException($"No se pudo convertir '{input}' a MedicoDiaDeLaSemana2025.");
	}

	// --- ToString() en español ---
	public override string ToString() =>
		Value switch {
			DayOfWeek.Sunday => "Domingo",
			DayOfWeek.Monday => "Lunes",
			DayOfWeek.Tuesday => "Martes",
			DayOfWeek.Wednesday => "Miércoles",
			DayOfWeek.Thursday => "Jueves",
			DayOfWeek.Friday => "Viernes",
			DayOfWeek.Saturday => "Sábado",
			_ => Value.ToString()
		};
}
