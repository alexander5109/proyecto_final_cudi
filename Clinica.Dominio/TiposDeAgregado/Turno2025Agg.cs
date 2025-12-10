using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeAgregado;

public record Turno2025Agg(TurnoId Id, Turno2025 Turno) {
	public static Turno2025Agg Crear(TurnoId id, Turno2025 turno) => new(id, turno);
	public static Result<Turno2025Agg> CrearResult(Result<TurnoId> idResult,Result<Turno2025> instanceResult)
		=> from id in idResult
		   from instance in instanceResult
		   select new Turno2025Agg(
			   id,
			   instance
		   );

}
