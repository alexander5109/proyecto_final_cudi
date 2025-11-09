namespace Clinica.Dominio.Entidades;

public record struct Turno2025(
	Medico2025 Medico,
	Paciente2025 Paciente,
	DateTime FechaYHora,
	TimeSpan Duracion // defaults to 40 minutes.
);

