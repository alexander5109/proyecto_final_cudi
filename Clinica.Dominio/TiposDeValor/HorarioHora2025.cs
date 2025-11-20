using System.Globalization;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct HorarioHora2025(
	TimeOnly Valor
): IComoTexto {
	public string ATexto() => Valor.ToString("HH:mm");
	public static Result<HorarioHora2025> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<HorarioHora2025>.Error("La hora no puede estar vacía.");

		if (TimeOnly.TryParseExact(
				input.Trim(),
				new[] { "HH:mm", "H:mm" },
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out var time))
			return new Result<HorarioHora2025>.Ok(new HorarioHora2025(time));

		return new Result<HorarioHora2025>.Error($"'{input}' no es una hora válida.");
	}

	public static Result<HorarioHora2025> Crear(TimeOnly value)
		=> new Result<HorarioHora2025>.Ok(new HorarioHora2025(value));

}
