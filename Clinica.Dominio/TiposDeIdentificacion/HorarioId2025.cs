using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public record struct HorarioId2025(int Valor) {
	public static Result<HorarioId2025> CrearResult(HorarioId2025? id) =>
		(id is HorarioId2025 idGood && idGood.Valor >= 0)
		? new Result<HorarioId2025>.Ok(idGood)
		: new Result<HorarioId2025>.Error("El id no puede ser nulo o negativo.");

	public static HorarioId2025 Crear(int id) => new(id);
	public static bool TryParse(string? s, out HorarioId2025 id) {
		if (int.TryParse(s, out int value)) {
			id = new HorarioId2025(value);
			return true;
		}

		id = default;
		return false;
	}
	public override string ToString() => Valor.ToString();
}
