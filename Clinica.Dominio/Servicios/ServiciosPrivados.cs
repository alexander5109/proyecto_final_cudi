using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.Dtos.DomainDtos;

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

	internal static async Task<Result<DisponibilidadEspecialidad2025>> EncontrarProximaDisponibilidad(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		IBaseDeDatosRepositorio repositorio
	) {
		await foreach (var disp in GenerarDisponibilidades(
			solicitudEspecialidad,
			solicitudFechaCreacion,
			repositorio
		)) {
			// First element wins → return immediately
			return new Result<DisponibilidadEspecialidad2025>.Ok(disp);
		}

		return new Result<DisponibilidadEspecialidad2025>.Error(
			"No se encontró ninguna disponibilidad."
		);
	}

	public static async Task<DateTime?> CalcularPrimerSlotDisponible(
		MedicoId medicoId,
		EspecialidadMedica2025 especialidad,
		IBaseDeDatosRepositorio repositorio
	) {
		int duracion = especialidad.DuracionConsultaMinutos;

		DateTime desdeBusqueda = DateTime.Now.Date;
		DateTime hastaBusqueda = desdeBusqueda.AddDays(7 * 30); // 30 semanas

		// 1. Cargar turnos del médico en el rango
		var turnos = (await repositorio.SelectTurnosProgramadosBetweenFechasWhereMedicoId(
			medicoId, desdeBusqueda, hastaBusqueda)).ToList();

		// 2. Cargar sus horarios vigentes
		var franjas = (await repositorio.SelectHorariosVigentesBetweenFechasWhereMedicoId(
			medicoId, desdeBusqueda, hastaBusqueda)).ToList();

		foreach (var franja in franjas) {
			// Ubicar primer día coincidente con el DíaSemana de la franja
			DateTime fecha = desdeBusqueda;
			while (fecha.DayOfWeek != franja.DiaSemana)
				fecha = fecha.AddDays(1);

			// 3. Iterar por semanas
			for (int semana = 0; semana < 30; semana++, fecha = fecha.AddDays(7)) {
				DateTime desde = fecha + franja.HoraDesde;
				DateTime hasta = fecha + franja.HoraHasta;

				if (hasta <= DateTime.Now)
					continue;

				// 4. Iterar slots dentro de la franja
				for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
					DateTime slotHasta = slot.AddMinutes(duracion);

					if (slotHasta <= DateTime.Now)
						continue;

					bool solapa = false;

					foreach (var t in turnos) {
						if (t.EspecialidadCodigo == especialidad.CodigoInternoValor &&
							t.OutcomeEstado == TurnoOutcomeEstado2025.Programado.Codigo &&
							t.FechaHoraAsignadaDesde < slotHasta &&
							slot < t.FechaHoraAsignadaHasta) {
							solapa = true;
							break;
						}
					}

					if (!solapa)
						return slot;
				}
			}
		}

		return null; // no hay turnos disponibles
	}

	internal static async IAsyncEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		IBaseDeDatosRepositorio repositorio
	) {
		var medicos = await repositorio.SelectMedicosWhereEspecialidad(solicitudEspecialidad);

		var medicosConPrioridad = new List<(MedicoDto medico, DateTime? firstSlot)>();

		foreach (var medico in medicos) {
			DateTime? first = await CalcularPrimerSlotDisponible(
				new MedicoId(medico.Id.Valor),
				solicitudEspecialidad,
				repositorio);

			medicosConPrioridad.Add((medico, first));
		}

		// Ordenar nulls últimos
		foreach (var x in medicosConPrioridad
			.OrderBy(x => x.firstSlot ?? DateTime.MaxValue)) {
			if (x.firstSlot is null)
				continue;

			// Generar slots para este médico
			await foreach (var disp in GenerarDisponibilidadesDeMedico(
				solicitudEspecialidad, solicitudFechaCreacion, x.medico, repositorio)) {
				yield return disp;
			}
		}
	}



	private static async IAsyncEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidadesDeMedico(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		MedicoDto medico,
		IBaseDeDatosRepositorio repositorio
	) {
		int duracion = solicitudEspecialidad.DuracionConsultaMinutos;
		int semanas = 30;

		DateTime desdeBusqueda = solicitudFechaCreacion.Date;
		DateTime hastaBusqueda = solicitudFechaCreacion.Date.AddDays(7 * semanas);

		var turnos = (await repositorio.SelectTurnosProgramadosBetweenFechasWhereMedicoId(
			new MedicoId(medico.Id.Valor), desdeBusqueda, hastaBusqueda)).ToList();

		var franjas = (await repositorio.SelectHorariosVigentesBetweenFechasWhereMedicoId(
			new MedicoId(medico.Id.Valor), desdeBusqueda, hastaBusqueda)).ToList();

		foreach (var franja in franjas) {
			DateTime fecha = desdeBusqueda;
			while (fecha.DayOfWeek != franja.DiaSemana)
				fecha = fecha.AddDays(1);

			for (int semana = 0; semana < semanas; semana++, fecha = fecha.AddDays(7)) {
				DateTime desde = fecha + franja.HoraDesde;
				DateTime hasta = fecha + franja.HoraHasta;

				if (desde < DateTime.Now)
					continue;

				for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
					var disp = new DisponibilidadEspecialidad2025(
						solicitudEspecialidad, medico.Id,
						slot, slot.AddMinutes(duracion));

					bool solapa = false;
					foreach (var t in turnos) {
						if (t.EspecialidadCodigo == disp.Especialidad.CodigoInternoValor &&
							t.OutcomeEstado == TurnoOutcomeEstado2025.Programado.Codigo &&
							t.FechaHoraAsignadaDesde < disp.FechaHoraHasta &&
							disp.FechaHoraDesde < t.FechaHoraAsignadaHasta) {
							solapa = true;
							break;
						}
					}

					if (!solapa)
						yield return disp;
				}
			}
		}
	}



}
