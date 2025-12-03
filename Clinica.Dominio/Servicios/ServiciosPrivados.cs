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
			repositorio)) {
			return disp;
		}
		return new Result<DisponibilidadEspecialidad2025>.Error("No se encontró ninguna disponibilidad.");
	}


	internal static async IAsyncEnumerable<Result<DisponibilidadEspecialidad2025>>GenerarDisponibilidades(
		EspecialidadMedica2025 especialidad,
		DateTime fechaCreacion,
		IRepositorioDomain repo
	) {
		var medicosResult = await repo.SelectMedicosIdWhereEspecialidadCode(especialidad.CodigoInternoValor);
		if (medicosResult is Result<IEnumerable<MedicoId>>.Error errMed) {
			yield return new Result<DisponibilidadEspecialidad2025>.Error(errMed.Mensaje);
			yield break;
		}
		var medicos = ((Result<IEnumerable<MedicoId>>.Ok)medicosResult).Valor;
		var ordenados = new List<(MedicoId medico, DateTime slot)>();
		foreach (var medico in medicos) {
			var primero = await CalcularPrimerSlotDisponible(medico, especialidad, repo);
			if (primero is Result<DateTime>.Error errSlot) {
				yield return new Result<DisponibilidadEspecialidad2025>.Error(errSlot.Mensaje);
				yield break;
			}
			ordenados.Add((medico, ((Result<DateTime>.Ok)primero).Valor));
		}
		foreach (var x in ordenados.OrderBy(t => t.slot)) {
			await foreach (var disp in GenerarDisponibilidadesDeMedico(
				especialidad, fechaCreacion, x.medico, repo)) {
				yield return disp;
			}
		}
	}



	private static async IAsyncEnumerable<Result<DisponibilidadEspecialidad2025>> GenerarDisponibilidadesDeMedico(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		MedicoId medicoId,
		IRepositorioDomain repositorio
	) {
		int duracion = solicitudEspecialidad.DuracionConsultaMinutos;
		int semanas = 30;

		DateTime desdeBusqueda = solicitudFechaCreacion.Date;
		DateTime hastaBusqueda = solicitudFechaCreacion.Date.AddDays(7 * semanas);

		var turnosResult = (await repositorio.SelectTurnosProgramadosBetweenFechasWhereMedicoId(
			medicoId, desdeBusqueda, hastaBusqueda));

		if (turnosResult.IsError) {
			yield return new Result<DisponibilidadEspecialidad2025>.Error(turnosResult.UnwrapAsError());
			yield break;
		}
		var turnos = turnosResult.UnwrapAsOk();


		var franjasResult = (await repositorio.SelectHorariosVigentesBetweenFechasWhereMedicoId(
			medicoId, desdeBusqueda, hastaBusqueda));

		if (franjasResult.IsError) {
			yield return new Result<DisponibilidadEspecialidad2025>.Error(franjasResult.UnwrapAsError());
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
						solicitudEspecialidad, medicoId,
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
						yield return new Result<DisponibilidadEspecialidad2025>.Ok(disp);
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
			return new Result<DateTime>.Error($"Error de la base de datos.\n\t No se pudo acceder a los turnos programados del MedicoId {medicoId}: \n\t\t Traceback: {turnosResult.UnwrapAsError()}");
		}
		var turnos = turnosResult.UnwrapAsOk();

		// 2. Cargar sus horarios vigentes
		var franjasResult = (await repositorio.SelectHorariosVigentesBetweenFechasWhereMedicoId(
			medicoId, desdeBusqueda, hastaBusqueda));
		if (franjasResult.IsError) {
			return new Result<DateTime>.Error($"Error de la base de datos.\n\tNo se pudo acceder a los horarios del MedicoId{medicoId}:\n\t\tTraceback: {turnosResult.UnwrapAsError()}");
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
		return new Result<DateTime>.Error("No hay disponibilidad para este medicoId"); // no hay turnos disponibles
	}


}
