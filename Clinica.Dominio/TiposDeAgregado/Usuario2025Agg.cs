using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeAgregado;

public record Usuario2025Agg(UsuarioId Id, Usuario2025 Usuario) {

	public static Usuario2025Agg Crear(
		UsuarioId id,
		Usuario2025 instance
	) => new(id, instance);

	public static Result<Usuario2025Agg> CrearResult(Result<UsuarioId> idResult, Result<Usuario2025> instanceResult)
		=> from id in idResult
		   from instance in instanceResult
		   select new Usuario2025Agg(
			   id,
			   instance
		   );

}
