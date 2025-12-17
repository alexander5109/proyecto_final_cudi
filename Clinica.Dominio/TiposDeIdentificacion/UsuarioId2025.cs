using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public readonly record struct UsuarioId2025(int Valor) {
	public static UsuarioId2025 Crear(int id) => new(id);
	public static Result<UsuarioId2025> CrearResult(int? id) =>
		id is int idGood
		? new Result<UsuarioId2025>.Ok(new UsuarioId2025(idGood))
		: new Result<UsuarioId2025>.Error("El id no puede ser nulo.");
	public readonly override string ToString() => Valor.ToString();
}
