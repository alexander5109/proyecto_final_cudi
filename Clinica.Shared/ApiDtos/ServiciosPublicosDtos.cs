using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.ApiDtos;

public static class ServiciosPublicosDtos {

	public record SolicitarDisponibilidadesDto(
		EspecialidadEnum EspecialidadCodigo,
		int Cuantos,
		DateTime? APartirDeCuando = null,
		DayOfWeek? DiaSemanaPreferido = null,
		MedicoId2025? MedicoPreferido = null
	);



	public sealed record ConcretarTurnoDto(
		TurnoId2025 TurnoId,
		DateTime FechaSolicitud,
		string? Comentario
	);
	public sealed record ModificarTurnoDto(
		TurnoId2025 TurnoId,
		DateTime FechaSolicitud,
		string? Comentario
	);
	public sealed record ProgramarTurnoDto(
		PacienteId2025 PacienteId,
		DateTime FechaSolicitud,
		Disponibilidad2025 Disponibilidad
	);
	//public sealed record DisponibilidadDto( //usemos el del domino
	//	EspecialidadEnum EspecialidadEnum,
	//	MedicoId2025 MedicoId2025,
	//	DateTime FechaHoraDesde,
	//	DateTime FechaHoraHasta
	//);
	public static Disponibilidad2025 ToDomain(this Disponibilidad2025 dto) {
		return new Disponibilidad2025(
			dto.EspecialidadCodigo,
			dto.MedicoId,
			dto.FechaHoraDesde,
			dto.FechaHoraHasta
		);
	}

}
