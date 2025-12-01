using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.Dtos;

public static partial class DomainDtos {
	public record TurnoDto {
		public TurnoId Id { get; set; }
		public DateTime FechaDeCreacion { get; set; }
		public PacienteId PacienteId { get; set; }
		public MedicoId MedicoId { get; set; }
		public EspecialidadCodigo2025 EspecialidadCodigo { get; set; }
		public DateTime FechaHoraAsignadaDesde { get; set; }
		public DateTime FechaHoraAsignadaHasta { get; set; }
		public TurnoOutcomeEstadoCodigo2025 OutcomeEstado { get; set; }
		public DateTime? OutcomeFecha { get; set; }
		public string? OutcomeComentario { get; set; }

		// Necesario para Dapper
		public TurnoDto() { }

		// Conveniencia si querés el ctor completo
		public TurnoDto(
			TurnoId id,
			DateTime fechaDeCreacion,
			PacienteId pacienteId,
			MedicoId medicoId,
			EspecialidadCodigo2025 especialidadCodigo,
			DateTime fechaHoraAsignadaDesde,
			DateTime fechaHoraAsignadaHasta,
			TurnoOutcomeEstadoCodigo2025 outcomeEstado,
			DateTime? outcomeFecha,
			string? outcomeComentario
		) {
			Id = id;
			FechaDeCreacion = fechaDeCreacion;
			PacienteId = pacienteId;
			MedicoId = medicoId;
			EspecialidadCodigo = especialidadCodigo;
			FechaHoraAsignadaDesde = fechaHoraAsignadaDesde;
			FechaHoraAsignadaHasta = fechaHoraAsignadaHasta;
			OutcomeEstado = outcomeEstado;
			OutcomeFecha = outcomeFecha;
			OutcomeComentario = outcomeComentario;
		}
	}

	public static TurnoDto ToDto(this Turno2025 turno) {
		return new TurnoDto(
			turno.Id,
			turno.FechaDeCreacion.Valor,
			turno.PacienteId,
			turno.MedicoId,
			turno.Especialidad.CodigoInternoValor,
			turno.FechaHoraAsignadaDesdeValor,
			turno.FechaHoraAsignadaHastaValor,
			turno.OutcomeEstado.Codigo,
			turno.OutcomeFechaOption.Match(d => d, () => (DateTime?)null),
			turno.OutcomeComentarioOption.Match(s => s, () => (string?)null)
		);
	}
	public static Result<Turno2025> ToDomain(this TurnoDto turnoDto) {
		return Turno2025.Crear(
			TurnoId.Crear(turnoDto.Id.Valor),
			FechaRegistro2025.Crear(turnoDto.FechaDeCreacion),
			PacienteId.Crear(turnoDto.PacienteId.Valor),
			MedicoId.Crear(turnoDto.MedicoId.Valor),
			EspecialidadMedica2025.CrearPorCodigoInterno(turnoDto.EspecialidadCodigo),
			turnoDto.FechaHoraAsignadaDesde,
			turnoDto.FechaHoraAsignadaHasta,
			TurnoOutcomeEstado2025.CrearPorCodigo(turnoDto.OutcomeEstado),
			turnoDto.OutcomeFecha.ToOption(),
			turnoDto.OutcomeComentario.ToOption()
		);
	}
}