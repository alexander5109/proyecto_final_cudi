using Clinica.Dominio.Comun;
using Clinica.Dominio.Extentions;

namespace Clinica.Dominio.Entidades;

public readonly record struct Turno2025(
	Medico2025 Medico,
	Paciente2025 Paciente,
	DateTime FechaYHora,
	TimeSpan Duracion
){

	public static Result<Turno2025> Crear(
		Medico2025 medico,
		Paciente2025 paciente,
		DateTime fechaYHora,
		TimeSpan? duracion = null) {
		if (fechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("El turno no puede ser en el pasado.");

		var dur = duracion ?? TimeSpan.FromMinutes(40); // default razonable
		if (dur.TotalMinutes is < 10 or > 120)
			return new Result<Turno2025>.Error("La duración del turno no es razonable.");

		// Validar disponibilidad del médico (regla de dominio)
		bool disponible = medico.ListaHorarios.TienenDisponibilidad(fechaYHora, dur);

		if (!disponible)
			return new Result<Turno2025>.Error("El médico no atiende en ese horario.");

		return new Result<Turno2025>.Ok(new Turno2025(medico, paciente, fechaYHora, dur));
	}

}
