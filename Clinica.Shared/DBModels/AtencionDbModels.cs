using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Shared.DBModels;

public static partial class DbModels {

	public record AtencionDbModel(
		AtencionId2025 Id,
		TurnoId2025 TurnoId,
		PacienteId2025 PacienteId,
		MedicoId2025 MedicoId,
		DateTime FechaHora,
		string Observaciones
	);


}
