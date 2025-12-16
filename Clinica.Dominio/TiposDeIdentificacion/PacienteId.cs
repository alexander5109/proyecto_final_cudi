using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public readonly record struct ConsultaId(int Valor) {
	public static Result<ConsultaId> CrearResult(ConsultaId? id) =>
		(id is ConsultaId idGood && idGood.Valor >= 0)
		? new Result<ConsultaId>.Ok(idGood)
		: new Result<ConsultaId>.Error("El id no puede ser nulo o negativo.");
	public static Result<ConsultaId> CrearResult(int? id) =>
		id is int idGood
		? new Result<ConsultaId>.Ok(new ConsultaId(idGood))
		: new Result<ConsultaId>.Error("El id no puede ser nulo.");
	public static ConsultaId Crear(int id) => new(id);
	public static bool TryParse(string? s, out ConsultaId id) {
		if (int.TryParse(s, out int value)) {
			id = new ConsultaId(value);
			return true;
		}
		id = default;
		return false;
	}
	public readonly override string ToString() {
		return Valor.ToString();
	}

}
