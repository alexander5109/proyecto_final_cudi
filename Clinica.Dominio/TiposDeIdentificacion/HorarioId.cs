using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public record struct HorarioId(int Valor) {
	public static Result<HorarioId> CrearResult(HorarioId? id) =>
		(id is HorarioId idGood && idGood.Valor >= 0)
		? new Result<HorarioId>.Ok(idGood)
		: new Result<HorarioId>.Error("El id no puede ser nulo o negativo.");

	public static Result<HorarioId> Crear(int? id) =>
		id is int idGood
		? new Result<HorarioId>.Ok(new HorarioId(idGood))
		: new Result<HorarioId>.Error("El id no puede ser nulo.");
	public static bool TryParse(string? s, out HorarioId id) {
		if (int.TryParse(s, out int value)) {
			id = new HorarioId(value);
			return true;
		}

		id = default;
		return false;
	}
	public override string ToString() => Valor.ToString();
}
