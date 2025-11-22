

using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.DataPersistencia;

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
);
	//public Result<Turno2025> ToDomain() {


	//}
