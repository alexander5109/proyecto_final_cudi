using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Shared.DBModels;

public static partial class DbModels {

	public record AtencionDbModel(
		AtencionId Id,
		TurnoId TurnoId,
		PacienteId PacienteId,
		MedicoId MedicoId,
		DateTime FechaHora,
		string Observaciones
	);


}
