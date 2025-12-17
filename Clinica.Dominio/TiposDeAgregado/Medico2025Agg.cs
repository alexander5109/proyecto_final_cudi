using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeAgregado;

//public struct HaceGuardia(bool Valor);

public record Medico2025Agg(MedicoId2025 Id, Medico2025 Medico) {
	public static Medico2025Agg Crear(MedicoId2025 id, Medico2025 medico) => new(id, medico);
	public static Result<Medico2025Agg> CrearResult(
		Result<MedicoId2025> idResult,
		Result<Medico2025> medicoResult
	)
		=> from id in idResult
		   from paciente in medicoResult
		   select new Medico2025Agg(
			   id,
			   paciente
		   );
}
