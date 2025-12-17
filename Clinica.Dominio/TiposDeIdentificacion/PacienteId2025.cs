using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public readonly record struct PacienteId2025(int Valor) {
	public static Result<PacienteId2025> CrearResult(PacienteId2025? id) =>
		(id is PacienteId2025 idGood && idGood.Valor >= 0)
		? new Result<PacienteId2025>.Ok(idGood)
		: new Result<PacienteId2025>.Error("El id no puede ser nulo o negativo.");
	public static Result<PacienteId2025> CrearResult(int? id) =>
		id is int idGood
		? new Result<PacienteId2025>.Ok(new PacienteId2025(idGood))
		: new Result<PacienteId2025>.Error("El id no puede ser nulo.");
	public static PacienteId2025 Crear(int id) => new(id);
	public static bool TryParse(string? s, out PacienteId2025 id) {
		if (int.TryParse(s, out int value)) {
			id = new PacienteId2025(value);
			return true;
		}
		id = default;
		return false;
	}
	public readonly override string ToString() {
		return Valor.ToString();
	}

}
