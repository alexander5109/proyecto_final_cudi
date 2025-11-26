
namespace Clinica.WebAPI.DTOs;

public record ReprogramarTurnoRequestDto(
	DateTime NuevaFechaDesde,
	DateTime NuevaFechaHasta
);

public record CrearTurnoRequestDto(
	int PacienteId,
	int MedicoId,
	int EspecialidadCodigo,
	DateTime Desde,
	DateTime Hasta
);

public record CancelarTurnoRequest(
	DateTime OutcomeFecha,
	string OutcomeComentario
);

public record DisponibilidadDTO(
	int MedicoId,
	DateTime FechaInicio,
	DateTime FechaFin,
	int EspecialidadCodigoInterno
);


public record ReprogramarTurnoRequest(
	DateTime OutcomeFecha,
	string OutcomeComentario
);

public record SolicitarTurnoRequest(
	int PacienteId,
	string Especialidad   // string para no acoplar la API al enum interno
);