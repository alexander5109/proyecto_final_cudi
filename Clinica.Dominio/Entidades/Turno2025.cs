using Clinica.Dominio.FunctionalProgramingTools;
using static Clinica.Dominio.Entidades.Entidades;

namespace Clinica.Dominio.Entidades;


public enum TurnoEstado2025 {
	Programado,
	Reprogramado,
	Cancelado,
	Atendido
}
public record Turno2025(
	Medico2025 MedicoAsignado,
	Paciente2025 Paciente,
	EspecialidadMedica2025 Especialidad,
	DateTime FechaHoraDesde,
	DateTime FechaHoraHasta,
	TurnoEstado2025 Estado
) {

	public static Result<Turno2025> Programar(
		SolicitudDeTurno solicitud,
		DisponibilidadEspecialidad2025 disp
	) {
		// --- Validación de los Result ----
		//if (solicitudResult is Result<SolicitudDeTurno>.Error solError)
		//	return new Result<Turno2025>.Error($"Error en solicitud: {solError.Mensaje}");

		//if (dispResult is Result<DisponibilidadEspecialidad2025>.Error dispError)
		//	return new Result<Turno2025>.Error($"Error en disponibilidad: {dispError.Mensaje}");

		//var solicitud = ((Result<SolicitudDeTurno>.Ok)solicitudResult).Valor;
		//var disp = ((Result<DisponibilidadEspecialidad2025>.Ok)dispResult).Valor;

		// --- Coherencias de dominio ---
		if (solicitud.Paciente is null)
			return new Result<Turno2025>.Error("La solicitud no tiene paciente.");

		if (solicitud.Especialidad.CodigoInterno != disp.Especialidad.CodigoInterno)
			return new Result<Turno2025>.Error("La disponibilidad no corresponde a la especialidad solicitada.");

		// El turno NO tiene por qué ser validado contra DateTime.Now.
		// Asumimos que la disponibilidad ya fue generada correctamente por el dominio.

		// --- Construcción del Turno ---
		var turno = new Turno2025(
			MedicoAsignado: disp.Medico,
			Paciente: solicitud.Paciente,
			Especialidad: disp.Especialidad,
			FechaHoraDesde: disp.FechaHoraDesde,
			FechaHoraHasta: disp.FechaHoraHasta,
			Estado: TurnoEstado2025.Programado
		);

		return new Result<Turno2025>.Ok(turno);
	}


	public Result<Turno2025> Cancelar(string? motivo = null) {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("El turno ya está cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("No se puede cancelar un turno ya atendido.");
		return new Result<Turno2025>.Ok(this with { Estado = TurnoEstado2025.Cancelado });
	}

	//public Result<Turno2025> MarcarComoAtendido() {
	//	if (Estado == TurnoEstado2025.Cancelado)
	//		return new Result<Turno2025>.Error("No se puede marcar como atendido un turno cancelado.");
	//	if (Estado == TurnoEstado2025.Atendido)
	//		return new Result<Turno2025>.Error("El turno ya está marcado como atendido.");
	//	if (FechaYHora > DateTime.Now.AddMinutes(15))
	//		return new Result<Turno2025>.Error("No se puede marcar como atendido antes de la hora programada (falta más de 15 minutos).");
	//	return new Result<Turno2025>.Ok(this with { Estado = TurnoEstado2025.Atendido });
	//}
	/*
	public Result<Turno2025> Reprogramar(DateTime nuevaFechaYHora, IEnumerable<Turno2025>? otrosTurnosDelMedico = null) {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("No se puede reprogramar un turno cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("No se puede reprogramar un turno ya atendido.");

		if (nuevaFechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("La nueva fecha no puede ser en el pasado.");

		var duracion = TimeSpan.FromMinutes(Especialidad.DuracionConsultaMinutos);
		if (duracion.TotalMinutes is < 5 or > 240)
			return new Result<Turno2025>.Error("La duración derivada de la especialidad no es razonable.");

		// Disponibilidad del médico asignado según sus horarios
		if (MedicoAsignado is null)
			return new Result<Turno2025>.Error("No hay médico asignado al turno para verificar disponibilidad.");

		if (!MedicoAsignado.ListaHorarios.TienenDisponibilidad(nuevaFechaYHora, duracion))
			return new Result<Turno2025>.Error("El médico no atiende en el nuevo horario.");

		// Verificar solapamiento con otros turnos del mismo médico
		if (otrosTurnosDelMedico is not null) {
			var comienzo = nuevaFechaYHora;
			var fin = nuevaFechaYHora.Add(duracion);
			foreach (var ot in otrosTurnosDelMedico) {
				if (ot.MedicoAsignado is null) continue;
				if (ot.MedicoAsignado.Dni == this.MedicoAsignado.Dni && ot.FechaYHora == this.FechaYHora)
					continue; // ignorar propio

				if (ot.MedicoAsignado.Dni == this.MedicoAsignado.Dni) {
					var otherStart = ot.FechaYHora;
					var otherEnd = ot.FechaYHora.Add(TimeSpan.FromMinutes(ot.Especialidad.DuracionConsultaMinutos));
					if (comienzo < otherEnd && otherStart < fin)
						return new Result<Turno2025>.Error("El nuevo horario se solapa con otro turno del médico.");
				}
			}
		}

		return new Result<Turno2025>.Ok(this with { FechaYHora = nuevaFechaYHora, Estado = TurnoEstado2025.Reprogramado });
	}
	*/
	public record DisponibilidadSlot(DateTime FechaHora, int? MedicoId, string MedicoDisplay);
}
