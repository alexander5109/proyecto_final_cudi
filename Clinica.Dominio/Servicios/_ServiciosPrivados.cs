using Clinica.Dominio._Disabled;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.IInterfaces.QueryModels;

namespace Clinica.Dominio.Servicios;

internal static class _ServiciosPrivados {
	//internal static Result<Disponibilidad2025> TomarPrimera(this Result<IReadOnlyList<Disponibilidad2025>> listadoResult) {
	//	return listadoResult.MatchAndSet(
	//		ok => {
	//			if (ok.Count == 0)
	//				return new Result<Disponibilidad2025>.Error(
	//					"No hay disponibilidades para seleccionar."
	//				);
	//			return new Result<Disponibilidad2025>.Ok(ok[0]);
	//		},
	//		mensajeError =>
	//			new Result<Disponibilidad2025>.Error(mensajeError)
	//	);
	//}

	internal static Result<IReadOnlyList<Disponibilidad2025>> AplicarFiltrosOpcionales(this Result<IReadOnlyList<Disponibilidad2025>> disponibilidadesResult, SolicitudDeTurnoPreferencias preferencias) {
		if (disponibilidadesResult is Result<IReadOnlyList<Disponibilidad2025>>.Error err)
			return new Result<IReadOnlyList<Disponibilidad2025>>.Error(err.Mensaje);

		IReadOnlyList<Disponibilidad2025> lista = ((Result<IReadOnlyList<Disponibilidad2025>>.Ok)disponibilidadesResult).Valor;
		IEnumerable<Disponibilidad2025> filtradas = lista;
		if (preferencias.DiaPreferido is DayOfWeek dia)
			filtradas = filtradas.Where(d => d.FechaHoraDesde.DayOfWeek == dia);

		if (preferencias.MomentoPreferido is TardeOMañana momento)
			filtradas = filtradas.Where(d => momento.AplicaA(d.FechaHoraDesde));

		if (filtradas.Any()) {
			return new Result<IReadOnlyList<Disponibilidad2025>>.Ok([.. filtradas]);
		} else {
			return new Result<IReadOnlyList<Disponibilidad2025>>.Error("No se encontraron disponibilidades");
		}
	}

	/*
	internal static async Task<Result<Disponibilidad2025>> EncontrarProximaDisponibilidad(
			Especialidad2025 solicitudEspecialidad,
			DateTime aPartirDeCuando,
			IRepositorioDomainServiciosPrivados repositorio
	) {
		await foreach (Result<Disponibilidad2025> disp in GenerarDisponibilidades(
			solicitudEspecialidad,
			aPartirDeCuando,
			repositorio)) {
			return disp;
		}
		return new Result<Disponibilidad2025>.Error("No se encontró ninguna disponibilidad.");
	}
	*/

	internal static async IAsyncEnumerable<Result<Disponibilidad2025>>GenerarDisponibilidades(
		Especialidad2025 especialidad,
		DateTime aPartirDeCuando,
		IRepositorioDominioServices repo
	) {
        Result<IEnumerable<MedicoId>> medicosResult = await repo.SelectMedicosIdWhereEspecialidadCodigo(especialidad.Codigo);
		if (medicosResult is Result<IEnumerable<MedicoId>>.Error errMed) {
			yield return new Result<Disponibilidad2025>.Error(errMed.Mensaje);
			yield break;
		}
        IEnumerable<MedicoId> medicos = ((Result<IEnumerable<MedicoId>>.Ok)medicosResult).Valor;
        List<(MedicoId medico, DateTime slot)> ordenados = new List<(MedicoId medico, DateTime slot)>();
		foreach (MedicoId medico in medicos) {
            Result<DateTime> primero = await CalcularPrimerSlotDisponible(medico, especialidad, aPartirDeCuando, repo);
			if (primero is Result<DateTime>.Error errSlot) {
				yield return new Result<Disponibilidad2025>.Error(errSlot.Mensaje);
				yield break;
			}
			ordenados.Add((medico, ((Result<DateTime>.Ok)primero).Valor));
		}
		foreach ((MedicoId medico, DateTime slot) x in ordenados.OrderBy(t => t.slot)) {
			await foreach (Result<Disponibilidad2025> disp in GenerarDisponibilidadesDeMedico(
				especialidad, aPartirDeCuando, x.medico, repo)) {
				yield return disp;
			}
		}
	}



