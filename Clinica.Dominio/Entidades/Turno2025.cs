using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;

public readonly record struct Turno2025(
	Medico2025 Medico,
	Paciente2025 Paciente,
	DateTime FechaYHora,
	TimeSpan Duracion
) {



	public static bool HorariosMedicosTienenDisponibilidad(IReadOnlyList<HorarioMedico> horarios, DateTime fechaYHora, TimeSpan duracion) {
		
		if (horarios is null || horarios.Count == 0)
			return false;

		var diaSemana = fechaYHora.DayOfWeek;
		var desde = TimeOnly.FromDateTime(fechaYHora);
		var hasta = desde.Add(duracion);

		// Buscar si hay algún horario que cubra completamente ese rango
		return horarios.Any(h =>
			h.DiaSemana == diaSemana &&
			h.Desde <= desde &&
			h.Hasta >= hasta
		);
	}



	public static Result<Turno2025> Crear(Medico2025 medico, Paciente2025 paciente, DateTime fechaYHora, TimeSpan? duracion = null) {
		if (fechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("El turno no puede ser en el pasado.");

		TimeSpan dur = duracion ?? TimeSpan.FromMinutes(40); // default
		if (dur.TotalMinutes is < 10 or > 120)
			return new Result<Turno2025>.Error("La duración del turno no es razonable.");

		// Validar disponibilidad del médico (regla de dominio compuesta)
		if (HorariosMedicosTienenDisponibilidad(medico.Horarios, fechaYHora, dur))
			return new Result<Turno2025>.Error("El médico no atiende en ese horario.");

		return new Result<Turno2025>.Ok(new Turno2025(medico, paciente, fechaYHora, dur));
	}

	public override string ToString() => $"Turno de {Paciente.NombreCompleto} con {Medico.NombreCompleto} el {FechaYHora:g}";
}
