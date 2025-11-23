using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.ListasOrganizadoras;

public static class ServicioTurnosManager {


	public static Result<IReadOnlyList<DisponibilidadEspecialidad2025>> SolicitarDisponibilidadesPara(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		int cuantos,
		IReadOnlyList<Result<Medico2025>> medicosResults,
		IReadOnlyList<Result<Turno2025>> turnosResults
	) {
		Result<IReadOnlyList<Medico2025>> medicosVerificados = ServicioTurnosManager._ValidarMedicos(medicosResults);
		if (medicosVerificados.IsError) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Medico2025>>.Error)medicosVerificados).Mensaje}");
		}
		IReadOnlyList<Medico2025> medicos = ((Result<IReadOnlyList<Medico2025>>.Ok)medicosVerificados).Valor;



		Result<IReadOnlyList<Turno2025>> turnosVerificados = ServicioTurnosManager._ValidarTurnos(turnosResults);
		if (turnosVerificados.IsError) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Turno2025>>.Error)turnosVerificados).Mensaje}");
		}
		IReadOnlyList<Turno2025> turnos = ((Result<IReadOnlyList<Turno2025>>.Ok)turnosVerificados).Valor;



		IReadOnlyList<DisponibilidadEspecialidad2025> disponibles = [.. _GenerarDisponibilidades(solicitudEspecialidad, solicitudFechaCreacion, medicos, turnos)
			.OrderBy(d => d.FechaHoraDesde)
			.Take(cuantos)];
		if (disponibles.Count > 0) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok(disponibles);
		} else {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error("No se encontraron disponibilidades");
		}
	}


	public static Result<Turno2025> SolicitarTurnoEnLaPrimeraDisponibilidad(
		PacienteId pacienteId,
		EspecialidadMedica2025 especialidadMedica,
		DateTime when,
		IReadOnlyList<Result<Medico2025>> medicosResults,
		IReadOnlyList<Result<Turno2025>> turnosResults
	) {
		//Faltaria validacion pacienteId in Pacientes.

		Result<IReadOnlyList<Medico2025>> medicosVerificados = _ValidarMedicos(medicosResults);
		if (medicosVerificados.IsError) {
			return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Medico2025>>.Error)medicosVerificados).Mensaje}");
		}
		IReadOnlyList<Medico2025> medicos = ((Result<IReadOnlyList<Medico2025>>.Ok)medicosVerificados).Valor;





		Result<IReadOnlyList<Turno2025>> turnosVerificados = _ValidarTurnos(turnosResults);
		if (turnosVerificados.IsError) {
			return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Turno2025>>.Error)turnosVerificados).Mensaje}");
		}
		IReadOnlyList<Turno2025> turnos = ((Result<IReadOnlyList<Turno2025>>.Ok)turnosVerificados).Valor;





		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1 = _EncontrarProximaDisponibilidad(especialidadMedica, when, medicos, turnos);

		Result<Turno2025> turno = Turno2025.Crear(new TurnoId(1), pacienteId, when, disponibilidadParaPaciente1);

		//TO-DO POST
		//_AgendarTurno(turno, turnos).PrintAndContinue("Agendando medico: ");

		return turno;
	}

	public static Result<Turno2025> SolicitarReprogramacionALaPrimeraDisponibilidad(
		Result<Turno2025> turnoOriginalResult,
		DateTime when,
		string why,
		IReadOnlyList<Result<Medico2025>> medicosResults,
		IReadOnlyList<Result<Turno2025>> turnosResults
	) {
		Result<IReadOnlyList<Turno2025>> turnosVerificados = _ValidarTurnos(turnosResults);
		if (turnosVerificados.IsError) {
			return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Turno2025>>.Error)turnosVerificados).Mensaje}");
		}
		IReadOnlyList<Turno2025> turnos = ((Result<IReadOnlyList<Turno2025>>.Ok)turnosVerificados).Valor;


		//Faltaria validacion pacienteId in Pacientes.



		Result<IReadOnlyList<Medico2025>> medicosVerificados = _ValidarMedicos(medicosResults);
		if (medicosVerificados.IsError) {
			return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Medico2025>>.Error)medicosVerificados).Mensaje}");
		}
		IReadOnlyList<Medico2025> medicos = ((Result<IReadOnlyList<Medico2025>>.Ok)medicosVerificados).Valor;




		if (turnoOriginalResult.IsError) {
			return new Result<Turno2025>.Error($"No se puede reprogramar un medico con errores previos: {((Result<Turno2025>.Error)turnoOriginalResult).Mensaje}");
		}
		Turno2025 turnoOriginal = ((Result<Turno2025>.Ok)turnoOriginalResult).Valor;


		Result<Turno2025> turnoCanceladoResult = _CambiarEstadoInterno(turnoOriginal, TurnoOutcomeEstado2025.Reprogramado, when, why, turnos);
		if (turnoCanceladoResult.IsError) return turnoCanceladoResult;
		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)turnoCanceladoResult).Valor;

		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1_reprogramado = _EncontrarProximaDisponibilidad(turnoCancelado.Especialidad, when, medicos, turnos);


		Result<Turno2025> nuevoTurno = Turno2025.Crear(new TurnoId(2), turnoCancelado.PacienteId, when, disponibilidadParaPaciente1_reprogramado)
			.PrintAndContinue("Creando nuevo medico: ");

		//TO-DO POST
		//_AgendarTurno(nuevoTurno, turnos).PrintAndContinue("Agendando nuevo medico: ");

		return nuevoTurno;
	}

	public static Result<Turno2025> SolicitarCancelacion(
		Result<Turno2025> turnoOriginalResult,
		DateTime solicitudFecha,
		string solicitudReason,
		IReadOnlyList<Result<Turno2025>> turnosResults

	) {
		if (turnoOriginalResult.IsError) {
			return new Result<Turno2025>.Error($"No se puede reprogramar un medico con errores previos: {((Result<Turno2025>.Error)turnoOriginalResult).Mensaje}");
		}
		Turno2025 turnoOriginal = ((Result<Turno2025>.Ok)turnoOriginalResult).Valor;




		Result<IReadOnlyList<Turno2025>> turnosVerificados = _ValidarTurnos(turnosResults);
		if (turnosVerificados.IsError) {
			return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Turno2025>>.Error)turnosVerificados).Mensaje}");
		}
		IReadOnlyList<Turno2025> turnos = ((Result<IReadOnlyList<Turno2025>>.Ok)turnosVerificados).Valor;



		return _CambiarEstadoInterno(turnoOriginal, TurnoOutcomeEstado2025.Cancelado, solicitudFecha, solicitudReason, turnos);
	}




	//------------------------------------------------PRIVATE-------------------------------------------------------//
	private static Result<IReadOnlyList<Turno2025>> _ValidarTurnos(IReadOnlyList<Result<Turno2025>> turnosResults) {
		List<string> errores = [];
		foreach (Result<Turno2025> turno in turnosResults) {
			if (turno is Result<Turno2025>.Error err)
				errores.Add(err.Mensaje);
			turno.PrintAndContinue("Turno domainizado");
		}
		if (errores.Count > 0)
			return new Result<IReadOnlyList<Turno2025>>.Error(string.Join("; ", errores));
		IReadOnlyList<Turno2025> turnosOk = [.. turnosResults
			.Cast<Result<Turno2025>.Ok>()
			.Select(ok => ok.Valor)];
		return new Result<IReadOnlyList<Turno2025>>.Ok(turnosOk);
	}

	private static Result<IReadOnlyList<Paciente2025>> _ValidaPacientes(IReadOnlyList<Result<Paciente2025>> pacientesResults) {
		List<string> errores = [];
		foreach (Result<Paciente2025> paciente in pacientesResults) {
			if (paciente is Result<Paciente2025>.Error err)
				errores.Add(err.Mensaje);
			paciente.PrintAndContinue("Paciente domainizado");
		}
		if (errores.Count > 0)
			return new Result<IReadOnlyList<Paciente2025>>.Error(string.Join("; ", errores));
		IReadOnlyList<Paciente2025> pacientesOk = [.. pacientesResults
			.Cast<Result<Paciente2025>.Ok>()
			.Select(ok => ok.Valor)];
		return new Result<IReadOnlyList<Paciente2025>>.Ok(pacientesOk);
	}


	private static Result<IReadOnlyList<Medico2025>> _ValidarMedicos(IReadOnlyList<Result<Medico2025>> medicosResults) {
		List<string> errores = [];
		foreach (Result<Medico2025> medico in medicosResults) {
			if (medico is Result<Medico2025>.Error err)
				errores.Add(err.Mensaje);
			medico.PrintAndContinue("Medico domainizado");
		}
		if (errores.Count > 0)
			return new Result<IReadOnlyList<Medico2025>>.Error(string.Join("; ", errores));
		IReadOnlyList<Medico2025> medicosOk = [.. medicosResults
			.Cast<Result<Medico2025>.Ok>()
			.Select(ok => ok.Valor)];
		return new Result<IReadOnlyList<Medico2025>>.Ok(medicosOk);
	}


	private static Result<Turno2025> _AgendarTurno(Result<Turno2025> turnoResult, List<Turno2025> turnos) {
		return turnoResult.Match<Result<Turno2025>>(
			ok => {
				turnos.Add(ok);
				return turnoResult;
			},
			mensaje => turnoResult
		);
	}

	private static Result<DisponibilidadEspecialidad2025> _EncontrarProximaDisponibilidad(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		IReadOnlyList<Medico2025> medicos,
		IReadOnlyList<Turno2025> turnos
	) {
		DisponibilidadEspecialidad2025? proxima = _GenerarDisponibilidades(solicitudEspecialidad, solicitudFechaCreacion, medicos, turnos).FirstOrDefault();
		if (proxima is null)
			return new Result<DisponibilidadEspecialidad2025>.Error("No se encontraron disponibilidades");
		return new Result<DisponibilidadEspecialidad2025>.Ok((DisponibilidadEspecialidad2025)proxima);
	}



	private static Result<Turno2025> _CambiarEstadoInterno(
		Turno2025 turnoOriginal,
		TurnoOutcomeEstado2025 outcomeEstado,
		DateTime outcomeFecha,
		string outcomeComentario,
		IReadOnlyList<Turno2025> turnos
	) {

		//Why can't we use .FindIndex over IReadOnlyList?
		int idx = turnos.ToList().FindIndex(t => t.Id == turnoOriginal.Id);
		if (idx == -1)
			return new Result<Turno2025>.Error("El medico no existe en esta ListaTurnos.");

		Result<Turno2025> nuevoTurnoResult = turnoOriginal.CambiarEstado(outcomeEstado, outcomeFecha, outcomeComentario);
		//if (nuevoTurnoResult.IsOk) {
		//	turnos.RemoveAt(idx);
		//	turnos.Add(((Result<Turno2025>.Ok)nuevoTurnoResult).Valor);
		//}
		//IRRELEVANT IF INMUTABLE
		return nuevoTurnoResult;
	}

	private static bool _DisponibilidadNoColisiona(
		MedicoId medicoId,
		EspecialidadMedica2025 especialidad,
		DateTime fechaHoraDesde,
		DateTime fechaHoraHasta,
		IReadOnlyList<Turno2025> turnos
	) {
		//Faltaria validacion medicoId in Medicos

		foreach (Turno2025 turno in turnos) {

			// 1) Debe ser del mismo médico y especialidad
			if (turno.MedicoId != medicoId) continue;
			if (turno.Especialidad != especialidad) continue;

			// 2) Solo los medicosResults programados bloquean agenda
			if (turno.OutcomeEstado != TurnoOutcomeEstado2025.Programado) continue;

			// 3) Chequeo de solapamiento clásico
			bool solapa =
				turno.FechaHoraAsignadaDesde < fechaHoraHasta &&
				fechaHoraDesde < turno.FechaHoraAsignadaHasta;

			if (solapa)
				return false;
		}

		return true;
	}
	private static IEnumerable<DisponibilidadEspecialidad2025> _GenerarDisponibilidades(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		IReadOnlyList<Medico2025> medicos,
		IReadOnlyList<Turno2025> turnos
	) {
		foreach (Medico2025 medico in medicos)
			foreach (EspecialidadMedica2025 especialidad in medico.Especialidades.Valores) {
				if (especialidad != solicitudEspecialidad)
					continue;

				int duracion = especialidad.DuracionConsultaMinutos;

				foreach (HorarioMedico2025 franja in medico.ListaHorarios.Valores) {
					DateTime fecha = solicitudFechaCreacion.Date;

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
								medico.Id,
								slot,
								slot.AddMinutes(duracion)
							);

							// chequeo de colisión inline
							if (!_DisponibilidadNoColisiona(disp.MedicoId, disp.Especialidad, disp.FechaHoraDesde, disp.FechaHoraHasta, turnos))
								continue;

							yield return disp;
						}
					}
				}
			}
	}









}
