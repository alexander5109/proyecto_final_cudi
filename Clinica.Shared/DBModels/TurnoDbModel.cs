using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public record TurnoDbModel(
		TurnoId Id,
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
		public TurnoDbModel() : this(default!, default, default!, default!, default!, default, default, default!, default, default) { }
	};



	public static TurnoDbModel ToModel(this Turno2025 turno) {
		return new TurnoDbModel(
			default,
			turno.FechaDeCreacion,
			turno.PacienteId,
			turno.MedicoId,
			turno.Especialidad.Codigo,
			turno.FechaHoraAsignadaDesdeValor,
			turno.FechaHoraAsignadaHastaValor,
			turno.OutcomeEstado,
			turno.OutcomeFechaOption.Match(d => d, () => (DateTime?)null),
			turno.OutcomeComentarioOption.Match(s => s, () => (string?)null)
		);
	}

	public static TurnoDbModel ToModel(this Turno2025Agg aggrg) {
		return new TurnoDbModel(
			aggrg.Id,
			aggrg.Turno.FechaDeCreacion,
			aggrg.Turno.PacienteId,
			aggrg.Turno.MedicoId,
			aggrg.Turno.Especialidad.Codigo,
			aggrg.Turno.FechaHoraAsignadaDesdeValor,
			aggrg.Turno.FechaHoraAsignadaHastaValor,
			aggrg.Turno.OutcomeEstado,
			aggrg.Turno.OutcomeFechaOption.Match(d => d, () => (DateTime?)null),
			aggrg.Turno.OutcomeComentarioOption.Match(s => s, () => (string?)null)
		);
	}
	public static Result<Turno2025> ToDomain(this TurnoDbModel turnoDto) {
		return Turno2025.CrearResult(
			//TurnoId.CrearResult(turnoDto.Id.Valor),
			turnoDto.FechaDeCreacion,
			PacienteId.CrearResult(turnoDto.PacienteId.Valor),
			MedicoId.CrearResult(turnoDto.MedicoId.Valor),
			Especialidad2025.CrearResult(turnoDto.EspecialidadCodigo),
			turnoDto.FechaHoraAsignadaDesde,
			turnoDto.FechaHoraAsignadaHasta,
			turnoDto.OutcomeEstado.CrearResult(),
			turnoDto.OutcomeFecha.ToOption(),
			turnoDto.OutcomeComentario.ToOption()
		);
	}
}