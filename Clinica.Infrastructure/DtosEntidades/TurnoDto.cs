using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Infrastructure.DtosEntidades;

public static partial class DtosEntidades {
	public record TurnoDto(
		int Id,
		DateTime FechaDeCreacion,
		int PacienteId,
		int MedicoId,
		int EspecialidadCodigo,
		DateTime FechaHoraAsignadaDesde,
		DateTime FechaHoraAsignadaHasta,
		byte? OutcomeEstado,
		DateTime? OutcomeFecha,
		string? OutcomeComentario
	);
	public static TurnoDto ToDto(this Turno2025 turno) {
		return new TurnoDto(
			turno.Id.Valor,
			turno.FechaDeCreacion.Valor,
			turno.PacienteId.Valor,
			turno.MedicoId.Valor,
			turno.Especialidad.CodigoInterno.Valor,
			turno.FechaHoraAsignadaDesdeValor,
			turno.FechaHoraAsignadaHastaValor,
			(byte?)turno.OutcomeEstadoOption.Codigo.Valor,
			turno.OutcomeFechaOption.Match(d => d, () => (DateTime?)null),
			turno.OutcomeComentarioOption.Match(s => s, () => (string?)null)
		);
	}
	public static Result<Turno2025> ToDomain(this TurnoDto turnoDto) {
		return Turno2025.Crear(
			new TurnoId(turnoDto.Id),
			FechaRegistro2025.Crear(turnoDto.FechaDeCreacion),
			new PacienteId(turnoDto.PacienteId),
			new MedicoId(turnoDto.MedicoId),
			EspecialidadMedica2025.CrearPorCodigoInterno(turnoDto.EspecialidadCodigo),
			turnoDto.FechaHoraAsignadaDesde,
			turnoDto.FechaHoraAsignadaHasta,
			TurnoOutcomeEstado2025.CrearPorCodigo(turnoDto.OutcomeEstado),
			turnoDto.OutcomeFecha.ToOption(),
			turnoDto.OutcomeComentario.ToOption()
		);
	}
}