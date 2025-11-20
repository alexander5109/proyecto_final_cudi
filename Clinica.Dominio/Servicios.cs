using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinica.Dominio.Entidades;
using static Clinica.Dominio.Entidades.Entidades;

namespace Clinica.Dominio;

public class Servicios {


	public static IEnumerable<(DateTime inicio, DateTime fin)> GenerarSlots(
		DateTime fechaBase,
		DayOfWeek diaSemana,
		TimeOnly desde,
		TimeOnly hasta,
		int duracionMin
	) {
		// Avanzar fechaBase hasta el próximo día con ese DayOfWeek
		while (fechaBase.DayOfWeek != diaSemana)
			fechaBase = fechaBase.AddDays(1);

		var inicioDia = fechaBase.Date.Add(desde.ToTimeSpan());
		var finDia = fechaBase.Date.Add(hasta.ToTimeSpan());

		for (var t = inicioDia; t.AddMinutes(duracionMin) <= finDia; t = t.AddMinutes(duracionMin))
			yield return (t, t.AddMinutes(duracionMin));
	}





	public static IEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
	IEnumerable<Medico2025> medicos,
	DateTime fechaMinima) {
		foreach (var medico in medicos) {
			int duracion = medico.Especialidad.DuracionConsultaMinutos;

			foreach (var franja in medico.ListaHorarios.Valores) {
				// Encontramos la próxima fecha válida a partir de 'fechaMinima'
				DateTime fecha = fechaMinima.Date;

				// Movemos fecha hacia adelante hasta coincidir con el día de semana del médico
				while (fecha.DayOfWeek != franja.DiaSemana.Valor)
					fecha = fecha.AddDays(1);

				// Generamos slots para los próximos X días (ej: 30)
				for (int dias = 0; dias < 30; dias++, fecha = fecha.AddDays(7)) {
					DateTime desde = fecha + franja.Desde.Valor.ToTimeSpan();
					DateTime hasta = fecha + franja.Hasta.Valor.ToTimeSpan();

					for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
						yield return new DisponibilidadEspecialidad2025(
							medico,
							slot,
							slot.AddMinutes(duracion)
						);
					}
				}
			}
		}
	}


}
