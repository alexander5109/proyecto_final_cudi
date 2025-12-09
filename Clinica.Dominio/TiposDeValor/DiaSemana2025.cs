using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;
/*
public sealed record DiaSemana2025(
	DayOfWeek EnumValor,
	string DiaNombre
) : IComoTexto {
	public string ATexto() {
		return EnumValor.ATexto();
	}

	public static readonly DiaSemana2025 Lunes = new(DayOfWeek.Monday, DayOfWeek.Monday.ATexto());
	public static readonly DiaSemana2025 Martes = new(DayOfWeek.Tuesday, DayOfWeek.Tuesday.ATexto());
	public static readonly DiaSemana2025 Miercoles = new(DayOfWeek.Wednesday, DayOfWeek.Wednesday.ATexto());
	public static readonly DiaSemana2025 Jueves = new(DayOfWeek.Thursday, DayOfWeek.Thursday.ATexto());
	public static readonly DiaSemana2025 Viernes = new(DayOfWeek.Friday, DayOfWeek.Friday.ATexto());
	public static readonly DiaSemana2025 Sabado = new(DayOfWeek.Saturday, DayOfWeek.Saturday.ATexto());
	public static readonly DiaSemana2025 Domingo = new(DayOfWeek.Sunday, DayOfWeek.Sunday.ATexto());

	public static readonly IReadOnlyList<DiaSemana2025> Todos = [Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo];

	public static DiaSemana2025 Crear(DayOfWeek input) {
		return new(input, input.ATexto());
	}

	//public static Result<DiaSemana2025> CrearResult(string input) {
	//	if (Enum.TryParse(input, out DayOfWeek dia)) {
	//		return new Result<DiaSemana2025>.Ok(new(dia, dia.ATexto()));
	//	} else {
	//		return new Result<DiaSemana2025>.Error("No es un dia válido");
	//	}
	//}
}

*/
public static class DayOfWeekExtensiones {


	public static string ATexto(this DayOfWeek dia) => dia switch {
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
