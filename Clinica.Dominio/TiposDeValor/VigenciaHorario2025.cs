using System.Globalization;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;



public record VigenciaHorario2025(DateOnly Valor) : IComoTexto {
	public string ATexto() {
		return Valor.ToString("dd/MM/yyyy");
	}
	public static Result<VigenciaHorario2025> Crear(DateOnly fecha) {
		return new Result<VigenciaHorario2025>.Ok(new(fecha));
	}

	public static Result<VigenciaHorario2025> Crear(DateTime? fecha) {
		if (fecha is null) {
			return new Result<VigenciaHorario2025>.Error("La fecha de vigencia no puede estar vacía.");
		}
		DateOnly dateOnly = DateOnly.FromDateTime(fecha.Value);
		return Crear(dateOnly);
	}

	public static Result<VigenciaHorario2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<VigenciaHorario2025>.Error("La fecha de vigencia no puede estar vacía.");

		// Soportar varios formatos razonables
		string[] formatos = [
			"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
		];

		if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
			return Crear(dt);

		if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt2))
			return Crear(dt2);

		return new Result<VigenciaHorario2025>.Error("Formato de fecha inválido.");
	}
}

