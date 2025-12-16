using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeEntidad;

public record Consulta2025(
	AtencionId Id,
	PacienteId PacienteId,
	MedicoId MedicoId,
	DateTime Fecha,
	string Observaciones
);


//public record Turno2025(
//	DateTime FechaDeCreacion,
//	PacienteId PacienteId,
//	MedicoId MedicoId,
//	Especialidad2025 Especialidad,
//	DateTime FechaHoraAsignadaDesdeValor,
//	DateTime FechaHoraAsignadaHastaValor,
//	TurnoEstadoEnum OutcomeEstado,
//	Option<DateTime> OutcomeFechaOption,
//	Option<string> OutcomeComentarioOption
//)