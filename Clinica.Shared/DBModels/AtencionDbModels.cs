using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Shared.DBModels;

public static partial class DbModels {

	public record AtencionDbModel(
		AtencionId2025 Id,
		TurnoId2025 TurnoId,
		PacienteId2025 PacienteId,
		MedicoId2025 MedicoId,
		DateTime Fecha,
		string Observaciones
	) {
		public AtencionDbModel()
			: this(default, default, default, default, default, "") { }
	}

	public record AtencionDbModelInsert(
		TurnoId2025 TurnoId,
		PacienteId2025 PacienteId,
		MedicoId2025 MedicoId,
		DateTime Fecha,
		string Observaciones
	) {
		public AtencionDbModelInsert()
			: this(default, default, default, default, "") { }
	}

	public static AtencionDbModelInsert ToModel(this Atencion2025 instance) => new AtencionDbModelInsert(instance.TurnoId, instance.PacienteId, instance.MedicoId, instance.Fecha, instance.Observaciones);


}
