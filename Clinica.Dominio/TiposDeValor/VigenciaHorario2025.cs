using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;



public record VigenciaHorario2025(DateOnly Valor) : IComoTexto {
	public string ATexto() {
		return Valor.ToString("dd/MM/yyyy");
	}
	public static VigenciaHorario2025 Crear(DateOnly fecha) {
		return new VigenciaHorario2025(fecha);
	}

	public static Result<VigenciaHorario2025> CrearResult(DateTime? fecha) {
		if (fecha is not DateTime) {
			return new Result<VigenciaHorario2025>.Error("La fecha de vigencia no puede estar vacía.");
		}
		DateOnly dateOnly = DateOnly.FromDateTime(fecha.Value);
		return new Result<VigenciaHorario2025>.Ok(new VigenciaHorario2025(dateOnly));
	}

	//public static Result<VigenciaHorario2025> Crear(string? input) {
	//	if (string.IsNullOrWhiteSpace(input))
	//		return new Result<VigenciaHorario2025>.Error("La fecha de vigencia no puede estar vacía.");

	//	// Soportar varios formatos razonables
	//	string[] formatos = [
	//		"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
	//	];

	//	if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
	//		return CrearResult(dt);

	//	if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt2))
	//		return CrearResult(dt2);

	//	return new Result<VigenciaHorario2025>.Error("Formato de fecha inválido.");
	//}
}

