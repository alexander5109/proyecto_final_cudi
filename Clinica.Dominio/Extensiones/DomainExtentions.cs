using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica.Dominio.Extensiones;
public static class DomainExtentions {
	public static string ToNombreEspañol(this DayOfWeek dia) {
		return dia switch {
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

}
