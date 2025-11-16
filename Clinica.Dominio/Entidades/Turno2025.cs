using Clinica.Dominio.Comun;
using Clinica.Dominio.Extentions;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public enum TurnoEstado2025
{
	Programado,
	Reprogramado,
	Cancelado,
	Atendido
}

public readonly record struct Turno2025(
	Medico2025? MedicoAsignado,
	Paciente2025 Paciente,
	DateTime FechaYHora,
	MedicoEspecialidad2025 Especialidad,
	TurnoEstado2025 Estado
) {

	public static Result<Turno2025> Crear(Result<Medico2025> medicoResult, Result<Paciente2025> pacienteResult, Result<MedicoEspecialidad2025> especialidadResult, DateTime? fechaYHora) {
		// Validar entradas
		if (medicoResult is Result<Medico2025>.Error medicoError)
			return new Result<Turno2025>.Error($"Error en médico: {medicoError.Mensaje}");

		if (pacienteResult is Result<Paciente2025>.Error pacienteError)
			return new Result<Turno2025>.Error($"Error en paciente: {pacienteError.Mensaje}");

		if (especialidadResult is Result<MedicoEspecialidad2025>.Error espError)
			return new Result<Turno2025>.Error($"Error en especialidad: {espError.Mensaje}");

		var medico = ((Result<Medico2025>.Ok)medicoResult).Valor;
		var paciente = ((Result<Paciente2025>.Ok)pacienteResult).Valor;
		var especialidad = ((Result<MedicoEspecialidad2025>.Ok)especialidadResult).Valor;

		if (fechaYHora is null)
			return new Result<Turno2025>.Error("La fecha y hora del turno es obligatoria.");

		if (fechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("El turno no puede ser en el pasado.");

		var duracion = TimeSpan.FromMinutes(especialidad.DuracionConsultaMinutos);
		if (duracion.TotalMinutes is < 5 or > 240)
			return new Result<Turno2025>.Error("La duración derivada de la especialidad no es razonable.");

		// Validar disponibilidad del médico asignado según sus horarios
		bool disponible = medico.ListaHorarios.TienenDisponibilidad(fechaYHora.Value, duracion);
		if (!disponible)
			return new Result<Turno2025>.Error("El médico no atiende en ese horario.");

		return new Result<Turno2025>.Ok(new Turno2025(medico, paciente, fechaYHora.Value, especialidad, TurnoEstado2025.Programado));
	}

	public Result<Turno2025> Cancelar(string? motivo = null) {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("El turno ya está cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("No se puede cancelar un turno ya atendido.");
		return new Result<Turno2025>.Ok(this with { Estado = TurnoEstado2025.Cancelado });
	}

	public Result<Turno2025> MarcarComoAtendido() {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("No se puede marcar como atendido un turno cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("El turno ya está marcado como atendido.");
		if (FechaYHora > DateTime.Now.AddMinutes(15))
			return new Result<Turno2025>.Error("No se puede marcar como atendido antes de la hora programada (falta más de 15 minutos).");
		return new Result<Turno2025>.Ok(this with { Estado = TurnoEstado2025.Atendido });
	}

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

		if (!MedicoAsignado.Value.ListaHorarios.TienenDisponibilidad(nuevaFechaYHora, duracion))
			return new Result<Turno2025>.Error("El médico no atiende en el nuevo horario.");

		// Verificar solapamiento con otros turnos del mismo médico
		if (otrosTurnosDelMedico is not null) {
			var comienzo = nuevaFechaYHora;
			var fin = nuevaFechaYHora.Add(duracion);
			foreach (var ot in otrosTurnosDelMedico) {
				if (ot.MedicoAsignado is null) continue;
				if (ot.MedicoAsignado.Value.Dni == this.MedicoAsignado.Value.Dni && ot.FechaYHora == this.FechaYHora)
					continue; // ignorar propio

				if (ot.MedicoAsignado.Value.Dni == this.MedicoAsignado.Value.Dni) {
					var otherStart = ot.FechaYHora;
					var otherEnd = ot.FechaYHora.Add(TimeSpan.FromMinutes(ot.Especialidad.DuracionConsultaMinutos));
					if (comienzo < otherEnd && otherStart < fin)
						return new Result<Turno2025>.Error("El nuevo horario se solapa con otro turno del médico.");
				}
			}
		}

		return new Result<Turno2025>.Ok(this with { FechaYHora = nuevaFechaYHora, Estado = TurnoEstado2025.Reprogramado });
	}

}
