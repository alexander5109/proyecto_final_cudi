using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.ApiDtos;

public static class TurnoDtos {

	public record TurnoDto(
		DateTime FechaDeCreacion,
		PacienteId PacienteId,
		MedicoId MedicoId,
		EspecialidadCodigo EspecialidadCodigo,
		DateTime FechaHoraAsignadaDesde,
		DateTime FechaHoraAsignadaHasta,
		TurnoEstadoCodigo OutcomeEstado,
		DateTime? OutcomeFecha,
		string? OutcomeComentario
	) {
		// Constructor sin parámetros requerido por algunos ORMs/serializadores
		public TurnoDto() : this(default, default!, default!, default!, default, default, default!, default, default) { }
	};


	public static TurnoDto ToDto(this Turno2025 turno) {
		return new TurnoDto(
			turno.FechaDeCreacion,
			turno.PacienteId,
			turno.MedicoId,
			turno.Especialidad.Codigo,
			turno.FechaHoraAsignadaDesdeValor,
			turno.FechaHoraAsignadaHastaValor,
			turno.OutcomeEstado,
			turno.OutcomeFechaOption.Match(d => (DateTime?)d, () => null),
			turno.OutcomeComentarioOption.Match(s => s, () => (string?)null)
		);
	}


}
