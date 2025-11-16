using Clinica.Dominio.Comun;
using System.Globalization;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct FechaIngreso2025(
	DateOnly Valor
) {
	public static readonly DateOnly Hoy = DateOnly.FromDateTime(DateTime.Now);

	public static Result<FechaIngreso2025> Crear(DateOnly fecha) {
		if (fecha > Hoy)
			return new Result<FechaIngreso2025>.Error("La fecha de ingreso no puede ser futura.");
		if (fecha < Hoy.AddYears(-30))
			return new Result<FechaIngreso2025>.Error("Hace 30 años no existia la clínica.");

		return new Result<FechaIngreso2025>.Ok(new(fecha));
	}

	public static Result<FechaIngreso2025> Crear(DateTime? fecha) {
		if (fecha is null) {
			return new Result<FechaIngreso2025>.Error("La fecha de ingreso no puede estar vacía.");
		}

		var dateOnly = DateOnly.FromDateTime(fecha.Value);
		return Crear(dateOnly);
	}

	public static Result<FechaIngreso2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaIngreso2025>.Error("La fecha de ingreso no puede estar vacía.");

		// Soportar varios formatos razonables
		string[] formatos = [
			"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
		];

		if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
			return Crear(dt);

		if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt2))
			return Crear(dt2);

		return new Result<FechaIngreso2025>.Error("Formato de fecha inválido.");
	}


}
