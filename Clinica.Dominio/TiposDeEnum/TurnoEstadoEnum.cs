namespace Clinica.Dominio.TiposDeEnum;



public enum TurnoEstadoEnum : byte {
	Programado = 1,
	Ausente = 2,
	Cancelado = 3,
	Concretado = 4,
	Reprogramado = 5
}

/*
public sealed record TurnoOutcomeEstado2025(
	TurnoEstadoEnum Codigo,
	string Nombre
) : IComoTexto {
	public string ATextoDia() => Nombre;

	public static readonly TurnoOutcomeEstado2025 Programado = new(TurnoEstadoEnum.Programado, "Programado");

	public static readonly TurnoOutcomeEstado2025 Ausente = new(TurnoEstadoEnum.Ausente, "Ausente");

	public static readonly TurnoOutcomeEstado2025 Cancelado = new(TurnoEstadoEnum.Cancelado, "Cancelado");

	public static readonly TurnoOutcomeEstado2025 Concretado = new(TurnoEstadoEnum.Concretado, "Concretado");

	public static readonly TurnoOutcomeEstado2025 Reprogramado = new(TurnoEstadoEnum.Reprogramado, "Reprogramado");


	public static readonly IReadOnlyList<TurnoOutcomeEstado2025> Todos = [
		Programado,
		Ausente,
		Cancelado,
		Concretado,
		Reprogramado
	];



	public static TurnoOutcomeEstado2025 CrearPorCodigo(TurnoEstadoEnum codigo) 
		=> Todos.FirstOrDefault(e => e.Codigo == codigo);
}
*/