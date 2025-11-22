using Clinica.Dominio.Comun;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;


public enum TurnoOutcomeEstado2025 {
	Programado = 1,
	Ausente = 2,
	Cancelado = 3,
	Concretado = 4,
	//Reprogramado = 5
}
public record struct TurnoId(int Value);
public record Turno2025(
	TurnoId Id,
	DateTime FechaDeCreacion,
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
		Result<SolicitudDeTurno> solicitudResult,
		Result<DisponibilidadEspecialidad2025> dispResult
	) {
		// --- Validación de los Result ----
		if (solicitudResult is Result<SolicitudDeTurno>.Error solError)
			return new Result<Turno2025>.Error($"Error en solicitud: {solError.Mensaje}");

		if (dispResult is Result<DisponibilidadEspecialidad2025>.Error dispError)
			return new Result<Turno2025>.Error($"Error en disponibilidad: {dispError.Mensaje}");

		SolicitudDeTurno solicitud = ((Result<SolicitudDeTurno>.Ok)solicitudResult).Valor;
		DisponibilidadEspecialidad2025 disp = ((Result<DisponibilidadEspecialidad2025>.Ok)dispResult).Valor;

		return new Result<Turno2025>.Ok(new Turno2025(
			Id: turnoId,
			FechaDeCreacion: solicitud.FechaCreacion,
			PacienteId: solicitud.PacienteId,
			MedicoId: disp.MedicoId,
			Especialidad: disp.Especialidad,
			FechaHoraAsignadaDesde: disp.FechaHoraDesde,
			FechaHoraAsignadaHasta: disp.FechaHoraHasta,
			OutcomeEstado: TurnoOutcomeEstado2025.Programado,
			OutcomeFecha: Option<DateTime>.None,
			OutcomeComentario: Option<string>.None
		));
	}

	public Result<Turno2025> CambiarEstado(TurnoOutcomeEstado2025 outcomeEstado, Option<DateTime> outcomeFecha, Option<string> outcomeComentario) {
		if (OutcomeEstado != TurnoOutcomeEstado2025.Programado) {
			return new Result<Turno2025>.Error("No se puede volver a cambiar el estado del turno.");
		}
		return new Result<Turno2025>.Ok(this with { OutcomeEstado = outcomeEstado, OutcomeComentario = outcomeComentario, OutcomeFecha = outcomeFecha });
	}



	//public static Result<Turno2025> ReprogramarDesde(
	//	Turno2025 turnoOriginal,
	//	DisponibilidadEspecialidad2025 nuevaDisp
	//) {
	//	return new Result<Turno2025>.Ok(new Turno2025(
	//		Guid: Guid.NewGuid(),
	//		FechaDeCreacion: DateTime.Now,
	//		PacienteId: turnoOriginal.PacienteId,
	//		MedicoId: nuevaDisp.Medico,
	//		Especialidad: nuevaDisp.Especialidad,
	//		FechaHoraAsignadaDesde: nuevaDisp.FechaHoraDesde,
	//		FechaHoraAsignadaHasta: nuevaDisp.FechaHoraHasta,
	//		OutcomeEstado: TurnoOutcomeEstado2025.Programado,
	//		OutcomeFecha: Option<DateTime>.None,
	//		OutcomeComentario: Option<string>.None
	//	));
	//}

}
