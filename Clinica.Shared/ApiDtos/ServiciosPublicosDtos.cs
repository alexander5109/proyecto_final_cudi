using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.ApiDtos;

public static class ServiciosPublicosDtos {
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
	public static Disponibilidad2025 ToDomain(this DisponibilidadDto dto) {
		return new Disponibilidad2025(
			dto.EspecialidadCodigo,
			dto.MedicoId,
			dto.FechaHoraDesde,
			dto.FechaHoraHasta
		);
	}

}
