using Clinica.Dominio.Comun;
using System.Globalization;

namespace Clinica.Dominio.Tipos;

public record struct FechaDeIngreso2025(
	DateOnly Value
) {
	private static readonly DateOnly Hoy = DateOnly.FromDateTime(DateTime.Now);

	public static Result<FechaDeIngreso2025> Crear(DateOnly fecha) {
		if (fecha > Hoy)
			return new Result<FechaDeIngreso2025>.Error("La fecha de ingreso no puede ser futura.");
		if (fecha < Hoy.AddYears(-30))
			return new Result<FechaDeIngreso2025>.Error("Hace 30 años no existia la clínica.");

		return new Result<FechaDeIngreso2025>.Ok(new(fecha));
	}

	public static Result<FechaDeIngreso2025> Crear(DateTime fecha) {
		var dateOnly = DateOnly.FromDateTime(fecha);
		return Crear(dateOnly);
	}

	public static Result<FechaDeIngreso2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaDeIngreso2025>.Error("La fecha de ingreso no puede estar vacía.");

		// Soportar varios formatos razonables
		string[] formatos = [
			"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
		];

		if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
			return Crear(dt);

		if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt2))
			return Crear(dt2);

		return new Result<FechaDeIngreso2025>.Error("Formato de fecha inválido.");
	}

	public override string ToString() => Value.ToString("dd/MM/yyyy");

	//public Result<FechaDeIngreso2025> Validate() {
	//	if (Value > Hoy)
	//		return new Result<FechaDeIngreso2025>.Error("La fecha de ingreso no puede ser futura.");
	//	if (Value < Hoy.AddYears(-30))
	//		return new Result<FechaDeIngreso2025>.Error("Hace 30 años no existia la clínica.");
	//	return new Result<FechaDeIngreso2025>.Ok(this);
	//}
}
