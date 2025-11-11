using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public record class Turno2025(
	Medico2025 Medico,
	Paciente2025 Paciente,
	DateTime FechaYHora,
	TimeSpan Duracion
) : IValidate<Contacto2025> {
	public static Result<Turno2025> Crear(Medico2025 medico, Paciente2025 paciente, DateTime fechaYHora, TimeSpan? duracion = null) {
		if (fechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("El turno no puede ser en el pasado.");

		TimeSpan dur = duracion ?? TimeSpan.FromMinutes(40);
		if (dur.TotalMinutes is < 10 or > 120)
			return new Result<Turno2025>.Error("La duración del turno no es razonable.");

		// Validar disponibilidad del médico (regla de dominio compuesta)
		if (!medico.Agenda.EstaDisponibleEn(fechaYHora, dur))
			return new Result<Turno2025>.Error("El médico no atiende en ese horario.");

		return new Result<Turno2025>.Ok(new Turno2025(medico, paciente, fechaYHora, dur));
	}

	public override string ToString() =>
		$"Turno de {Paciente.NombreCompleto} con {Medico.Nombre} el {FechaYHora:g}";
}
