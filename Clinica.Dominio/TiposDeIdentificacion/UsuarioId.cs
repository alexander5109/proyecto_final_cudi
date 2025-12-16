using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public readonly record struct UsuarioId(int Valor) {
	public static UsuarioId Crear(int id) => new(id);
	public static Result<UsuarioId> CrearResult(int? id) =>
		id is int idGood
		? new Result<UsuarioId>.Ok(new UsuarioId(idGood))
		: new Result<UsuarioId>.Error("El id no puede ser nulo.");
}
