using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.TiposDeEntidad;


public static class TurnoPolicyRaw {
	public static bool PuedeMarcarComoAusente(
		TurnoEstadoCodigo estado,
		DateTime fechaAsignadaHasta,
		bool tieneOutcome,
		DateTime ahora
	) {
		return estado == TurnoEstadoCodigo.Programado
			&& !tieneOutcome
			&& ahora >= fechaAsignadaHasta;
	}

	public static bool PuedeConfirmar(
		TurnoEstadoCodigo estado,
		DateTime fechaAsignadaDesde,
		bool tieneOutcome,
		DateTime ahora
	) {
		return estado == TurnoEstadoCodigo.Programado
			&& !tieneOutcome
			&& ahora.Date >= fechaAsignadaDesde.Date;
	}

	public static bool PuedeCancelar(
		TurnoEstadoCodigo estado,
		DateTime fechaAsignadaDesde,
		bool tieneOutcome,
		DateTime ahora
	) {
		return estado == TurnoEstadoCodigo.Programado
			&& !tieneOutcome
			&& ahora.Date < fechaAsignadaDesde.Date;
	}

	public static bool PuedeReprogramar(
		TurnoEstadoCodigo estado,
		DateTime fechaAsignadaHasta,
		bool tieneOutcome,
		DateTime ahora
	) {
		return estado == TurnoEstadoCodigo.Programado
			&& !tieneOutcome
			&& ahora < fechaAsignadaHasta;
	}
}



public record Turno2025(
	DateTime FechaDeCreacion,
	PacienteId PacienteId,
	MedicoId MedicoId,
	Especialidad2025 Especialidad,
	DateTime FechaHoraAsignadaDesdeValor,
	DateTime FechaHoraAsignadaHastaValor,
	TurnoEstadoCodigo OutcomeEstado,
	Option<DateTime> OutcomeFechaOption,
	Option<string> OutcomeComentarioOption
) : IComoTexto {

	public string ATexto() {
		string fecha = FechaHoraAsignadaDesdeValor.ToString("dddd dd/MM/yyyy");
		string desde = FechaHoraAsignadaDesdeValor.ToString("HH:mm");
		string hasta = FechaHoraAsignadaHastaValor.ToString("HH:mm");
		double duracion = (FechaHoraAsignadaHastaValor - FechaHoraAsignadaDesdeValor).TotalMinutes;
		return
			$"Turno de {Especialidad.ATexto()}\n" +
			$"  • PacienteId: {PacienteId}\n" +
			$"  • Médico asignado: {MedicoId}\n" +
			$"  • Fecha: {fecha}\n" +
			$"  • Horario: {desde}–{hasta} ({duracion} min)\n" +
			$"  • OutcomeEstado: {OutcomeEstado}\n";
	}

	public static Turno2025 Representar(
		DateTime fechaCreacion,
		PacienteId pacienteId,
		MedicoId medicoId,
		EspecialidadCodigo especialidadCododigo,
		DateTime desde,
		DateTime hasta,
		TurnoEstadoCodigo outcomeEstado,
		DateTime? outcomeFecha,
		string? outcomeComentario
	) {
		return new(
			FechaDeCreacion: fechaCreacion,
			PacienteId: pacienteId,
			MedicoId: medicoId,
			Especialidad: Especialidad2025.Representar(especialidadCododigo),
			FechaHoraAsignadaDesdeValor: desde,
			FechaHoraAsignadaHastaValor: hasta,
			OutcomeEstado: outcomeEstado,
			OutcomeFechaOption: outcomeFecha is DateTime fechagud ? Option<DateTime>.Some(fechagud) : Option<DateTime>.None,
			OutcomeComentarioOption: outcomeComentario is null ? Option<string>.None : Option<string>.Some(outcomeComentario)
		);
	}


	public static Result<Turno2025> CrearResult(
		//Result<TurnoId> idResult,
		DateTime fechaCreacion,
		Result<PacienteId> pacienteIdResult,
		Result<MedicoId> medicoIdResult,
		Result<Especialidad2025> especialidadResult,
		DateTime desde,
		DateTime hasta,
		Result<TurnoEstadoCodigo> outcomeEstadoResult,
		Option<DateTime> outcomeFecha,
		Option<string> outcomeComentario
	) {
		return
			//from id in idResult
			from pacienteId in pacienteIdResult
			from medicoId in medicoIdResult
			from especialidad in especialidadResult
			from estado in outcomeEstadoResult
			select new Turno2025(
				//id,
				fechaCreacion,
				pacienteId,
				medicoId,
				especialidad,
				desde,
				hasta,
				estado,
				outcomeFecha,
				outcomeComentario
			);
	}



	public static Result<Turno2025> Programar(
		PacienteId pacienteId,
		DateTime solicitadoEn,
		Disponibilidad2025 disp
	) {

		if (disp.FechaHoraDesde >= disp.FechaHoraHasta)
			return new Result<Turno2025>.Error(
				"La hora de inicio debe ser anterior a la hora de fin."
			);

		if (disp.FechaHoraDesde < solicitadoEn)
			return new Result<Turno2025>.Error("No puede programarse un turno para una fecha pasada.");


		//we cant check this unless Disponibilidad2025 richness includes more doctor info. So lLet's trust the Disponibilidad2025 generator system
		//if (disp.Medico.EspecialidadUnica != disp.Especialidad)
		//	return new Result<Turno2025>.Error($"El medico {disp.Medico.NombreCompleto.ATextoDia()} tentativo no es especialidad en {disp.Especialidad.ATextoDia()}");

		if (disp.FechaHoraDesde.Date != disp.FechaHoraHasta.Date)
			return new Result<Turno2025>.Error(
				"Un turno no puede extenderse entre dos días distintos."
			);

		return new Result<Turno2025>.Ok(new Turno2025(
			//Id: turnoId,
			FechaDeCreacion: solicitadoEn,
			PacienteId: pacienteId,
			MedicoId: disp.MedicoId,
			Especialidad: Especialidad2025.Representar(disp.EspecialidadCodigo),
			FechaHoraAsignadaDesdeValor: disp.FechaHoraDesde,
			FechaHoraAsignadaHastaValor: disp.FechaHoraHasta,
			OutcomeEstado: TurnoEstadoCodigo.Programado,
			OutcomeFechaOption: Option<DateTime>.None,
			OutcomeComentarioOption: Option<string>.None
		));
	}





}






