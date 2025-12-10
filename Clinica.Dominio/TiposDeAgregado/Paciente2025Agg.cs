using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeAgregado;

public record Paciente2025Agg(
	PacienteId Id,
	Paciente2025 Paciente
) {
	public static Paciente2025Agg Crear(PacienteId id, Paciente2025 paciente) => new(id, paciente);
	public static Result<Paciente2025Agg> CrearResult(
		Result<PacienteId> idResult,
		Result<Paciente2025> pacienteResult
	)
		=> from id in idResult
		   from paciente in pacienteResult
		   select new Paciente2025Agg(
			   id,
			   paciente
		   );

}
