using System;
using System.Collections.Generic;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.Servicios;

public static class ServiciosPublicos {


	public static Result<IReadOnlyList<DisponibilidadEspecialidad2025>> SolicitarDisponibilidadesPara(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		int cuantos,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, IEnumerable<HorarioMedico2025>> funcSelectHorariosWhereMedicoId,
		Func<MedicoId, DateTime, IEnumerable<Turno2025>> funcSelectTurnosWhereMedicoIdAndDate
	) {

		IReadOnlyList<DisponibilidadEspecialidad2025> disponibles = [.. ServiciosPrivados._GenerarDisponibilidades(
			solicitudEspecialidad, 
			solicitudFechaCreacion,
			funcSelectMedicosWhereEspecialidad,
			funcSelectHorariosWhereMedicoId,
			funcSelectTurnosWhereMedicoIdAndDate
		)
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
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, IEnumerable<HorarioMedico2025>> funcSelectHorariosWhereMedicoId,
		Func<MedicoId, DateTime, IEnumerable<Turno2025>> funcSelectTurnosWhereMedicoIdAndDate,
		Func<Turno2025, Task<Result<TurnoId>>> funcInsertTurnoReturnId
	) {
		//Faltaria validacion pacienteId in Pacientes.
		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1 = ServiciosPrivados._EncontrarProximaDisponibilidad(
			solicitudEspecialidad, 
			solicitudFechaCreacion,
			funcSelectMedicosWhereEspecialidad,
			funcSelectHorariosWhereMedicoId,
			funcSelectTurnosWhereMedicoIdAndDate
		);
		Result<Turno2025> turnoProvisorioResult = Turno2025.Crear(new TurnoId(-1), pacienteId, solicitudFechaCreacion, disponibilidadParaPaciente1);

		if (turnoProvisorioResult is Result<Turno2025>.Error err3)
			return err3;
		Turno2025 turnoProvisorio = ((Result<Turno2025>.Ok)turnoProvisorioResult).Valor;

		// ---------------------------------------------------------
		// 5. IO: Insertar nuevo turno y obtener ID
		// ---------------------------------------------------------
		Result<TurnoId> insertResult = await funcInsertTurnoReturnId(turnoProvisorio);

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
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, IEnumerable<HorarioMedico2025>> funcSelectHorariosWhereMedicoId,
		Func<MedicoId, DateTime, IEnumerable<Turno2025>> funcSelectTurnosWhereMedicoIdAndDate,
		Func<Turno2025, Task<Result<Unit>>> funcUpdateTurnoWhereId,
		Func<Turno2025, Task<Result<TurnoId>>> funcInsertTurnoReturnId
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
		Result<Unit> updateResult = await funcUpdateTurnoWhereId(turnoCancelado);

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
			funcSelectMedicosWhereEspecialidad,
			funcSelectHorariosWhereMedicoId,
			funcSelectTurnosWhereMedicoIdAndDate
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
		Result<TurnoId> insertResult = await funcInsertTurnoReturnId(turnoProvisorio);

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
		Func<Turno2025, Task<Result<Unit>>> funcUpdateTurnoWhereId
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
		Result<Unit> updateResult = await funcUpdateTurnoWhereId(turnoCancelado);

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
