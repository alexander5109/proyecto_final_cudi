using Clinica.Dominio.Comun;
using Clinica.Dominio.Extentions;

namespace Clinica.Dominio.Entidades;

public enum TurnoEstado2025
{
	Programado,
	Reprogramado,
	Cancelado,
	Atendido
}

public readonly record struct Turno2025(
	Medico2025 Medico,
	Paciente2025 Paciente,
	DateTime FechaYHora,
	TimeSpan DuracionMinutos,
	TurnoEstado2025 Estado
) {

	public static Result<Turno2025> SacarTurno(Result<Medico2025> medicoResult, Result<Paciente2025> pacienteResult, DateTime? fechaYHora, int? duracionMinutos) {
		// 🟡 Validar resultados previos (early return)
		if (medicoResult is Result<Medico2025>.Error medicoError)
			return new Result<Turno2025>.Error($"Error en médico: {medicoError.Mensaje}");

		if (pacienteResult is Result<Paciente2025>.Error pacienteError)
			return new Result<Turno2025>.Error($"Error en paciente: {pacienteError.Mensaje}");

		// 🟢 Desempaquetar entidades válidas
		var medico = ((Result<Medico2025>.Ok)medicoResult).Valor;
		var paciente = ((Result<Paciente2025>.Ok)pacienteResult).Valor;

		// 🕒 Validar fecha
		if (fechaYHora is null)
			return new Result<Turno2025>.Error("La fecha y hora del turno es obligatoria.");

		if (fechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("El turno no puede ser en el pasado.");

		// ⏱️ Duración
		var duracion = TimeSpan.FromMinutes(duracionMinutos ?? 40);

		if (duracion.TotalMinutes is < 10 or > 120)
			return new Result<Turno2025>.Error("La duración del turno no es razonable (10–120 min).");

		// 👩‍⚕️ Validar disponibilidad del médico
		bool disponible = medico.ListaHorarios.TienenDisponibilidad(fechaYHora.Value, duracion);
		if (!disponible)
			return new Result<Turno2025>.Error("El médico no atiende en ese horario.");

		// ✅ Si todo está bien
		return new Result<Turno2025>.Ok(new Turno2025(medico, paciente, fechaYHora.Value, duracion, TurnoEstado2025.Programado));
	}

	// Acciones de dominio (métodos de instancia que retornan una nueva instancia validada)

	public Result<Turno2025> Cancelar(string? motivo = null) {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("El turno ya está cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("No se puede cancelar un turno ya atendido.");

		// Podríamos validar reglas adicionales (p. ej. antelación mínima para cancelar)
		return new Result<Turno2025>.Ok(this with { Estado = TurnoEstado2025.Cancelado });
	}

	public Result<Turno2025> MarcarComoAtendido() {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("No se puede marcar como atendido un turno cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("El turno ya está marcado como atendido.");

		// Sólo permitir marcar como atendido si la fecha ya ocurrió (o está en curso)
		if (FechaYHora > DateTime.Now.AddMinutes(15))
			return new Result<Turno2025>.Error("No se puede marcar como atendido antes de la hora programada (falta más de 15 minutos).");

		return new Result<Turno2025>.Ok(this with { Estado = TurnoEstado2025.Atendido });
	}

	public Result<Turno2025> Reprogramar(DateTime nuevaFechaYHora, int? nuevaDuracionMinutos, IEnumerable<Turno2025>? otrosTurnosDelMedico = null) {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("No se puede reprogramar un turno cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("No se puede reprogramar un turno ya atendido.");

		if (nuevaFechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("La nueva fecha no puede ser en el pasado.");

		var duracion = TimeSpan.FromMinutes(nuevaDuracionMinutos ?? (int)DuracionMinutos.TotalMinutes);
		if (duracion.TotalMinutes is < 10 or > 120)
			return new Result<Turno2025>.Error("La duración del turno no es razonable (10–120 min).");

		// Disponibilidad del médico según sus horarios
		if (!Medico.ListaHorarios.TienenDisponibilidad(nuevaFechaYHora, duracion))
			return new Result<Turno2025>.Error("El médico no atiende en el nuevo horario.");

		// Verificar solapamiento con otros turnos del mismo médico
		if (otrosTurnosDelMedico is not null) {
			var comienzo = nuevaFechaYHora;
			var fin = nuevaFechaYHora.Add(duracion);
			foreach (var ot in otrosTurnosDelMedico) {
				// Ignorar el propio turno
				if (ot.Medico.Dni == this.Medico.Dni && ot.Paciente.Dni == this.Paciente.Dni && ot.FechaYHora == this.FechaYHora && ot.DuracionMinutos == this.DuracionMinutos)
					continue;

				if (ot.Medico.Dni == this.Medico.Dni) {
					var otherStart = ot.FechaYHora;
					var otherEnd = ot.FechaYHora.Add(ot.DuracionMinutos);
					if (comienzo < otherEnd && otherStart < fin)
						return new Result<Turno2025>.Error("El nuevo horario se solapa con otro turno del médico.");
				}
			}
		}

		return new Result<Turno2025>.Ok(this with { FechaYHora = nuevaFechaYHora, DuracionMinutos = duracion, Estado = TurnoEstado2025.Reprogramado });
	}

}
