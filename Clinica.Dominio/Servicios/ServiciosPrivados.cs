using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.IRepositorios.QueryModels;

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
		IRepositorioDomain repositorio
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


	internal static async IAsyncEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		IRepositorioDomain repositorio
	) {
        Result<IEnumerable<MedicoQM>> medicosResult = (await repositorio.SelectMedicosWhereEspecialidadCode(solicitudEspecialidad.CodigoInternoValor));

		if (medicosResult.IsError) {
			yield break;
		}
       var medicos = medicosResult.UnwrapAsOk();

		List<(MedicoQM medico, DateTime firstSlot)> medicosConPrioridad = new();

		foreach (MedicoQM medico in medicos) {
			Result<DateTime> first = await CalcularPrimerSlotDisponible(
				medico.Id,
				solicitudEspecialidad,
				repositorio);
			if (first.IsOk) { 
				medicosConPrioridad.Add((medico, first.UnwrapAsOk()));
			}
		}

		// Ordenar nulls últimos
		foreach ((MedicoQM medico, DateTime firstSlot) x in medicosConPrioridad
			.OrderBy(x => x.firstSlot)) {

			// Generar slots para este médico
			await foreach (DisponibilidadEspecialidad2025 disp in GenerarDisponibilidadesDeMedico(
				solicitudEspecialidad, solicitudFechaCreacion, x.medico, repositorio)) {
				yield return disp;
			}
		}

	}



	private static async IAsyncEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidadesDeMedico(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		MedicoQM medico,
		IRepositorioDomain repositorio
	) {
		int duracion = solicitudEspecialidad.DuracionConsultaMinutos;
		int semanas = 30;

		DateTime desdeBusqueda = solicitudFechaCreacion.Date;
		DateTime hastaBusqueda = solicitudFechaCreacion.Date.AddDays(7 * semanas);

		var turnosResult = (await repositorio.SelectTurnosProgramadosBetweenFechasWhereMedicoId(
			new MedicoId(medico.Id.Valor), desdeBusqueda, hastaBusqueda));

		if (turnosResult.IsError) {
			yield break;
		}
		var turnos = turnosResult.UnwrapAsOk();


		var franjasResult = (await repositorio.SelectHorariosVigentesBetweenFechasWhereMedicoId(
			new MedicoId(medico.Id.Valor), desdeBusqueda, hastaBusqueda));

		if (franjasResult.IsError) {
			yield break;
		}
		var franjas = franjasResult.UnwrapAsOk();


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

	public static async Task<Result<DateTime>> CalcularPrimerSlotDisponible(
		MedicoId medicoId,
		EspecialidadMedica2025 especialidad,
		IRepositorioDomain repositorio
	) {
		int duracion = especialidad.DuracionConsultaMinutos;

		DateTime desdeBusqueda = DateTime.Now.Date;
		DateTime hastaBusqueda = desdeBusqueda.AddDays(7 * 30); // 30 semanas

		// 1. Cargar turnos del médico en el rango
		var turnosResult = (await repositorio.SelectTurnosProgramadosBetweenFechasWhereMedicoId(medicoId, desdeBusqueda, hastaBusqueda));
		if (turnosResult.IsError) {
			return new Result<DateTime>.Error($"Error de la base de datos. No se pudo acceder a los turnos programados del {medicoId}: Traceback: {turnosResult.UnwrapAsError()}");
		}
		var turnos = turnosResult.UnwrapAsOk();

		// 2. Cargar sus horarios vigentes
		var franjasResult = (await repositorio.SelectHorariosVigentesBetweenFechasWhereMedicoId(
			medicoId, desdeBusqueda, hastaBusqueda));
		if (franjasResult.IsError) {
			return new Result<DateTime>.Error($"Error de la base de datos. No se pudo acceder a los horarios del {medicoId}: Traceback: {turnosResult.UnwrapAsError()}");
		}
		var franjas = franjasResult.UnwrapAsOk();

		foreach (HorarioMedicoQM franja in franjas) {
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

					foreach (TurnoQM t in turnos) {
						if (t.EspecialidadCodigo == especialidad.CodigoInternoValor &&
							t.OutcomeEstado == TurnoOutcomeEstado2025.Programado.Codigo &&
							t.FechaHoraAsignadaDesde < slotHasta &&
							slot < t.FechaHoraAsignadaHasta) {
							solapa = true;
							break;
						}
					}

					if (!solapa)
						return new Result<DateTime>.Ok(slot);
				}
			}
		}
		return new Result<DateTime>.Error("No hay disponibilidad para este medico"); // no hay turnos disponibles
	}


}
