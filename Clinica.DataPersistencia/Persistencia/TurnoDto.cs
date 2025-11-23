using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Infrastructure.Persistencia;

public record TurnoDto(
	int Id,
	DateTime FechaDeCreacion,
	int PacienteId,
	int MedicoId,
	int EspecialidadCodigo,
	DateTime FechaHoraAsignadaDesde,
	DateTime FechaHoraAsignadaHasta,
	byte OutcomeEstado,
	DateTime? OutcomeFecha,
	string? OutcomeComentario
) {
	public Result<Turno2025> ToDomain() {
		return new Result<Turno2025>.Ok(new Turno2025(
			//new TurnoId(Id),
			FechaDeCreacion,
			new PacienteId(PacienteId),
			new MedicoId(MedicoId),
			(EspecialidadMedica2025.CrearPorCodigoInterno(EspecialidadCodigo)).GetOrRaise(),
			FechaHoraAsignadaDesde,
			FechaHoraAsignadaHasta,
			(TurnoOutcomeEstado2025)OutcomeEstado,
			OutcomeFecha.ToOption(),
			OutcomeComentario.ToOption()
		));
	}
}