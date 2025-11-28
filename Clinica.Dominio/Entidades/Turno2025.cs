using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;


public record struct TurnoId(int Valor);

public record Turno2025(
	TurnoId Id,
	FechaRegistro2025 FechaDeCreacion,
	PacienteId PacienteId,
	MedicoId MedicoId,
	EspecialidadMedica2025 Especialidad,
	DateTime FechaHoraAsignadaDesdeValor,
	DateTime FechaHoraAsignadaHastaValor,
	TurnoOutcomeEstado2025 OutcomeEstadoOption,
	Option<DateTime> OutcomeFechaOption,
	Option<string> OutcomeComentarioOption
) : IComoTexto {

	public string ATexto() {
		var fecha = FechaHoraAsignadaDesdeValor.ToString("dddd dd/MM/yyyy");
		var desde = FechaHoraAsignadaDesdeValor.ToString("HH:mm");
		var hasta = FechaHoraAsignadaHastaValor.ToString("HH:mm");
		var duracion = (FechaHoraAsignadaHastaValor - FechaHoraAsignadaDesdeValor).TotalMinutes;
		return
			$"Turno de {Especialidad.ATexto()}\n" +
			$"  • PacienteId: {PacienteId}\n" +
			$"  • Médico asignado: {MedicoId}\n" +
			$"  • Fecha: {fecha}\n" +
			$"  • Horario: {desde}–{hasta} ({duracion} min)\n" +
			$"  • OutcomeEstadoOption: {OutcomeEstadoOption}";
	}
	public static Result<Turno2025> Crear(
		TurnoId id,
		Result<FechaRegistro2025> fechaCreacionResult,
		PacienteId pacienteId,
		MedicoId medicoId,
		Result<EspecialidadMedica2025> especialidadResult,
		DateTime desde,
		DateTime hasta,
		Result<TurnoOutcomeEstado2025> outcomeEstadoResult,
		Option<DateTime> outcomeFecha,
		Option<string> outcomeComentario
	) {
		// Componemos los VO dependientes
		return
			from fechaCreacion in fechaCreacionResult
			from especialidad in especialidadResult
			from estado in outcomeEstadoResult
			from _ in ValidarOutcome(estado, outcomeFecha, outcomeComentario)
			select new Turno2025(
				id,
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
	private static Result<Unit> ValidarOutcome(TurnoOutcomeEstado2025 estado, Option<DateTime> fecha, Option<string> comentario) {
		bool esProgramado = estado == TurnoOutcomeEstado2025.Programado;

		if (esProgramado) {
			if (fecha.HasValor || comentario.HasValor)
				return new Result<Unit>.Error(
					"Un turno Programado no puede tener Fecha ni Comentario."
				);

			return new Result<Unit>.Ok(Unit.Valor);
		}

		// Estado es Ausente, Cancelado, Concretado, Reprogramado
		if (fecha.HasValor || comentario.HasValor)
			return new Result<Unit>.Error(
				"Los turnos NO programados deben tener Fecha y Comentario."
			);

		return new Result<Unit>.Ok(Unit.Valor);
	}


	public static Result<Turno2025> ProgramarNuevo(
		TurnoId turnoId,
		PacienteId pacienteId,
		FechaRegistro2025 solicitadoEn,
		DisponibilidadEspecialidad2025 disp
	) {
		if (disp.FechaHoraDesde >= disp.FechaHoraHasta)
			return new Result<Turno2025>.Error(
				"La hora de inicio debe ser anterior a la hora de fin."
			);

		if (disp.FechaHoraDesde.Date != disp.FechaHoraHasta.Date)
			return new Result<Turno2025>.Error(
				"Un turno no puede extenderse entre dos días distintos."
			);

		return new Result<Turno2025>.Ok(new Turno2025(
			Id: turnoId,
			FechaDeCreacion: solicitadoEn,
			PacienteId: pacienteId,
			MedicoId: disp.MedicoId,
			Especialidad: disp.Especialidad,
			FechaHoraAsignadaDesdeValor: disp.FechaHoraDesde,
			FechaHoraAsignadaHastaValor: disp.FechaHoraHasta,
			OutcomeEstadoOption: TurnoOutcomeEstado2025.Programado,
			OutcomeFechaOption: Option<DateTime>.None,
			OutcomeComentarioOption: Option<string>.None
		));
	}

	public Result<Turno2025> SetOutcome(TurnoOutcomeEstado2025 outcomeEstado, DateTime outcomeFecha, string outcomeComentario) {
		if (OutcomeEstadoOption != TurnoOutcomeEstado2025.Programado)
			return new Result<Turno2025>.Error("El turno ya tiene un estado final. No puede modificarse.");

		return new Result<Turno2025>.Ok(
			this with {
				OutcomeEstadoOption = outcomeEstado,
				OutcomeComentarioOption = Option<string>.Some(outcomeComentario),
				OutcomeFechaOption = Option<DateTime>.Some(outcomeFecha)
			}
		);
	}

	public Result<Turno2025> Reprogramar(
		DisponibilidadEspecialidad2025 nuevaDisp,
		TurnoId nuevoId
	) {
		if (OutcomeEstadoOption == TurnoOutcomeEstado2025.Programado)
			return new Result<Turno2025>.Error("No puede reprogramarse un turno que todavía está programado.");

		if (!OutcomeFechaOption.HasValor)
			return new Result<Turno2025>.Error("No se puede reprogramar un turno sin fecha de finalización del estado anterior.");

		if (nuevaDisp.FechaHoraDesde >= nuevaDisp.FechaHoraHasta)
			return new Result<Turno2025>.Error("La disponibilidad nueva es inválida: fechaDesde >= fechaHasta.");

		return new Result<Turno2025>.Ok(new Turno2025(
			Id: nuevoId,
			FechaDeCreacion: new FechaRegistro2025(OutcomeFechaOption.Valor),
			PacienteId: PacienteId,
			MedicoId: MedicoId,
			Especialidad: Especialidad,
			FechaHoraAsignadaDesdeValor: nuevaDisp.FechaHoraDesde,
			FechaHoraAsignadaHastaValor: nuevaDisp.FechaHoraHasta,
			OutcomeEstadoOption: TurnoOutcomeEstado2025.Programado,
			OutcomeFechaOption: Option<DateTime>.None,
			OutcomeComentarioOption: Option<string>.None
		));
	}
}
