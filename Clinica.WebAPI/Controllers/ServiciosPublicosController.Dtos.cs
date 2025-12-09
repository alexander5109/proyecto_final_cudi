using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.WebAPI.Controllers;



public static class ServiciosPublicosControllerDtos {

	public sealed record ProgramarTurnoDto(
		int PacienteId,
		DateTime FechaSolicitud,
		DisponibilidadDto Disponibilidad
	);

	public sealed record DisponibilidadDto(
		EspecialidadCodigo EspecialidadCodigo,
		int MedicoId,
		DateTime FechaHoraDesde,
		DateTime FechaHoraHasta
	);

	public sealed record OutcomeTurnoDto(
		int TurnoId,
		DateTime Fecha,
		string? Comentario
	);

	public static Disponibilidad2025 ToDomain(this DisponibilidadDto dto) {
		return new Disponibilidad2025(
			dto.EspecialidadCodigo,
			new MedicoId(dto.MedicoId),
			dto.FechaHoraDesde,
			dto.FechaHoraHasta
		);
	}



}
