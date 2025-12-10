using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public readonly record struct PacienteId(int Valor) {
	public static Result<PacienteId> CrearResult(PacienteId? id) =>
		(id is PacienteId idGood && idGood.Valor >= 0)
		? new Result<PacienteId>.Ok(idGood)
		: new Result<PacienteId>.Error("El id no puede ser nulo o negativo.");
	public static Result<PacienteId> CrearResult(int? id) =>
		id is int idGood
		? new Result<PacienteId>.Ok(new PacienteId(idGood))
		: new Result<PacienteId>.Error("El id no puede ser nulo.");
	public static PacienteId Crear(int id) => new(id);
	public static bool TryParse(string? s, out PacienteId id) {
		if (int.TryParse(s, out int value)) {
			id = new PacienteId(value);
			return true;
		}
		id = default;
		return false;
	}
	public readonly override string ToString() {
		return Valor.ToString();
	}

}
