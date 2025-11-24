using System;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;

public enum TurnoOutcomeEstado2025 {
	Programado = 1,
	Ausente = 2,
	Cancelado = 3,
	Concretado = 4,
	Reprogramado = 5
}
public record struct TurnoId(int Valor);

public record Turno2025(
	TurnoId Id,
	FechaRegistro2025 FechaDeCreacion,
	PacienteId PacienteId,
	MedicoId MedicoId,
	EspecialidadMedica2025 Especialidad,
	DateTime FechaHoraAsignadaDesde,
	DateTime FechaHoraAsignadaHasta,
	TurnoOutcomeEstado2025 OutcomeEstado,
	Option<DateTime> OutcomeFecha,
	Option<string> OutcomeComentario
) : IComoTexto {

	public string ATexto() {
		var fecha = FechaHoraAsignadaDesde.ToString("dddd dd/MM/yyyy");
		var desde = FechaHoraAsignadaDesde.ToString("HH:mm");
		var hasta = FechaHoraAsignadaHasta.ToString("HH:mm");
		var duracion = (FechaHoraAsignadaHasta - FechaHoraAsignadaDesde).TotalMinutes;
		return
			$"Turno de {Especialidad.ATexto()}\n" +
			$"  • PacienteId: {PacienteId}\n" +
			$"  • Médico asignado: {MedicoId}\n" +
			$"  • Fecha: {fecha}\n" +
			$"  • Horario: {desde}–{hasta} ({duracion} min)\n" +
			$"  • OutcomeEstado: {OutcomeEstado}";
	}

	public static Result<Turno2025> Crear(
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
			FechaHoraAsignadaDesde: disp.FechaHoraDesde,
			FechaHoraAsignadaHasta: disp.FechaHoraHasta,
			OutcomeEstado: TurnoOutcomeEstado2025.Programado,
			OutcomeFecha: Option<DateTime>.None,
			OutcomeComentario: Option<string>.None
		));
	}

	public Result<Turno2025> SetOutcome(TurnoOutcomeEstado2025 outcomeEstado, DateTime outcomeFecha, string outcomeComentario) {
		if (OutcomeEstado != TurnoOutcomeEstado2025.Programado)
			return new Result<Turno2025>.Error("El turno ya tiene un estado final. No puede modificarse.");

		return new Result<Turno2025>.Ok(
			this with {
				OutcomeEstado = outcomeEstado,
				OutcomeComentario = Option<string>.Some(outcomeComentario),
				OutcomeFecha = Option<DateTime>.Some(outcomeFecha)
			}
		);
	}

	public Result<Turno2025> Reprogramar(
		DisponibilidadEspecialidad2025 nuevaDisp,
		TurnoId nuevoId
	) {
		if (OutcomeEstado == TurnoOutcomeEstado2025.Programado)
			return new Result<Turno2025>.Error("No puede reprogramarse un turno que todavía está programado.");

		if (!OutcomeFecha.HasValue)
			return new Result<Turno2025>.Error("No se puede reprogramar un turno sin fecha de finalización del estado anterior.");

		if (nuevaDisp.FechaHoraDesde >= nuevaDisp.FechaHoraHasta)
			return new Result<Turno2025>.Error("La disponibilidad nueva es inválida: fechaDesde >= fechaHasta.");

		return new Result<Turno2025>.Ok(new Turno2025(
			Id: nuevoId,
			FechaDeCreacion: new FechaRegistro2025(OutcomeFecha.Value),
			PacienteId: PacienteId,
			MedicoId: MedicoId,
			Especialidad: Especialidad,
			FechaHoraAsignadaDesde: nuevaDisp.FechaHoraDesde,
			FechaHoraAsignadaHasta: nuevaDisp.FechaHoraHasta,
			OutcomeEstado: TurnoOutcomeEstado2025.Programado,
			OutcomeFecha: Option<DateTime>.None,
			OutcomeComentario: Option<string>.None
		));
	}
}
