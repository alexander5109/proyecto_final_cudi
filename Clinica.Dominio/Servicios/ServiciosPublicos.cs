using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.Servicios;

public static class ServiciosPublicos {


	public static Result<IReadOnlyList<DisponibilidadEspecialidad2025>> SolicitarDisponibilidadesPara(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		int cuantos,
		IReadOnlyList<Result<Medico2025>> medicosResults,
		IReadOnlyList<Result<Turno2025>> turnosResults
	) {
		Result<IReadOnlyList<Medico2025>> medicosVerificados = ServiciosPrivados._ValidarMedicos(medicosResults);
		if (medicosVerificados.IsError) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Medico2025>>.Error)medicosVerificados).Mensaje}");
		}
		IReadOnlyList<Medico2025> medicos = ((Result<IReadOnlyList<Medico2025>>.Ok)medicosVerificados).Valor;



		Result<IReadOnlyList<Turno2025>> turnosVerificados = ServiciosPrivados._ValidarTurnos(turnosResults);
		if (turnosVerificados.IsError) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Turno2025>>.Error)turnosVerificados).Mensaje}");
		}
		IReadOnlyList<Turno2025> turnos = ((Result<IReadOnlyList<Turno2025>>.Ok)turnosVerificados).Valor;



		IReadOnlyList<DisponibilidadEspecialidad2025> disponibles = [.. ServiciosPrivados._GenerarDisponibilidades(solicitudEspecialidad, solicitudFechaCreacion, medicos, turnos)
			.OrderBy(d => d.FechaHoraDesde)
			.Take(cuantos)];
		if (disponibles.Count > 0) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok(disponibles);
		} else {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error("No se encontraron proximaDisponibilidad");
		}
	}


	public static async Task<Result<Turno2025>> SolicitarTurnoEnLaPrimeraDisponibilidad(
		PacienteId pacienteId,
		EspecialidadMedica2025 especialidadMedica,
		DateTime when,
		IReadOnlyList<Result<Medico2025>> medicosResults,
		IReadOnlyList<Result<Turno2025>> turnosResults,
		Func<Turno2025, Task<Result<TurnoId>>> funcInsertTurno
	) {
		//Faltaria validacion pacienteId in Pacientes.
		Result<IReadOnlyList<Medico2025>> medicosVerificados = ServiciosPrivados._ValidarMedicos(medicosResults);
		if (medicosVerificados.IsError) {
			return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Medico2025>>.Error)medicosVerificados).Mensaje}");
		}
		IReadOnlyList<Medico2025> medicos = ((Result<IReadOnlyList<Medico2025>>.Ok)medicosVerificados).Valor;

		Result<IReadOnlyList<Turno2025>> turnosVerificados = ServiciosPrivados._ValidarTurnos(turnosResults);
		if (turnosVerificados.IsError) {
			return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Turno2025>>.Error)turnosVerificados).Mensaje}");
		}
		IReadOnlyList<Turno2025> turnos = ((Result<IReadOnlyList<Turno2025>>.Ok)turnosVerificados).Valor;
		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1 = ServiciosPrivados._EncontrarProximaDisponibilidad(especialidadMedica, when, medicos, turnos);
		Result<Turno2025> turnoProvisorioResult = Turno2025.Crear(new TurnoId(-1), pacienteId, when, disponibilidadParaPaciente1);

		if (turnoProvisorioResult is Result<Turno2025>.Error err3)
			return err3;
		Turno2025 turnoProvisorio = ((Result<Turno2025>.Ok)turnoProvisorioResult).Valor;

		// ---------------------------------------------------------
		// 5. IO: Insertar nuevo turno y obtener ID
		// ---------------------------------------------------------
		Result<TurnoId> insertResult = await funcInsertTurno(turnoProvisorio);

		switch (insertResult) {
			case Result<TurnoId>.Error errInsert:
				return new Result<Turno2025>.Error(
					$"Error al persistir el nuevo turno reprogramado: {errInsert.Mensaje}"
				);

			case Result<TurnoId>.Ok okInsert: {
				TurnoId idReal = okInsert.Valor;

				// -------------------------------------------------
				// 6. Adjuntar Id real (PURO)
				// -------------------------------------------------
				Turno2025 turnoFinal = turnoProvisorio with { Id = idReal };

				return new Result<Turno2025>.Ok(turnoFinal);
			}
			default:
				throw new Exception("Inalcanzable"); // por exhaustividad
		}
	}



	public static async Task<Result<Turno2025>> SolicitarReprogramacionALaPrimeraDisponibilidad(
		Turno2025 turnoOriginal,
		DateTime outcomeFecha,
		string outcomeComentario,
		IReadOnlyList<Medico2025> medicos,
		IReadOnlyList<Turno2025> turnos,
		Func<Turno2025, Task<Result<Unit>>> funcUpdateTurno,
		Func<Turno2025, Task<Result<TurnoId>>> funcInsertTurno
	) {
		// ---------------------------------------------------------
		// 1. Cambiar estado del turno original (PURO)
		// ---------------------------------------------------------
		var turnoCanceladoResult = turnoOriginal.SetOutcome(
			TurnoOutcomeEstado2025.Reprogramado,
			outcomeFecha,
			outcomeComentario
		);

		if (turnoCanceladoResult is Result<Turno2025>.Error err1)
			return new Result<Turno2025>.Error(err1.Mensaje);

		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)turnoCanceladoResult).Valor;


		// ---------------------------------------------------------
		// 2. IO: Persistir cambios del turno cancelado
		// ---------------------------------------------------------
		Result<Unit> updateResult = await funcUpdateTurno(turnoCancelado);

		switch (updateResult) {
			case Result<Unit>.Error errUpdate:
				return new Result<Turno2025>.Error(
					$"Error al persistir la cancelación del turno: {errUpdate.Mensaje}"
				);

			case Result<Unit>.Ok:
				break; // OK, continuamos
		}


		// ---------------------------------------------------------
		// 3. Encontrar próxima disponibilidad (PURO)
		// ---------------------------------------------------------
		var dispResult = ServiciosPrivados._EncontrarProximaDisponibilidad(
			turnoOriginal.Especialidad,
			outcomeFecha,
			medicos,
			turnos
		);

		if (dispResult is Result<DisponibilidadEspecialidad2025>.Error errDisp)
			return new Result<Turno2025>.Error(errDisp.Mensaje);


		// ---------------------------------------------------------
		// 4. Crear nuevo turno (PURO)
		//    El ID aún no lo sabemos → se pone dummy
		// ---------------------------------------------------------
		var turnoProvisorioResult = turnoCancelado.Reprogramar(
			dispResult,
			new TurnoId(-1) // placeholder
		);

		if (turnoProvisorioResult is Result<Turno2025>.Error err3)
			return err3;

		Turno2025 turnoProvisorio = ((Result<Turno2025>.Ok)turnoProvisorioResult).Valor;


		// ---------------------------------------------------------
		// 5. IO: Insertar nuevo turno y obtener ID
		// ---------------------------------------------------------
		Result<TurnoId> insertResult = await funcInsertTurno(turnoProvisorio);

		switch (insertResult) {
			case Result<TurnoId>.Error errInsert:
				return new Result<Turno2025>.Error(
					$"Error al persistir el nuevo turno reprogramado: {errInsert.Mensaje}"
				);

			case Result<TurnoId>.Ok okInsert: {
				TurnoId idReal = okInsert.Valor;

				// -------------------------------------------------
				// 6. Adjuntar Id real (PURO)
				// -------------------------------------------------
				Turno2025 turnoFinal = turnoProvisorio with { Id = idReal };

				return new Result<Turno2025>.Ok(turnoFinal);
			}

			default:
				throw new Exception("Inalcanzable"); // por exhaustividad
		}
	}



	public static async Task<Result<Turno2025>> SolicitarCancelacion(
		Result<Turno2025> turnoOriginalResult,
		DateTime outcomeFecha,
		string outcomeComentario,
		Func<Turno2025, Task<Result<Unit>>> funcUpdateTurno
	) {
		if (turnoOriginalResult.IsError) {
			return new Result<Turno2025>.Error($"No se puede reprogramar un turno con errores previos: {((Result<Turno2025>.Error)turnoOriginalResult).Mensaje}");
		}
		Turno2025 turnoOriginal = ((Result<Turno2025>.Ok)turnoOriginalResult).Valor;


        Result<Turno2025> turnoCanceladoResult = turnoOriginal.SetOutcome(TurnoOutcomeEstado2025.Cancelado, outcomeFecha, outcomeComentario);

		if (turnoCanceladoResult is Result<Turno2025>.Error err1)
			return new Result<Turno2025>.Error(err1.Mensaje);

		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)turnoCanceladoResult).Valor;



		//Result<IReadOnlyList<Turno2025>> turnosVerificados = ServiciosPrivados._ValidarTurnos(turnosResults);
		//if (turnosVerificados.IsError) {
		//	return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Turno2025>>.Error)turnosVerificados).Mensaje}");
		//}
		//IReadOnlyList<Turno2025> turnos = ((Result<IReadOnlyList<Turno2025>>.Ok)turnosVerificados).Valor;



		// ---------------------------------------------------------
		// 2. IO: Persistir cambios del turno cancelado
		// ---------------------------------------------------------
		Result<Unit> updateResult = await funcUpdateTurno(turnoCancelado);

		switch (updateResult) {
			case Result<Unit>.Error errUpdate:
				return new Result<Turno2025>.Error(
					$"Error al persistir la cancelación del turno: {errUpdate.Mensaje}"
				);

			case Result<Unit>.Ok:
				return new Result<Turno2025>.Ok(turnoCancelado);

			default:
				throw new Exception("Inalcanzable"); // por exhaustividad
		}
	}



}
