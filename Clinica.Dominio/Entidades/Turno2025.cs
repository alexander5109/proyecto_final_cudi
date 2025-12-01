using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;


public record struct TurnoId(int Valor) {
	public static Result<TurnoId> Crear(int? id) =>
		id is int idGood
		? new Result<TurnoId>.Ok(new TurnoId(idGood))
		: new Result<TurnoId>.Error("El id no puede ser nulo.");
	public static bool TryParse(string? s, out TurnoId id) {
		if (int.TryParse(s, out int value)) {
			id = new TurnoId(value);
			return true;
		}
		id = default;
		return false;
	}

	public override string ToString() {
		return Valor.ToString();
	}
}

public record Turno2025(
	TurnoId Id,
	FechaRegistro2025 FechaDeCreacion,
	PacienteId PacienteId,
	MedicoId MedicoId,
	EspecialidadMedica2025 Especialidad,
	DateTime FechaHoraAsignadaDesdeValor,
	DateTime FechaHoraAsignadaHastaValor,
	TurnoOutcomeEstado2025 OutcomeEstado,
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
			$"  • OutcomeEstado: {OutcomeEstado}";
	}
	public static Result<Turno2025> Crear(
		Result<TurnoId> idResult,
		Result<FechaRegistro2025> fechaCreacionResult,
		Result<PacienteId> pacienteIdResult,
		Result<MedicoId> medicoIdResult,
		Result<EspecialidadMedica2025> especialidadResult,
		DateTime desde,
		DateTime hasta,
		Result<TurnoOutcomeEstado2025> outcomeEstadoResult,
		Option<DateTime> outcomeFecha,
		Option<string> outcomeComentario
	) {
		// Componemos los VO dependientes
		return
			from id in idResult
			from fechaCreacion in fechaCreacionResult
			from pacienteId in pacienteIdResult
			from medicoId in medicoIdResult
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
			OutcomeEstado: TurnoOutcomeEstado2025.Programado,
			OutcomeFechaOption: Option<DateTime>.None,
			OutcomeComentarioOption: Option<string>.None
		));
	}

	public Result<Turno2025> SetOutcome(TurnoOutcomeEstado2025 outcomeEstado, DateTime outcomeFecha, string outcomeComentario) {
		if (OutcomeEstado != TurnoOutcomeEstado2025.Programado)
			return new Result<Turno2025>.Error("El turno ya tiene un estado final. No puede modificarse.");

		return new Result<Turno2025>.Ok(
			this with {
				OutcomeEstado = outcomeEstado,
				OutcomeComentarioOption = Option<string>.Some(outcomeComentario),
				OutcomeFechaOption = Option<DateTime>.Some(outcomeFecha)
			}
		);
	}

	public Result<Turno2025> Reprogramar(
		DisponibilidadEspecialidad2025 nuevaDisp,
		TurnoId nuevoId
	) {
		if (OutcomeEstado == TurnoOutcomeEstado2025.Programado)
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
			OutcomeEstado: TurnoOutcomeEstado2025.Programado,
			OutcomeFechaOption: Option<DateTime>.None,
			OutcomeComentarioOption: Option<string>.None
		));
	}
}