	private static async IAsyncEnumerable<Result<Disponibilidad2025>> GenerarDisponibilidadesDeMedico(
		Especialidad2025 solicitudEspecialidad,
		DateTime aPartirDeCuando,
		MedicoId medicoId,
		IRepositorioDominioServices repositorio
	) {
		int duracion = solicitudEspecialidad.DuracionConsultaMinutos;
		int semanas = 30;

		//DateTime desdeBusqueda = aPartirDeCuando.Date;
		DateTime hastaBusqueda = aPartirDeCuando.Date.AddDays(7 * semanas);

        Result<IEnumerable<TurnoQM>> turnosResult = (await repositorio.SelectTurnosProgramadosBetweenFechasWhereMedicoId(
			medicoId, aPartirDeCuando, hastaBusqueda));

		if (turnosResult.IsError) {
			yield return new Result<Disponibilidad2025>.Error(turnosResult.UnwrapAsError());
			yield break;
		}
        IEnumerable<TurnoQM> turnos = turnosResult.UnwrapAsOk();


        Result<IEnumerable<HorarioMedicoQM>> franjasResult = (await repositorio.SelectHorariosVigentesBetweenFechasWhereMedicoId(
			medicoId, aPartirDeCuando, hastaBusqueda));

		if (franjasResult.IsError) {
			yield return new Result<Disponibilidad2025>.Error(franjasResult.UnwrapAsError());
			yield break;
		}
        IEnumerable<HorarioMedicoQM> franjas = franjasResult.UnwrapAsOk();


		foreach (HorarioMedicoQM? franja in franjas) {
			DateTime fecha = aPartirDeCuando;
			while (fecha.DayOfWeek != franja.DiaSemana)
				fecha = fecha.AddDays(1);

			for (int semana = 0; semana < semanas; semana++, fecha = fecha.AddDays(7)) {
				DateTime desde = fecha + franja.HoraDesde;
				DateTime hasta = fecha + franja.HoraHasta;

				if (desde < aPartirDeCuando)
					continue;

				for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
                    Disponibilidad2025 disp = new Disponibilidad2025(
						solicitudEspecialidad.Codigo, 
						medicoId,
						slot, 
						slot.AddMinutes(duracion)
						//DiaSemana2025.CrearResult(slot.DayOfWeek)
					);

					bool solapa = false;
					foreach (TurnoQM? t in turnos) {
						if (t.EspecialidadCodigo == disp.EspecialidadCodigo &&
							t.OutcomeEstado == TurnoEstadoCodigo.Programado &&
							t.FechaHoraAsignadaDesde < disp.FechaHoraHasta &&
							disp.FechaHoraDesde < t.FechaHoraAsignadaHasta) {
							solapa = true;
							break;
						}
					}

					if (!solapa)
						yield return new Result<Disponibilidad2025>.Ok(disp);
				}
			}
		}
	}

	public static async Task<Result<DateTime>> CalcularPrimerSlotDisponible(
		MedicoId medicoId,
		Especialidad2025 especialidad,
		DateTime aPartirDeCuando,
		IRepositorioDominioServices repositorio
	) {
		int duracion = especialidad.DuracionConsultaMinutos;

		DateTime hastaBusqueda = aPartirDeCuando.AddDays(7 * 30); // 30 semanas

        // 1. Cargar turnos del médico en el rango
        Result<IEnumerable<TurnoQM>> turnosResult = (await repositorio.SelectTurnosProgramadosBetweenFechasWhereMedicoId(medicoId, aPartirDeCuando, hastaBusqueda));
		if (turnosResult.IsError) {
			return new Result<DateTime>.Error($"Error de la base de datos.\n\t No se pudo acceder a los turnos programados del MedicoId {medicoId}: \n\t\t Traceback: {turnosResult.UnwrapAsError()}");
		}
        IEnumerable<TurnoQM> turnos = turnosResult.UnwrapAsOk();

        // 2. Cargar sus horarios vigentes
        Result<IEnumerable<HorarioMedicoQM>> franjasResult = (await repositorio.SelectHorariosVigentesBetweenFechasWhereMedicoId(
			medicoId, aPartirDeCuando, hastaBusqueda));
		if (franjasResult.IsError) {
			return new Result<DateTime>.Error($"Error de la base de datos.\n\tNo se pudo acceder a los horarios del MedicoId{medicoId}:\n\t\tTraceback: {turnosResult.UnwrapAsError()}");
		}
        IEnumerable<HorarioMedicoQM> franjas = franjasResult.UnwrapAsOk();

		foreach (HorarioMedicoQM franja in franjas) {
			// Ubicar primer día coincidente con el DíaSemana de la franja
			DateTime fecha = aPartirDeCuando;
			while (fecha.DayOfWeek != franja.DiaSemana)
				fecha = fecha.AddDays(1);

			// 3. Iterar por semanas
			for (int semana = 0; semana < 30; semana++, fecha = fecha.AddDays(7)) {
				DateTime desde = fecha + franja.HoraDesde;
				DateTime hasta = fecha + franja.HoraHasta;

				if (hasta <= aPartirDeCuando)
					continue;

				// 4. Iterar slots dentro de la franja
				for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
					DateTime slotHasta = slot.AddMinutes(duracion);

					if (slotHasta <= aPartirDeCuando)
						continue;

					bool solapa = false;

					foreach (TurnoQM t in turnos) {
						if (t.EspecialidadCodigo == especialidad.Codigo &&
							t.OutcomeEstado == TurnoEstadoCodigo.Programado &&
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
		return new Result<DateTime>.Error("No se encontraron turnos disponibles");
	}


}
