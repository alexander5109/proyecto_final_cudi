using System.Globalization;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;


public record FechaDeNacimiento2025(DateOnly Valor) : IComoTexto {
	public string ATexto() {
		return Valor.ToString("dd/MM/yyyy");
	}

	public static readonly DateOnly Hoy = DateOnly.FromDateTime(DateTime.Now);

	public static Result<FechaDeNacimiento2025> Crear(DateOnly fecha) {
		if (fecha > Hoy)
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede ser futura.");
		if (fecha < Hoy.AddYears(-120))
			return new Result<FechaDeNacimiento2025>.Error("Edad no válida (más de 120 años).");

		return new Result<FechaDeNacimiento2025>.Ok(new(fecha));
	}

	public static Result<FechaDeNacimiento2025> Crear(DateTime? fecha) {
		if (fecha is null) {
			return new Result<FechaDeNacimiento2025>.Error("La fecha de ingreso no puede estar vacía.");
		}
		DateOnly dateOnly = DateOnly.FromDateTime(fecha.Value);
		return Crear(dateOnly);
	}

	public static Result<FechaDeNacimiento2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede estar vacía.");

		// Soportar varios formatos razonables
		string[] formatos = [
			"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
		];

		if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
			return Crear(dt);

		if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt2))
			return Crear(dt2);

		return new Result<FechaDeNacimiento2025>.Error("Formato de fecha inválido.");
	}

	// --- Edad aproximada ---
	public static int Edad(FechaDeNacimiento2025 fecha) {
		DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);
		int edad = hoy.Year - fecha.Valor.Year;
		if (hoy < fecha.Valor.AddYears(edad)) edad--;
		return edad;
	}
}
