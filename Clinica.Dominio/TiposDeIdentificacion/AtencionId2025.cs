using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeIdentificacion;

public record struct AtencionId2025(int Valor) {
	public static Result<AtencionId2025> CrearResult(AtencionId2025? id) =>
		(id is AtencionId2025 idGood && idGood.Valor >= 0)
		? new Result<AtencionId2025>.Ok(idGood)
		: new Result<AtencionId2025>.Error("El id no puede ser nulo o negativo.");
	public static Result<AtencionId2025> CrearResult(int? id) =>
		id is int idGood
		? new Result<AtencionId2025>.Ok(new AtencionId2025(idGood))
		: new Result<AtencionId2025>.Error("El id no puede ser nulo.");
	public static AtencionId2025 Crear(int id) => new(id);
	public static bool TryParse(string? s, out AtencionId2025 id) {
		if (int.TryParse(s, out int value)) {
			id = new AtencionId2025(value);
			return true;
		}
		id = default;
		return false;
	}
	public readonly override string ToString() => Valor.ToString();

}
