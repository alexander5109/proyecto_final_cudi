using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public record struct TurnoId2025(int Valor) {
	public static Result<TurnoId2025> CrearResult(TurnoId2025? id) =>
		(id is TurnoId2025 idGood && idGood.Valor >= 0)
		? new Result<TurnoId2025>.Ok(idGood)
		: new Result<TurnoId2025>.Error("El id no puede ser nulo o negativo.");
	public static TurnoId2025 Crear(int id) => new(id);
	public static Result<TurnoId2025> CrearResult(int? id) =>
		id is int idGood
		? new Result<TurnoId2025>.Ok(new TurnoId2025(idGood))
		: new Result<TurnoId2025>.Error("El id no puede ser nulo.");
	public static bool TryParse(string? s, out TurnoId2025 id) {
		if (int.TryParse(s, out int value)) {
			id = new TurnoId2025(value);
			return true;
		}
		id = default;
		return false;
	}
	public readonly override string ToString() => Valor.ToString();
}
