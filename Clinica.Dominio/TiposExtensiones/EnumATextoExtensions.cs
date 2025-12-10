using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;

namespace Clinica.Dominio.TiposExtensiones;

public static class EnumATextoExtensions {
	//public static string ATexto(this TimeSpan hora) => hora.ToString(@"hh\:mm");
	public static string ATextoHoras(this TimeSpan hora) => hora.ToString("HH:mm");
	public static string ATextoHoras(this TimeOnly hora) => hora.ToString("HH:mm");
	public static string ATexto(this DateOnly fecha) => fecha.ToString("yyyy/dd/MM");
	public static string ATextoDia(this DateTime fecha) => fecha.ToString("yyyy/dd/MM");
	public static string ATextoHoras(this DateTime fechaHora) => fechaHora.ToString("HH:mm");
	public static string ATexto(this DateTime fechaHora) => fechaHora.ToString("yyyy/dd/MM HH:mm");

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


	public static string ATexto(this ProvinciaCodigo codigo) {
		Result<ProvinciaArgentina2025> resultado = ProvinciaArgentina2025.CrearResultPorCodigo(codigo);
		if (resultado is Result<ProvinciaArgentina2025>.Ok ok)
			return ok.Valor.NombreValor;
		return "Código de provincia inválido";
	}

}
