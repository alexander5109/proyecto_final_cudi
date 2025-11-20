using System.Runtime.CompilerServices;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;

public sealed record ListaDisponibilidades2025(
	IReadOnlyList<DisponibilidadEspecialidad2025> Valores
) : IComoTexto {
	public string ATexto() =>
		"Lista de disponibilidades:\n" +
		string.Join("\n", Valores.Select(d => "- " + d.ATexto()));



	// --- Tu generador original ---


	private static bool NoColisionaConTurnos(
		DisponibilidadEspecialidad2025 disp,
		IEnumerable<Turno2025> turnos) {
		foreach (var turno in turnos) {
			if (turno.MedicoAsignado != disp.Medico) continue;
			if (turno.Especialidad != disp.Especialidad) continue;

			bool solapa =
				turno.FechaHoraDesde < disp.FechaHoraHasta &&
				disp.FechaHoraDesde < turno.FechaHoraHasta;

			if (solapa)
				return false;
		}

		return true;
	}

	public static IEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
		SolicitudDeTurnoBasica solicitud,
		IEnumerable<Medico2025> medicos,
		IEnumerable<Turno2025> turnosActuales
	) {
		foreach (var medico in medicos)
			foreach (var especialidad in medico.Especialidades.Valores) {
				if (especialidad != solicitud.Especialidad)
					continue;

				int duracion = especialidad.DuracionConsultaMinutos;

				foreach (var franja in medico.ListaHorarios.Valores) {
					DateTime fecha = solicitud.Fecha.Date;

					// Ajustar al próximo día válido
					while (fecha.DayOfWeek != franja.DiaSemana.Valor)
						fecha = fecha.AddDays(1);

					for (int semana = 0; semana < 30; semana++, fecha = fecha.AddDays(7)) {
						DateTime desde = fecha + franja.Desde.Valor.ToTimeSpan();
						DateTime hasta = fecha + franja.Hasta.Valor.ToTimeSpan();

						// evitar slots pasados
						if (desde < DateTime.Now)
							continue;

						for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
							var disp = new DisponibilidadEspecialidad2025(
								especialidad,
								medico,
								slot,
								slot.AddMinutes(duracion)
							);

							// chequeo de colisión inline
							if (!NoColisionaConTurnos(disp, turnosActuales))
								continue;

							yield return disp;
						}
					}
				}
			}
	}



	public static Result<ListaDisponibilidades2025> Buscar(
		SolicitudDeTurnoBasica solicitud,
		IEnumerable<Medico2025> medicos,
		IEnumerable<Turno2025> turnos,
		int cuantos
	) {
        List<DisponibilidadEspecialidad2025> disponibles = GenerarDisponibilidades(solicitud, medicos, turnos)
			.OrderBy(d => d.FechaHoraDesde)
			.Take(cuantos)
			.ToList();

		if (disponibles.Count == 0)
			return new Result<ListaDisponibilidades2025>.Error(
				"No hay disponibilidades para la fecha solicitada o posterior."
			);

		return new Result<ListaDisponibilidades2025>.Ok(new(disponibles));
	}

	public Result<ListaDisponibilidades2025> AplicarFiltrosOpcionales(
		SolicitudDeTurnoPreferencias preferencias
	) {
		var filtradas = this.Valores.AsEnumerable();

		if (preferencias.DiaPreferido is DiaSemana2025 dia)
			filtradas = filtradas.Where(d => d.FechaHoraDesde.DayOfWeek == dia.Valor);

		if (preferencias.MomentoPreferido is TardeOMañana momento)
			filtradas = filtradas.Where(d => momento.AplicaA(d.FechaHoraDesde));

		return filtradas.ToListaDisponibilidades2025();
	}





}



public static class DisponibilidadesExtensions {
	public static Result<ListaDisponibilidades2025> ToListaDisponibilidades2025(
		this IEnumerable<DisponibilidadEspecialidad2025> secuencia) {
		var lista = secuencia.ToList();

		if (lista.Count == 0)
			return new Result<ListaDisponibilidades2025>.Error("No se encontraron disponibilidades.");

		return new Result<ListaDisponibilidades2025>.Ok(
			new ListaDisponibilidades2025(lista)
		);
	}
}
