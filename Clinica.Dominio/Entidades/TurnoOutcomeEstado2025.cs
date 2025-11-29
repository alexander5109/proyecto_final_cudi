using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;

public readonly record struct TurnoOutcomeEstadoCodigo2025(byte Valor);

public sealed record TurnoOutcomeEstado2025(
	TurnoOutcomeEstadoCodigo2025 Codigo,
	string Nombre
) : IComoTexto {
	public string ATexto() => Nombre;

	private TurnoOutcomeEstado2025(byte codigo, string nombre)
		: this(new TurnoOutcomeEstadoCodigo2025(codigo), nombre) { }

	public static readonly TurnoOutcomeEstado2025 Programado = new(1, "Programado");
	public static readonly TurnoOutcomeEstado2025 Ausente = new(2, "Ausente");
	public static readonly TurnoOutcomeEstado2025 Cancelado = new(3, "Cancelado");
	public static readonly TurnoOutcomeEstado2025 Concretado = new(4, "Concretado");
	public static readonly TurnoOutcomeEstado2025 Reprogramado = new(5, "Reprogramado");

	public static readonly IReadOnlyList<TurnoOutcomeEstado2025> Todos = [
		Programado,
		Ausente,
		Cancelado,
		Concretado,
		Reprogramado
	];

	// --- Lookup seguro ---
	public static Result<TurnoOutcomeEstado2025> CrearPorCodigo(byte? codigo) {
		if (codigo is null)
			return new Result<TurnoOutcomeEstado2025>.Error("El código del Outcome no puede ser nulo.");

        TurnoOutcomeEstado2025? estado = Todos.FirstOrDefault(e => e.Codigo.Valor == codigo.Value);

		return estado is not null
			? new Result<TurnoOutcomeEstado2025>.Ok(estado)
			: new Result<TurnoOutcomeEstado2025>.Error($"No existe un TurnoOutcomeEstado con código {codigo}.");
	}
}
