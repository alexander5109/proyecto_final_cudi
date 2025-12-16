using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public record struct TurnoId(int Valor) {
	public static Result<TurnoId> CrearResult(TurnoId? id) =>
		(id is TurnoId idGood && idGood.Valor >= 0)
		? new Result<TurnoId>.Ok(idGood)
		: new Result<TurnoId>.Error("El id no puede ser nulo o negativo.");
	public static TurnoId Crear(int id) => new(id);
	public static Result<TurnoId> CrearResult(int? id) =>
		id is int idGood
		? new Result<TurnoId>.Ok(new TurnoId(idGood))
		: new Result<TurnoId>.Error("El id no puede ser nulo.");
	public static bool TryParse(string? s, out TurnoId id) {
		if (int.TryParse(s, out int value)) {
			id = new TurnoId(value);
			return true;
		}
		id = default;
		return false;
	}
	public readonly override string ToString() => Valor.ToString();
}
