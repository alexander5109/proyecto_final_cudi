using System.Globalization;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct FechaIngreso2025(DateTime Valor): IComoTexto {
	public static readonly DateTime Ahora = DateTime.Now;
	public string ATexto() {
		return Valor.ToString("dd/MM/yyyy");
	}

	// ==============================
	// -------- Factories ----------
	// ==============================

	public static Result<FechaIngreso2025> Crear(DateTime fecha) {
		// VALIDACIONES DE DOMINIO
		if (fecha > Ahora)
			return new Result<FechaIngreso2025>.Error("La fecha de ingreso no puede ser futura.");

		if (fecha < Ahora.AddYears(-30))
			return new Result<FechaIngreso2025>.Error("Hace 30 años no existía la clínica.");

		return new Result<FechaIngreso2025>.Ok(new FechaIngreso2025(fecha));
	}

	public static Result<FechaIngreso2025> Crear(DateTime? fecha) {
		if (fecha is null)
			return new Result<FechaIngreso2025>.Error("La fecha de ingreso no puede estar vacía.");

		return Crear(fecha.Value);
	}

	// Compatibilidad: DateOnly → DateTime
	public static Result<FechaIngreso2025> Crear(DateOnly fecha)
		=> Crear(fecha.ToDateTime(TimeOnly.MinValue));

	public static Result<FechaIngreso2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaIngreso2025>.Error("La fecha de ingreso no puede estar vacía.");

		input = input.Trim();

		// Formatos razonables de fecha y fecha+hora
		string[] formatos =
		{
			"dd/MM/yyyy",
			"dd/MM/yyyy HH:mm",
			"dd/MM/yyyy HH:mm:ss",

			"yyyy-MM-dd",
			"yyyy-MM-dd HH:mm",
			"yyyy-MM-dd HH:mm:ss",
			"yyyy-MM-ddTHH:mm",
			"yyyy-MM-ddTHH:mm:ss",

			"d/M/yyyy",
			"d/M/yyyy HH:mm",
			"M/d/yyyy",
			"M/d/yyyy HH:mm"
		};

		// Intento 1: parseo exacto
		if (DateTime.TryParseExact(
				input,
				formatos,
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out var dtExact)) {
			return Crear(dtExact);
		}

		// Intento 2: parseo flexible según cultura del usuario
		if (DateTime.TryParse(
				input,
				CultureInfo.CurrentCulture,
				DateTimeStyles.AssumeLocal,
				out var dtCulture)) {
			return Crear(dtCulture);
		}

		return new Result<FechaIngreso2025>.Error("Formato de fecha inválido.");
	}
}
