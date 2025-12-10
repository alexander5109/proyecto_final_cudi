namespace Clinica.Dominio._Disabled;
/*
public sealed record DiaSemana2025(
	DayOfWeek EnumValor,
	string DiaNombre
) : IComoTexto {
	public string ATextoDia() {
		return EnumValor.ATextoDia();
	}

	public static readonly DiaSemana2025 Lunes = new(DayOfWeek.Monday, DayOfWeek.Monday.ATextoDia());
	public static readonly DiaSemana2025 Martes = new(DayOfWeek.Tuesday, DayOfWeek.Tuesday.ATextoDia());
	public static readonly DiaSemana2025 Miercoles = new(DayOfWeek.Wednesday, DayOfWeek.Wednesday.ATextoDia());
	public static readonly DiaSemana2025 Jueves = new(DayOfWeek.Thursday, DayOfWeek.Thursday.ATextoDia());
	public static readonly DiaSemana2025 Viernes = new(DayOfWeek.Friday, DayOfWeek.Friday.ATextoDia());
	public static readonly DiaSemana2025 Sabado = new(DayOfWeek.Saturday, DayOfWeek.Saturday.ATextoDia());
	public static readonly DiaSemana2025 Domingo = new(DayOfWeek.Sunday, DayOfWeek.Sunday.ATextoDia());

	public static readonly IReadOnlyList<DiaSemana2025> Todos = [Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo];

	public static DiaSemana2025 Crear(DayOfWeek input) {
		return new(input, input.ATextoDia());
	}

	//public static Result<DiaSemana2025> CrearResult(string input) {
	//	if (Enum.TryParse(input, out DayOfWeek dia)) {
	//		return new Result<DiaSemana2025>.Ok(new(dia, dia.ATextoDia()));
	//	} else {
	//		return new Result<DiaSemana2025>.Error("No es un dia válido");
	//	}
	//}
}

*/