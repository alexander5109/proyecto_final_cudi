using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeAgregado;

//public readonly record HorarioMedico2025(
//	HorarioId Id,
//	MedicoId MedicoId,
//	Horario2025 Horario
//);
public readonly record struct Horario2025Agg(HorarioId Id, Horario2025 Horario) {
	public static Horario2025Agg Crear(HorarioId id, Horario2025 turno) => new(id, turno);
	public static Result<Horario2025Agg> CrearResult(
		Result<HorarioId> idResult,
		Result<Horario2025> pacienteResult
	)
		=> from id in idResult
		   from paciente in pacienteResult
		   select new Horario2025Agg(
			   id,
			   paciente
		   );
}
