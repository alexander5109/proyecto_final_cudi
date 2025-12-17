using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeAgregado;

public record Usuario2025Agg(UsuarioId2025 Id, Usuario2025 Usuario) {

	public static Usuario2025Agg Crear(
		UsuarioId2025 id,
		Usuario2025 instance
	) => new(id, instance);
	public static Result<Usuario2025Agg> CrearResult(Result<UsuarioId2025> idResult, Result<Usuario2025> instanceResult)
		=> from id in idResult
		   from instance in instanceResult
		   select new Usuario2025Agg(
			   id,
			   instance
		   );

}

public record Usuario2025EdicionAgg(UsuarioId2025 Id, Usuario2025Edicion Usuario) {

	public static Usuario2025EdicionAgg Crear(
		UsuarioId2025 id,
		Usuario2025Edicion instance
	) => new(id, instance);
	public static Result<Usuario2025EdicionAgg> CrearResult(Result<UsuarioId2025> idResult, Result<Usuario2025Edicion> instanceResult)
		=> from id in idResult
		   from instance in instanceResult
		   select new Usuario2025EdicionAgg(
			   id,
			   instance
		   );

}
