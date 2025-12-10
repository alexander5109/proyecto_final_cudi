using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public record struct MedicoId(int Valor) {
	public static Result<MedicoId> CrearResult(MedicoId? id) =>
		(id is MedicoId idGood && idGood.Valor >= 0)
		? new Result<MedicoId>.Ok(idGood)
		: new Result<MedicoId>.Error("El id no puede ser nulo o negativo.");
	public static Result<MedicoId> CrearResult(int? id) =>
		id is int idGood
		? new Result<MedicoId>.Ok(new MedicoId(idGood))
		: new Result<MedicoId>.Error("El id no puede ser nulo.");
	public static MedicoId Crear(int id) => new(id);
	public static bool TryParse(string? s, out MedicoId id) {
		if (int.TryParse(s, out int value)) {
			id = new MedicoId(value);
			return true;
		}

		id = default;
		return false;
	}
	public override string ToString() => Valor.ToString();
}
