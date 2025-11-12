using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public readonly record struct TurnoType(
	MedicoType Medico,
	PacienteType Paciente,
	DateTime FechaYHora,
	TimeSpan Duracion
);

public static class Turno2025 {
	public static bool HorariosMedicosTienenDisponibilidad(
		IReadOnlyList<HorarioMedicoType> horarios,
		DateTime fechaYHora,
		TimeSpan duracion) {
		if (horarios is null || horarios.Count == 0)
			return false;

		var diaSemana = new HorarioDiaSemanaType(fechaYHora.DayOfWeek);
		var desde = new HorarioHoraType(TimeOnly.FromDateTime(fechaYHora));
		var hasta = new HorarioHoraType(desde.Value.Add(duracion));

		// Hay disponibilidad si existe al menos un horario que cubra ese rango
		return horarios.Any(h =>
			h.DiaSemana == diaSemana &&
			h.Desde.Value <= desde.Value &&
			h.Hasta.Value >= hasta.Value
		);
	}

	public static Result<TurnoType> Create(
		MedicoType medico,
		PacienteType paciente,
		DateTime fechaYHora,
		TimeSpan? duracion = null) {
		if (fechaYHora < DateTime.Now)
			return new Result<TurnoType>.Error("El turno no puede ser en el pasado.");

		var dur = duracion ?? TimeSpan.FromMinutes(40); // default razonable
		if (dur.TotalMinutes is < 10 or > 120)
			return new Result<TurnoType>.Error("La duración del turno no es razonable.");

		// Validar disponibilidad del médico (regla de dominio)
		bool disponible = HorariosMedicosTienenDisponibilidad(medico.Horarios, fechaYHora, dur);

		if (!disponible)
			return new Result<TurnoType>.Error("El médico no atiende en ese horario.");

		return new Result<TurnoType>.Ok(new TurnoType(medico, paciente, fechaYHora, dur));
	}

	public static string AString(this TurnoType turno)
		=> $"Turno de {turno.Paciente.NombreCompleto} con {turno.Medico.NombreCompleto} el {turno.FechaYHora:g}";
}
