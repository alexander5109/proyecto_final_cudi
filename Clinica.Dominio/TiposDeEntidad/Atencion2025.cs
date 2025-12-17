using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeEntidad;

public record Atencion2025(
	PacienteId2025 PacienteId,
	MedicoId2025 MedicoId,
	TurnoId2025 TurnoId,
	DateTime Fecha,
	string Observaciones
) {
	// Método estático para validar la creación del objeto Atencion2025
	public static Result<Atencion2025> CrearResult(PacienteId2025 pacienteId, MedicoId2025 medicoId, TurnoId2025 turnoId, DateTime fecha, string? observaciones) {
		// Validación de las IDs
		if (pacienteId == default)
			return new Result<Atencion2025>.Error("El ID del paciente no puede ser nulo o default.");
		if (medicoId == default)
			return new Result<Atencion2025>.Error("El ID del médico no puede ser nulo o default.");
		if (turnoId == default)
			return new Result<Atencion2025>.Error("El ID del turno no puede ser nulo o default.");
		if (fecha == default)
			return new Result<Atencion2025>.Error("La fecha de la observación no es valida");

		// Validación de las observaciones
		if (string.IsNullOrWhiteSpace(observaciones))
			return new Result<Atencion2025>.Error("Las observaciones son obligatorias.");
		if (observaciones.Length <= 50)
			return new Result<Atencion2025>.Error("Las observaciones deben tener más de 50 caracteres.");

		// Si todo está bien, se devuelve el objeto creado
		return new Result<Atencion2025>.Ok(new Atencion2025(pacienteId, medicoId, turnoId, fecha, observaciones));
	}
}
