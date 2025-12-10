using System.Globalization;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;


public readonly record struct FechaDeNacimiento2025(DateOnly Valor) : IComoTexto {
	public string ATexto() {
		return Valor.ToString("dd/MM/yyyy");
	}

	public static readonly DateOnly Hoy = DateOnly.FromDateTime(DateTime.Now);

	public static Result<FechaDeNacimiento2025> CrearResult(DateOnly fecha) {
		if (fecha > Hoy)
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede ser futura.");
		if (fecha < Hoy.AddYears(-120))
			return new Result<FechaDeNacimiento2025>.Error("Edad no válida (más de 120 años).");

		return new Result<FechaDeNacimiento2025>.Ok(new(fecha));
	}

	public static Result<FechaDeNacimiento2025> CrearResult(DateTime? fecha) {
		if (fecha is null) {
			return new Result<FechaDeNacimiento2025>.Error("La fecha de ingreso no puede estar vacía.");
		}
		DateOnly dateOnly = DateOnly.FromDateTime(fecha.Value);
		return CrearResult(dateOnly);
	}

	public static Result<FechaDeNacimiento2025> CrearResult(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede estar vacía.");

		// Soportar varios formatos razonables
		string[] formatos = [
			"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
		];

		if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
			return CrearResult(dt);

		if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt2))
			return CrearResult(dt2);

		return new Result<FechaDeNacimiento2025>.Error("Formato de fecha inválido.");
	}

	// --- Edad aproximada ---
	public static int Edad(FechaDeNacimiento2025 fecha) {
		int edad = Hoy.Year - fecha.Valor.Year;
		if (Hoy < fecha.Valor.AddYears(edad)) edad--;
		return edad;
	}
}
