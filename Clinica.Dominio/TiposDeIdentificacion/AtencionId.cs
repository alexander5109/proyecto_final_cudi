using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public readonly record struct AtencionId(int Valor) {
	public static Result<AtencionId> CrearResult(AtencionId? id) =>
		(id is AtencionId idGood && idGood.Valor >= 0)
		? new Result<AtencionId>.Ok(idGood)
		: new Result<AtencionId>.Error("El id no puede ser nulo o negativo.");
	public static Result<AtencionId> CrearResult(int? id) =>
		id is int idGood
		? new Result<AtencionId>.Ok(new AtencionId(idGood))
		: new Result<AtencionId>.Error("El id no puede ser nulo.");
	public static AtencionId Crear(int id) => new(id);
	public static bool TryParse(string? s, out AtencionId id) {
		if (int.TryParse(s, out int value)) {
			id = new AtencionId(value);
			return true;
		}
		id = default;
		return false;
	}
	public readonly override string ToString() => Valor.ToString();

}
