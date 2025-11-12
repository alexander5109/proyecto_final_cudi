using Clinica.Dominio.Comun;
using System.Globalization;

namespace Clinica.Dominio.Tipos;

public readonly record struct FechaIngresoType(
	DateOnly Valor
);
public static class FechaIngreso2025 {
	private static readonly DateOnly Hoy = DateOnly.FromDateTime(DateTime.Now);

	public static Result<FechaIngresoType> Crear(DateOnly fecha) {
		if (fecha > Hoy)
			return new Result<FechaIngresoType>.Error("La fecha de ingreso no puede ser futura.");
		if (fecha < Hoy.AddYears(-30))
			return new Result<FechaIngresoType>.Error("Hace 30 años no existia la clínica.");

		return new Result<FechaIngresoType>.Ok(new(fecha));
	}

	public static Result<FechaIngresoType> Crear(DateTime fecha) {
		var dateOnly = DateOnly.FromDateTime(fecha);
		return Crear(dateOnly);
	}

	public static Result<FechaIngresoType> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaIngresoType>.Error("La fecha de ingreso no puede estar vacía.");

		// Soportar varios formatos razonables
		string[] formatos = [
			"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
		];

		if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
			return Crear(dt);

		if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt2))
			return Crear(dt2);

		return new Result<FechaIngresoType>.Error("Formato de fecha inválido.");
	}

	public static string AString(this FechaIngresoType fecha) => fecha.Valor.ToString("dd/MM/yyyy");

}
