using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

public sealed record DiaSemana2025(
	DayOfWeek Valor
) : IComoTexto {
	public string ATexto() {
		return Valor.ATexto();
	}

	public static readonly DiaSemana2025 Lunes = new(DayOfWeek.Monday);
	public static readonly DiaSemana2025 Martes = new(DayOfWeek.Tuesday);
	public static readonly DiaSemana2025 Miercoles = new(DayOfWeek.Wednesday);
	public static readonly DiaSemana2025 Jueves = new(DayOfWeek.Thursday);
	public static readonly DiaSemana2025 Viernes = new(DayOfWeek.Friday);
	public static readonly DiaSemana2025 Sabado = new(DayOfWeek.Saturday);
	public static readonly DiaSemana2025 Domingo = new(DayOfWeek.Sunday);

	public static readonly IReadOnlyList<DiaSemana2025> Todos = [Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo];

	public static Result<DiaSemana2025> Crear(DayOfWeek input) {
		return new Result<DiaSemana2025>.Ok(new(input));
	}

	public static Result<DiaSemana2025> Crear(string input) {
        Enum.TryParse(input, out DayOfWeek dia);
		return new Result<DiaSemana2025>.Ok(new(dia));
	}
}
