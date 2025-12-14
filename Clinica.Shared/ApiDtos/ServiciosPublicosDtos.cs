using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.ApiDtos;

public static class ServiciosPublicosDtos {

	public sealed class SolicitarDisponibilidadesDto {
		public EspecialidadEnum EspecialidadCodigo { get; init; }
		public int Cuantos { get; init; }
		public DateTime? APartirDeCuando { get; init; }
		public DayOfWeek? DiaSemanaPreferido { get; init; }
	}





	public sealed record ConcretarTurnoDto(
		TurnoId TurnoId,
		DateTime FechaSolicitud,
		string? Comentario
	);
	public sealed record ModificarTurnoDto(
		TurnoId TurnoId,
		DateTime FechaSolicitud,
		string? Comentario
	);
	public sealed record ProgramarTurnoDto(
		PacienteId PacienteId,
		DateTime FechaSolicitud,
		Disponibilidad2025 Disponibilidad
	);
	//public sealed record DisponibilidadDto( //usemos el del domino
	//	EspecialidadEnum EspecialidadEnum,
	//	MedicoId MedicoId,
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
