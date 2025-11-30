using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.Servicios;

public static class ServiciosPublicos {

	//CRUD paciente
	//CRUD medico
	//CRUD turno
	//public static async Task<Result<IReadOnlyList<DisponibilidadEspecialidad2025>>> Crear(






	public static async Task<Result<IReadOnlyList<DisponibilidadEspecialidad2025>>> SolicitarDisponibilidadesPara(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		int cuantos,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, DateTime, DateTime, IEnumerable<HorarioMedico2025>> funcSelectHorariosVigentesBetweenFechasWhereMedicoId,
		Func<MedicoId, DateTime, DateTime, IEnumerable<Turno2025>> funcSelectTurnosProgramadosBetweenFechasWhereMedicoId
	) {
		IReadOnlyList<DisponibilidadEspecialidad2025> disponibles = [.. ServiciosPrivados.GenerarDisponibilidades(
			solicitudEspecialidad,
			solicitudFechaCreacion,
			funcSelectMedicosWhereEspecialidad,
			funcSelectHorariosVigentesBetweenFechasWhereMedicoId,
			funcSelectTurnosProgramadosBetweenFechasWhereMedicoId
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
		FechaRegistro2025 solicitudFechaCreacion,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, DateTime, DateTime, IEnumerable<HorarioMedico2025>> funcSelectHorariosVigentesBetweenFechasWhereMedicoId,
		Func<MedicoId, DateTime, DateTime, IEnumerable<Turno2025>> funcSelectTurnosProgramadosBetweenFechasWhereMedicoId,
		Func<Turno2025, Task<Result<TurnoId>>> funcInsertTurnoReturnId
	) {
		Result<DisponibilidadEspecialidad2025> dispResult = ServiciosPrivados.EncontrarProximaDisponibilidad(
			solicitudEspecialidad,
			solicitudFechaCreacion.Valor,
			funcSelectMedicosWhereEspecialidad,
			funcSelectHorariosVigentesBetweenFechasWhereMedicoId,
			funcSelectTurnosProgramadosBetweenFechasWhereMedicoId
		);

		if (dispResult is Result<DisponibilidadEspecialidad2025>.Error e1)
			return new Result<Turno2025>.Error(e1.Mensaje);

		Result<Turno2025> turnoResult = Turno2025.ProgramarNuevo(
			new TurnoId(-1),
			pacienteId,
			solicitudFechaCreacion,
			dispResult.UnwrapAsOk()
		);

		if (turnoResult is Result<Turno2025>.Error e2)
			return new Result<Turno2025>.Error(e2.Mensaje);

		Turno2025 turnoProvisorio = ((Result<Turno2025>.Ok)turnoResult).Valor;

		Result<TurnoId> insertResult = await funcInsertTurnoReturnId(turnoProvisorio);

		if (insertResult is Result<TurnoId>.Error e3)
			return new Result<Turno2025>.Error(
				$"Error al persistir el nuevo turno: {e3.Mensaje}"
			);

		TurnoId idReal = ((Result<TurnoId>.Ok)insertResult).Valor;

		return new Result<Turno2025>.Ok(
			turnoProvisorio with { Id = idReal }
		);
	}



	public static async Task<Result<Turno2025>> SolicitarReprogramacionALaPrimeraDisponibilidad(
		Turno2025 turnoOriginal,
		DateTime outcomeFecha,
		string outcomeComentario,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcSelectMedicosWhereEspecialidad,
		Func<MedicoId, DateTime, DateTime, IEnumerable<HorarioMedico2025>> funcSelectHorariosVigentesBetweenFechasWhereMedicoId,
		Func<MedicoId, DateTime, DateTime, IEnumerable<Turno2025>> funcSelectTurnosWhereMedicoIdBetweenFechas,
		Func<Turno2025, Task<Result<Unit>>> funcUpdateTurnoWhereId,
		Func<Turno2025, Task<Result<TurnoId>>> funcInsertTurnoReturnId
	) {
		Result<Turno2025> canceladoResult = turnoOriginal.SetOutcome(
			TurnoOutcomeEstado2025.Reprogramado,
			outcomeFecha,
			outcomeComentario
		);

		if (canceladoResult is Result<Turno2025>.Error e1)
			return new Result<Turno2025>.Error(e1.Mensaje);

		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)canceladoResult).Valor;


		Result<Unit> updateResult = await funcUpdateTurnoWhereId(turnoCancelado);

		if (updateResult is Result<Unit>.Error e2)
			return new Result<Turno2025>.Error(
				$"Error al persistir la cancelación del turno: {e2.Mensaje}"
			);


		Result<DisponibilidadEspecialidad2025> dispResult = ServiciosPrivados.EncontrarProximaDisponibilidad(
			turnoOriginal.Especialidad,
			outcomeFecha,
			funcSelectMedicosWhereEspecialidad,
			funcSelectHorariosVigentesBetweenFechasWhereMedicoId,
			funcSelectTurnosWhereMedicoIdBetweenFechas
		);

		if (dispResult is Result<DisponibilidadEspecialidad2025>.Error e3)
			return new Result<Turno2025>.Error(e3.Mensaje);

		DisponibilidadEspecialidad2025 disponibilidad = ((Result<DisponibilidadEspecialidad2025>.Ok)dispResult).Valor;


		Result<Turno2025> provResult = turnoCancelado.Reprogramar(
			disponibilidad,
			new TurnoId(-1) // placeholder hasta persistencia
		);

		if (provResult is Result<Turno2025>.Error e4)
			return new Result<Turno2025>.Error(e4.Mensaje);

		Turno2025 turnoProvisorio = ((Result<Turno2025>.Ok)provResult).Valor;


		Result<TurnoId> insertResult = await funcInsertTurnoReturnId(turnoProvisorio);

		if (insertResult is Result<TurnoId>.Error e5)
			return new Result<Turno2025>.Error(
				$"Error al persistir el nuevo turno reprogramado: {e5.Mensaje}"
			);

		TurnoId idReal = ((Result<TurnoId>.Ok)insertResult).Valor;


		return new Result<Turno2025>.Ok(
			turnoProvisorio with { Id = idReal }
		);
	}



	public static async Task<Result<Turno2025>> SolicitarCancelacion(
		Turno2025 turnoOriginal,
		DateTime outcomeFecha,
		string outcomeComentario,
		Func<Turno2025, Task<Result<Unit>>> funcUpdateTurnoWhereId
	) {
		// 1. Aplicar regla de dominio para cancelar
		Result<Turno2025> canceladoResult = turnoOriginal.SetOutcome(
			TurnoOutcomeEstado2025.Cancelado,
			outcomeFecha,
			outcomeComentario
		);

		if (canceladoResult is Result<Turno2025>.Error e1)
			return new Result<Turno2025>.Error(e1.Mensaje);

		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)canceladoResult).Valor;

		// 2. Guardar cambios (IO)
		Result<Unit> updateResult = await funcUpdateTurnoWhereId(turnoCancelado);

		if (updateResult is Result<Unit>.Error e2)
			return new Result<Turno2025>.Error(
				$"Error al persistir la cancelación del turno: {e2.Mensaje}"
			);

		return new Result<Turno2025>.Ok(turnoCancelado);
	}




}
