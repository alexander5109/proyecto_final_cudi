using Clinica.Dominio.Comun;
using System.Globalization;

namespace Clinica.Dominio.Tipos;

public readonly record struct FechaDeNacimientoType(
	DateOnly Valor
);
public static class FechaDeNacimiento2025 {
	private static readonly DateOnly Hoy = DateOnly.FromDateTime(DateTime.Now);

	public static Result<FechaDeNacimientoType> Crear(DateOnly fecha) {
		if (fecha > Hoy)
			return new Result<FechaDeNacimientoType>.Error("La fecha de nacimiento no puede ser futura.");
		if (fecha < Hoy.AddYears(-120))
			return new Result<FechaDeNacimientoType>.Error("Edad no válida (más de 120 años).");

		return new Result<FechaDeNacimientoType>.Ok(new(fecha));
	}

	public static Result<FechaDeNacimientoType> Crear(DateTime fecha) {
		var dateOnly = DateOnly.FromDateTime(fecha);
		return Crear(dateOnly);
	}

	public static Result<FechaDeNacimientoType> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaDeNacimientoType>.Error("La fecha de nacimiento no puede estar vacía.");

		// Soportar varios formatos razonables
		string[] formatos = [
			"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
		];

		if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
			return Crear(dt);

		if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt2))
			return Crear(dt2);

		return new Result<FechaDeNacimientoType>.Error("Formato de fecha inválido.");
	}

	// --- Edad aproximada ---
	public static int Edad(this FechaDeNacimientoType fecha) { 
		var hoy = DateOnly.FromDateTime(DateTime.Now);
		int edad = hoy.Year - fecha.Valor.Year;
		if (hoy < fecha.Valor.AddYears(edad)) edad--;
		return edad;
	}

	public static string AString(this FechaDeNacimientoType fecha) => fecha.Valor.ToString("dd/MM/yyyy");
}
