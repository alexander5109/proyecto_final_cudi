using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;



public enum TurnoOutcomeEstadoCodigo2025 : byte {
	Programado = 1,
	Ausente = 2,
	Cancelado = 3,
	Concretado = 4,
	Reprogramado = 5
}

//public static class TurnoOutcomeEstadoCodigo2025Extentions {
//	public static string AFechaArgentina(this TurnoOutcomeEstadoCodigo2025 codigo) => dia.ToString("dd/MM/yyyy");
//	public static string AHorasArgentina(this TurnoOutcomeEstadoCodigo2025 codigo) => dia.ToString("HH:mm");
//}


public sealed record TurnoOutcomeEstado2025(
	TurnoOutcomeEstadoCodigo2025 Codigo,
	string Nombre
) : IComoTexto {
	public string ATexto() => Nombre;

	public static readonly TurnoOutcomeEstado2025 Programado = new(TurnoOutcomeEstadoCodigo2025.Programado, "Programado");

	public static readonly TurnoOutcomeEstado2025 Ausente = new(TurnoOutcomeEstadoCodigo2025.Ausente, "Ausente");

	public static readonly TurnoOutcomeEstado2025 Cancelado = new(TurnoOutcomeEstadoCodigo2025.Cancelado, "Cancelado");

	public static readonly TurnoOutcomeEstado2025 Concretado = new(TurnoOutcomeEstadoCodigo2025.Concretado, "Concretado");

	public static readonly TurnoOutcomeEstado2025 Reprogramado = new(TurnoOutcomeEstadoCodigo2025.Reprogramado, "Reprogramado");


	public static readonly IReadOnlyList<TurnoOutcomeEstado2025> Todos = [
		Programado,
		Ausente,
		Cancelado,
		Concretado,
		Reprogramado
	];



	public static Result<TurnoOutcomeEstado2025> CrearPorCodigo(TurnoOutcomeEstadoCodigo2025 codigo) {
		TurnoOutcomeEstado2025? estado = Todos.FirstOrDefault(e => e.Codigo == codigo);
		return estado is not null
			? new Result<TurnoOutcomeEstado2025>.Ok(estado)
			: new Result<TurnoOutcomeEstado2025>.Error(
				$"No existe un TurnoOutcomeEstado con código {(byte)codigo}."
			  );
	}
}
