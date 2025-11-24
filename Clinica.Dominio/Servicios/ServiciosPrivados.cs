using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Servicios;

internal static class ServiciosPrivados {
	internal static Result<DisponibilidadEspecialidad2025> EncontrarProximaDisponibilidad(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, DateTime, DateTime, IEnumerable<HorarioMedico2025>> funcSelectHorariosWhereMedicoIdInVigencia,
		Func<MedicoId, DateTime, DateTime, IEnumerable<Turno2025>> funcSelectTurnosWhereMedicoIdBetweenFechas
	) {
		DisponibilidadEspecialidad2025? proxima = GenerarDisponibilidades(
			solicitudEspecialidad,
			solicitudFechaCreacion,
			funcSelectMedicosWhereEspecialidad,
			funcSelectHorariosWhereMedicoIdInVigencia,
			funcSelectTurnosWhereMedicoIdBetweenFechas
		).FirstOrDefault();
		if (proxima is null)
			return new Result<DisponibilidadEspecialidad2025>.Error("No se encontraron proximaDisponibilidad");
		return new Result<DisponibilidadEspecialidad2025>.Ok((DisponibilidadEspecialidad2025)proxima);
	}


	internal static bool DisponibilidadNoColisiona(
		EspecialidadMedica2025 especialidad,
		DateTime fechaHoraDesde,
		DateTime fechaHoraHasta,
		List<Turno2025> turnos
	) {
		foreach (Turno2025 turno in turnos) {
			// Debe ser de la misma especialidad
			if (turno.Especialidad != especialidad)
				continue;

			// Solo bloquean los programados
			if (turno.OutcomeEstado != TurnoOutcomeEstado2025.Programado)
				continue;

			// Solapamiento clásico
			bool solapa =
				turno.FechaHoraAsignadaDesde < fechaHoraHasta &&
				fechaHoraDesde < turno.FechaHoraAsignadaHasta;

			if (solapa)
				return false;
		}

		return true;
	}
	internal static IEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, DateTime, DateTime, IEnumerable<HorarioMedico2025>> funcSelectHorariosWhereMedicoIdInVigencia,
		Func<MedicoId, DateTime, DateTime, IEnumerable<Turno2025>> funcSelectTurnosWhereMedicoIdBetweenFechas
	) {
		// buscar médicos para esa especialidad
		foreach (Medico2025 medico in funcSelectMedicosWhereEspecialidad(solicitudEspecialidad)) {
			int duracion = solicitudEspecialidad.DuracionConsultaMinutos;

			// Generamos un rango de 30 semanas desde la solicitud
			DateTime rangoDesde = solicitudFechaCreacion.Date;
			DateTime rangoHasta = solicitudFechaCreacion.Date.AddDays(7 * 30);

			// pedir horarios vigentes en ese rango
			IEnumerable<HorarioMedico2025> horarios = funcSelectHorariosWhereMedicoIdInVigencia(medico.Id, rangoDesde, rangoHasta);

			// traer turnos ya asignados dentro del rango
			List<Turno2025> turnos = [.. funcSelectTurnosWhereMedicoIdBetweenFechas(medico.Id, rangoDesde, rangoHasta)]; // se usa mucho

			foreach (HorarioMedico2025 franja in horarios) {
				DateTime fecha = solicitudFechaCreacion.Date;

				// ajustar al próximo día válido según la franja
				while (fecha.DayOfWeek != franja.DiaSemana.Valor)
					fecha = fecha.AddDays(1);

				for (int semana = 0; semana < 30; semana++, fecha = fecha.AddDays(7)) {
					DateTime desde = fecha + franja.Desde.Valor.ToTimeSpan();
					DateTime hasta = fecha + franja.Hasta.Valor.ToTimeSpan();

					if (desde < DateTime.Now)
						continue;

					for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
						DisponibilidadEspecialidad2025 disp = new(solicitudEspecialidad, medico.Id, slot, slot.AddMinutes(duracion));

						if (!DisponibilidadNoColisiona(
							disp.Especialidad,
							disp.FechaHoraDesde,
							disp.FechaHoraHasta,
							turnos))
							continue;

						yield return disp;
					}
				}
			}
		}
	}




}
