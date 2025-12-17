using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public record struct MedicoId2025(int Valor) {
	public static Result<MedicoId2025> CrearResult(MedicoId2025? id) =>
		(id is MedicoId2025 idGood && idGood.Valor >= 0)
		? new Result<MedicoId2025>.Ok(idGood)
		: new Result<MedicoId2025>.Error("El id no puede ser nulo o negativo.");
	public static Result<MedicoId2025> CrearResult(int? id) =>
		id is int idGood
		? new Result<MedicoId2025>.Ok(new MedicoId2025(idGood))
		: new Result<MedicoId2025>.Error("El id no puede ser nulo.");
	public static MedicoId2025 Crear(int id) => new(id);
	public static bool TryParse(string? s, out MedicoId2025 id) {
		if (int.TryParse(s, out int value)) {
			id = new MedicoId2025(value);
			return true;
		}

		id = default;
		return false;
	}
	public override string ToString() => Valor.ToString();
}
