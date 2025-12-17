using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeAgregado;

public record Paciente2025Agg(
	PacienteId2025 Id,
	Paciente2025 Paciente
) {
	public static Paciente2025Agg Crear(PacienteId2025 id, Paciente2025 paciente) => new(id, paciente);
	public static Result<Paciente2025Agg> CrearResult(
		Result<PacienteId2025> idResult,
		Result<Paciente2025> pacienteResult
	)
		=> from id in idResult
		   from paciente in pacienteResult
		   select new Paciente2025Agg(
			   id,
			   paciente
		   );

}
