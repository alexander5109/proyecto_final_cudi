namespace Clinica.Dominio.Entidades;

public record struct Turno {
	Medico Medico;
	Paciente Paciente;
	DateTime FechaYHora;
	TimeSpan Duracion; // defaults to 40 minutes.
}


