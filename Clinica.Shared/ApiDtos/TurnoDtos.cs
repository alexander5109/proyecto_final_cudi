using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposExtensiones;

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


	public static Result<Turno2025> ToDomain(this TurnoDto dbModel) {
		return Turno2025.CrearResult(
			dbModel.FechaDeCreacion,
			PacienteId.CrearResult(dbModel.PacienteId.Valor),
			MedicoId.CrearResult(dbModel.MedicoId.Valor),
			Especialidad2025.CrearResult(dbModel.EspecialidadCodigo),
			dbModel.FechaHoraAsignadaDesde,
			dbModel.FechaHoraAsignadaHasta,
			dbModel.OutcomeEstado.CrearResult(),
			dbModel.OutcomeFecha.ToOption(),
			dbModel.OutcomeComentario.ToOption()
		);
	}


}
