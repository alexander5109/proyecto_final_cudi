using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.Dtos;

public static partial class DbModels {
	public record TurnoDbModel(
		TurnoId Id,
		DateTime FechaDeCreacion,
		PacienteId PacienteId,
		MedicoId MedicoId,
		EspecialidadCodigo EspecialidadCodigo,
		DateTime FechaHoraAsignadaDesde,
		DateTime FechaHoraAsignadaHasta,
		TurnoOutcomeEstadoCodigo2025 OutcomeEstado,
		DateTime? OutcomeFecha,
		string? OutcomeComentario
	) {
		// Constructor sin parámetros requerido por algunos ORMs/serializadores
		public TurnoDbModel() : this(default!, default, default!, default!, default!, default, default, default!, default, default) { }
	};

	public static TurnoDbModel ToModel(this Turno2025 turno) {
		return new TurnoDbModel(
			turno.Id,
			turno.FechaDeCreacion.Valor,
			turno.PacienteId,
			turno.MedicoId,
			turno.Especialidad.Codigo,
			turno.FechaHoraAsignadaDesdeValor,
			turno.FechaHoraAsignadaHastaValor,
			turno.OutcomeEstado.Codigo,
			turno.OutcomeFechaOption.Match(d => d, () => (DateTime?)null),
			turno.OutcomeComentarioOption.Match(s => s, () => (string?)null)
		);
	}
	public static Result<Turno2025> ToDomain(this TurnoDbModel turnoDto) {
		return Turno2025.CrearResult(
			TurnoId.CrearResult(turnoDto.Id.Valor),
			FechaRegistro2025.CrearResult(turnoDto.FechaDeCreacion),
			PacienteId.CrearResult(turnoDto.PacienteId.Valor),
			MedicoId.CrearResult(turnoDto.MedicoId.Valor),
			Especialidad2025.CrearResultPorCodigoInterno(turnoDto.EspecialidadCodigo),
			turnoDto.FechaHoraAsignadaDesde,
			turnoDto.FechaHoraAsignadaHasta,
			TurnoOutcomeEstado2025.CrearPorCodigo(turnoDto.OutcomeEstado),
			turnoDto.OutcomeFecha.ToOption(),
			turnoDto.OutcomeComentario.ToOption()
		);
	}
}