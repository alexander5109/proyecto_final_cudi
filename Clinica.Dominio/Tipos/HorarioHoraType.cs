using Clinica.Dominio.Comun;
using System.Globalization;
namespace Clinica.Dominio.Tipos;

public readonly record struct HorarioHoraType(
	TimeOnly Valor
);
public static class HorarioHora2025 {
	public static Result<HorarioHoraType> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<HorarioHoraType>.Error("La hora no puede estar vacía.");

		if (TimeOnly.TryParseExact(
				input.Trim(),
				new[] { "HH:mm", "H:mm" },
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out var time))
			return new Result<HorarioHoraType>.Ok(new HorarioHoraType(time));

		return new Result<HorarioHoraType>.Error($"'{input}' no es una hora válida.");
	}

	public static Result<HorarioHoraType> Crear(TimeOnly value)
		=> new Result<HorarioHoraType>.Ok(new HorarioHoraType(value));

	public static string AString(this HorarioHoraType hora)
		=> hora.Valor.ToString("HH:mm");
}