public static class TurnoExtentions {

	public static Result<Turno2025> MarcarComoAusente(
		this Turno2025 turnoOriginal,
		DateTime fechaEvento,
		string? comentario
	) {
		if (comentario is null)
			return new Result<Turno2025>.Error("El comentario es obligatorio al marcar como ausente un turno.");

		if (turnoOriginal.OutcomeEstado != TurnoEstadoCodigo.Programado)
			return new Result<Turno2025>.Error("Solo puede marcarse como ausente un turno que esté programado.");


		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				OutcomeEstado = TurnoEstadoCodigo.Ausente,
				OutcomeFechaOption = Option<DateTime>.Some(fechaEvento),
				OutcomeComentarioOption = Option<string>.Some(comentario)
			}
		);
	}

	public static Result<Turno2025> MarcarComoConcretado(
		this Turno2025 turnoOriginal,
		DateTime fechaEvento,
		string? comentario //INTENDED TO BE OPTIONAL
	) {

		if (turnoOriginal.OutcomeEstado != TurnoEstadoCodigo.Programado)
			return new Result<Turno2025>.Error("Solo puede concretarse un turno que esté programado.");

		if (fechaEvento.Date < turnoOriginal.FechaHoraAsignadaDesdeValor.Date)
			return new Result<Turno2025>.Error("No puede confirmarse un turno antes de la fecha de la cita");

		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				OutcomeEstado = TurnoEstadoCodigo.Concretado,
				OutcomeFechaOption = Option<DateTime>.Some(fechaEvento),
				OutcomeComentarioOption = comentario is null ? Option<string>.None : Option<string>.Some(comentario)
			}
		);
	}
	public static Result<Turno2025> MarcarComoCancelado(
		this Turno2025 turnoOriginal,
		DateTime fechaEvento,
		string? comentario
	) {
		if (comentario is null)
			return new Result<Turno2025>.Error("El comentario es obligatorio para cancelar un turno.");

		if (turnoOriginal.OutcomeEstado != TurnoEstadoCodigo.Programado)
			return new Result<Turno2025>.Error("Solo puede cancelarse un turno que esté programado.");

		if (turnoOriginal.OutcomeFechaOption.HasValor)
			return new Result<Turno2025>.Error("El turno ya tiene un resultado asignado y no puede cancelarse.");

		if (fechaEvento < turnoOriginal.FechaDeCreacion)
			return new Result<Turno2025>.Error("La fecha del evento no puede ser anterior a la fecha de creación del turno.");

		if (fechaEvento >= turnoOriginal.FechaHoraAsignadaHastaValor)
			return new Result<Turno2025>.Error("No puede cancelarse un turno después de la cita.");

		if (fechaEvento.Date == turnoOriginal.FechaHoraAsignadaDesdeValor.Date)
			return new Result<Turno2025>.Error("No puede cancelarse el mismo dia de la cita. Por favor marcar como ausente por no avisar antes.");

		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				OutcomeEstado = TurnoEstadoCodigo.Cancelado,
				OutcomeFechaOption = Option<DateTime>.Some(fechaEvento),
				OutcomeComentarioOption = Option<string>.Some(comentario)
			}
		);
	}

	public static Result<Turno2025> MarcarComoReprogramado(
		this Turno2025 turnoOriginal,
		DateTime fechaEvento,
		string? comentario
	) {
		if (comentario is null)
			return new Result<Turno2025>.Error("El comentario es obligatorio al marcar querer reprogramar un turno.");

		if (turnoOriginal.OutcomeEstado != TurnoEstadoCodigo.Programado)
			return new Result<Turno2025>.Error("Solo puede reprogramarse un turno que esté programado.");

		if (turnoOriginal.OutcomeFechaOption.HasValor)
			return new Result<Turno2025>.Error("El turno ya tiene un resultado asignado y no puede reprogramarse.");

		if (fechaEvento < turnoOriginal.FechaDeCreacion)
			return new Result<Turno2025>.Error("La reprogramación no puede registrarse antes de que el turno haya sido creado.");


		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				OutcomeEstado = TurnoEstadoCodigo.Reprogramado,
				OutcomeFechaOption = Option<DateTime>.Some(fechaEvento),
				OutcomeComentarioOption = Option<string>.Some(comentario)
			}
		);
	}





}