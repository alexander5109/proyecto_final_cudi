using static Clinica.Dominio.Entidades.Entidades;

namespace Clinica.Dominio;

public class Servicios {

	public static IEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
		IEnumerable<Medico2025> medicos,
		DateTime fechaMinima) {
		foreach (var medico in medicos) {
			foreach (var especialidad in medico.Especialidades.Valores) {
				int duracion = especialidad.DuracionConsultaMinutos;

				foreach (var franja in medico.ListaHorarios.Valores) {
					// Punto de partida
					DateTime fecha = fechaMinima.Date;

					// Avanzamos hasta coincidir con el día de semana de la franja
					while (fecha.DayOfWeek != franja.DiaSemana.Valor)
						fecha = fecha.AddDays(1);

					// Producimos slots durante las próximas N semanas
					for (int semana = 0; semana < 30; semana++, fecha = fecha.AddDays(7)) {
						DateTime desde = fecha + franja.Desde.Valor.ToTimeSpan();
						DateTime hasta = fecha + franja.Hasta.Valor.ToTimeSpan();

						for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
							yield return new DisponibilidadEspecialidad2025(
								especialidad,
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



}
