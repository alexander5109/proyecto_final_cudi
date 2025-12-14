using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposExtensiones;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public record TurnoDbModel(
		TurnoId Id,
		DateTime FechaDeCreacion,
		PacienteId PacienteId,
		MedicoId MedicoId,
		EspecialidadEnumCodigo EspecialidadCodigo,
		DateTime FechaHoraAsignadaDesde,
		DateTime FechaHoraAsignadaHasta,
		TurnoEstadoCodigo OutcomeEstado,
		DateTime? OutcomeFecha,
		string? OutcomeComentario
	) {
		// Constructor sin parámetros requerido por algunos ORMs/serializadores
		public TurnoDbModel() : this(default!, default, default!, default!, default!, default, default, default!, default, default) { }
	};

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
	public static TurnoDbModel ToModel(this Turno2025 instance, TurnoId id) {
		return new TurnoDbModel(
			id,
			instance.FechaDeCreacion,
			instance.PacienteId,
			instance.MedicoId,
			instance.Especialidad.Codigo,
			instance.FechaHoraAsignadaDesdeValor,
			instance.FechaHoraAsignadaHastaValor,
			instance.OutcomeEstado,
			instance.OutcomeFechaOption.Match(d => d, () => (DateTime?)null),
			instance.OutcomeComentarioOption.Match(s => s, () => (string?)null)
		);
	}
	public static Result<Turno2025Agg> ToDomainAgg(this TurnoDbModel dbModel) {
		return Turno2025Agg.CrearResult(
			TurnoId.CrearResult(dbModel.Id),
			Turno2025.CrearResult(
			//TurnoId.CrearResult(dbModel.Id.Valor),
			dbModel.FechaDeCreacion,
			PacienteId.CrearResult(dbModel.PacienteId.Valor),
			MedicoId.CrearResult(dbModel.MedicoId.Valor),
			Especialidad2025.CrearResult(dbModel.EspecialidadCodigo),
			dbModel.FechaHoraAsignadaDesde,
			dbModel.FechaHoraAsignadaHasta,
			dbModel.OutcomeEstado.CrearResult(),
			dbModel.OutcomeFecha.ToOption(),
			dbModel.OutcomeComentario.ToOption()
		));
	}
}