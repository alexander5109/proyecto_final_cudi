using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

public sealed record DiaSemana2025(
	DayOfWeek Valor,
	string NombreEspañol
) : IComoTexto {
	public string ATexto() {
		return Valor.AEspañol();
	}

	public static readonly DiaSemana2025 Lunes = new(DayOfWeek.Monday, DayOfWeek.Monday.AEspañol());
	public static readonly DiaSemana2025 Martes = new(DayOfWeek.Tuesday, DayOfWeek.Tuesday.AEspañol());
	public static readonly DiaSemana2025 Miercoles = new(DayOfWeek.Wednesday, DayOfWeek.Wednesday.AEspañol());
	public static readonly DiaSemana2025 Jueves = new(DayOfWeek.Thursday, DayOfWeek.Thursday.AEspañol());
	public static readonly DiaSemana2025 Viernes = new(DayOfWeek.Friday, DayOfWeek.Friday.AEspañol());
	public static readonly DiaSemana2025 Sabado = new(DayOfWeek.Saturday, DayOfWeek.Saturday.AEspañol());
	public static readonly DiaSemana2025 Domingo = new(DayOfWeek.Sunday, DayOfWeek.Sunday.AEspañol());

	public static readonly IReadOnlyList<DiaSemana2025> Todos = [Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo];

	public static DiaSemana2025 Crear(DayOfWeek input) {
		return new(input, input.AEspañol());
	}

	//public static Result<DiaSemana2025> CrearResult(string input) {
	//	if (Enum.TryParse(input, out DayOfWeek dia)) {
	//		return new Result<DiaSemana2025>.Ok(new(dia, dia.AEspañol()));
	//	} else {
	//		return new Result<DiaSemana2025>.Error("No es un dia válido");
	//	}
	//}
}
public static class DayOfWeekExtensiones {
	public static string AEspañol(this DayOfWeek dia) => dia switch {
		DayOfWeek.Monday => "Lunes",
		DayOfWeek.Tuesday => "Martes",
		DayOfWeek.Wednesday => "Miércoles",
		DayOfWeek.Thursday => "Jueves",
		DayOfWeek.Friday => "Viernes",
		DayOfWeek.Saturday => "Sábado",
		DayOfWeek.Sunday => "Domingo",
		_ => throw new ArgumentOutOfRangeException(nameof(dia), dia, null)
	};
}
