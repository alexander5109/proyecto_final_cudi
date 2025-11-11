using Clinica.Dominio.Comun;
using System.Globalization;

namespace Clinica.Dominio.Tipos;

public record struct FechaDeNacimiento2025(
	DateOnly Value
) : IValidate<FechaDeNacimiento2025> {
	private static readonly DateOnly Hoy = DateOnly.FromDateTime(DateTime.Now);

	public static Result<FechaDeNacimiento2025> Crear(DateOnly fecha) {
		if (fecha > Hoy)
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede ser futura.");
		if (fecha < Hoy.AddYears(-120))
			return new Result<FechaDeNacimiento2025>.Error("Edad no válida (más de 120 años).");

		return new Result<FechaDeNacimiento2025>.Ok(new(fecha));
	}

	public static Result<FechaDeNacimiento2025> Crear(DateTime fecha) {
		var dateOnly = DateOnly.FromDateTime(fecha);
		return Crear(dateOnly);
	}

	public static Result<FechaDeNacimiento2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede estar vacía.");

		// Soportar varios formatos razonables
		string[] formatos = [
			"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
		];

		if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
			return Crear(dt);

		if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt2))
			return Crear(dt2);

		return new Result<FechaDeNacimiento2025>.Error("Formato de fecha inválido.");
	}

	// --- Edad aproximada ---
	public int Edad {
		get {
			var hoy = DateOnly.FromDateTime(DateTime.Now);
			int edad = hoy.Year - Value.Year;
			if (hoy < Value.AddYears(edad)) edad--;
			return edad;
		}
	}

	public override string ToString() => Value.ToString("dd/MM/yyyy");

	//public Result<FechaDeNacimiento2025> Validate() {
	//	if (Value > Hoy)
	//		return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede ser futura.");
	//	if (Value < Hoy.AddYears(-120))
	//		return new Result<FechaDeNacimiento2025>.Error("Edad no válida (más de 120 años).");
	//	return new Result<FechaDeNacimiento2025>.Ok(this);
	//}
}
