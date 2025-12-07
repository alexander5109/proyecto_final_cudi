using System.Globalization;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

public record FechaRegistro2025(DateTime Valor) : IComoTexto {
	public static readonly DateTime Ahora = DateTime.Now;
	public string ATexto() {
		return Valor.ToString("dd/MM/yyyy");
	}
	public static Result<FechaRegistro2025> CrearResult(DateTime? fecha) {
		if (fecha is DateTime fechagud){
			return new Result<FechaRegistro2025>.Ok(new FechaRegistro2025(fechagud));
		}
		return new Result<FechaRegistro2025>.Error("La fecha de registro en el sistema no puede estar vacía.");

	}
	public static Result<FechaRegistro2025> CrearResult(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaRegistro2025>.Error("La fecha de registro en el sistema no puede estar vacía.");

		input = input.Trim();

		// Formatos razonables de fecha y fecha+hora
		string[] formatos = [
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
		];

		// Intento 1: parseo exacto
		if (DateTime.TryParseExact(
				input,
				formatos,
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out DateTime dtExact)) {
			return CrearResult(dtExact);
		}

		// Intento 2: parseo flexible según cultura del usuario
		if (DateTime.TryParse(
				input,
				CultureInfo.CurrentCulture,
				DateTimeStyles.AssumeLocal,
				out DateTime dtCulture)) {
			return CrearResult(dtCulture);
		}

		return new Result<FechaRegistro2025>.Error("Formato de fecha inválido.");
	}
}
