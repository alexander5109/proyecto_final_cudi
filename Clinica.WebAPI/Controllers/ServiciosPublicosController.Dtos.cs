using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.WebAPI.Controllers;



public static class ServiciosPublicosControllerDtos {

	public sealed record ConcretarTurnoDto(
		TurnoId TurnoId,
		DateTime FechaSolicitud,
		string? Comentario
	);
	public sealed record ModificarTurnoDto(
		TurnoId TurnoId,
		DateTime FechaSolicitud,
		string Comentario
	);
	public sealed record ProgramarTurnoDto(
		PacienteId PacienteId,
		DateTime FechaSolicitud,
		DisponibilidadDto Disponibilidad
	);

	public sealed record DisponibilidadDto(
		EspecialidadCodigo EspecialidadCodigo,
		MedicoId MedicoId,
		DateTime FechaHoraDesde,
		DateTime FechaHoraHasta
	);

	public sealed record OutcomeTurnoDto(
		TurnoId TurnoId,
		DateTime Fecha,
		string? Comentario
	);

	public static Disponibilidad2025 ToDomain(this DisponibilidadDto dto) {
		return new Disponibilidad2025(
			dto.EspecialidadCodigo,
			dto.MedicoId,
			dto.FechaHoraDesde,
			dto.FechaHoraHasta
		);
	}



}
