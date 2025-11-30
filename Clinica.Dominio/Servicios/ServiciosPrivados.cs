using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Servicios;

internal static class ServiciosPrivados {
	internal static Result<DisponibilidadEspecialidad2025> TomarPrimera(this Result<IReadOnlyList<DisponibilidadEspecialidad2025>> listadoResult) {
		return listadoResult.Match<Result<DisponibilidadEspecialidad2025>>(
			ok => {
				// la lista existe, ahora chequeamos si tiene elementos
				if (ok.Count == 0)
					return new Result<DisponibilidadEspecialidad2025>.Error(
						"No hay disponibilidades para seleccionar."
					);
				return new Result<DisponibilidadEspecialidad2025>.Ok(ok[0]);
			},
			mensajeError =>
				new Result<DisponibilidadEspecialidad2025>.Error(mensajeError)
		);
	}

	internal static Result<IReadOnlyList<DisponibilidadEspecialidad2025>> AplicarFiltrosOpcionales(this Result<IReadOnlyList<DisponibilidadEspecialidad2025>> disponibilidadesResult, SolicitudDeTurnoPreferencias preferencias) {
		if (disponibilidadesResult is Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error err)
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error(err.Mensaje);

		IReadOnlyList<DisponibilidadEspecialidad2025> lista = ((Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok)disponibilidadesResult).Valor;
		IEnumerable<DisponibilidadEspecialidad2025> filtradas = lista;
		if (preferencias.DiaPreferido is DiaSemana2025 dia)
			filtradas = filtradas.Where(d => d.FechaHoraDesde.DayOfWeek == dia.Valor);

		if (preferencias.MomentoPreferido is TardeOMañana momento)
			filtradas = filtradas.Where(d => momento.AplicaA(d.FechaHoraDesde));

		if (filtradas.Any()) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok([.. filtradas]);
		} else {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error("No se encontraron disponibilidades");
		}
	}

	internal static Result<DisponibilidadEspecialidad2025> EncontrarProximaDisponibilidad(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, DateTime, DateTime, IEnumerable<HorarioMedico2025>> funcSelectHorariosVigentesBetweenFechasWhereMedicoId,
		Func<MedicoId, DateTime, DateTime, IEnumerable<Turno2025>> funcSelectTurnosProgramadosBetweenFechasWhereMedicoId
	) {
		DisponibilidadEspecialidad2025? proxima = GenerarDisponibilidades(
			solicitudEspecialidad,
			solicitudFechaCreacion,
			funcSelectMedicosWhereEspecialidad,
			funcSelectHorariosVigentesBetweenFechasWhereMedicoId,
			funcSelectTurnosProgramadosBetweenFechasWhereMedicoId
		).FirstOrDefault();
		if (proxima is null)
			return new Result<DisponibilidadEspecialidad2025>.Error("No se encontraron proximaDisponibilidad");
		return new Result<DisponibilidadEspecialidad2025>.Ok((DisponibilidadEspecialidad2025)proxima);
	}

	internal static IEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, DateTime, DateTime, IEnumerable<HorarioMedico2025>> funcSelectHorariosVigentesBetweenFechasWhereMedicoId,
		Func<MedicoId, DateTime, DateTime, IEnumerable<Turno2025>> funcSelectTurnosProgramadosBetweenFechasWhereMedicoId
	) {
		foreach (Medico2025 medico in funcSelectMedicosWhereEspecialidad(solicitudEspecialidad)) {

			int duracion = solicitudEspecialidad.DuracionConsultaMinutos;
			int semanas = 30;
			DateTime desdeBusqueda = solicitudFechaCreacion.Date;
			DateTime hastaBusqueda = solicitudFechaCreacion.Date.AddDays(7 * semanas);

			List<Turno2025> turnos = [
				.. funcSelectTurnosProgramadosBetweenFechasWhereMedicoId(
				medico.Id, desdeBusqueda, hastaBusqueda)
			];

			foreach (HorarioMedico2025 franja in funcSelectHorariosVigentesBetweenFechasWhereMedicoId(medico.Id, desdeBusqueda, hastaBusqueda)) {

				DateTime fecha = desdeBusqueda;
				while (fecha.DayOfWeek != franja.DiaSemana.Valor)
					fecha = fecha.AddDays(1);

				for (int semana = 0; semana < semanas; semana++, fecha = fecha.AddDays(7)) {

					DateTime desde = fecha + franja.Desde.Valor.ToTimeSpan();
					DateTime hasta = fecha + franja.Hasta.Valor.ToTimeSpan();

					if (desde < DateTime.Now)
						continue;

					for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {

						DisponibilidadEspecialidad2025 disp = new(
							solicitudEspecialidad, medico.Id,
							slot, slot.AddMinutes(duracion));

						bool solapa = turnos.Any(t =>
							t.Especialidad == disp.Especialidad &&
							t.OutcomeEstadoOption == TurnoOutcomeEstado2025.Programado &&
							t.FechaHoraAsignadaDesdeValor < disp.FechaHoraHasta &&
							disp.FechaHoraDesde < t.FechaHoraAsignadaHastaValor
						);

						if (!solapa)
							yield return disp;
					}
				}
			}
		}
	}
}
