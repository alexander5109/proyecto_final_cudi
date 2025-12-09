using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;



public enum TurnoEstadoCodigo : byte {
	Programado = 1,
	Ausente = 2,
	Cancelado = 3,
	Concretado = 4,
	Reprogramado = 5
}

public static class TurnoEstadoCodigoFactory {

	public static Result<TurnoEstadoCodigo> CrearResult(TurnoEstadoCodigo codigo) {
		if (!Enum.IsDefined(typeof(TurnoEstadoCodigo), codigo)) {
			return new Result<TurnoEstadoCodigo>.Error($"Valor de TurnoEstadoCodigo inválido: {codigo}");
		}
		return new Result<TurnoEstadoCodigo>.Ok(codigo);
	}

	public static Result<TurnoEstadoCodigo> CrearResult(byte valor) {
		if (!Enum.IsDefined(typeof(TurnoEstadoCodigo), valor)) {
			return new Result<TurnoEstadoCodigo>.Error($"Valor de TurnoEstadoCodigo inválido: {valor}");
		}

		return new Result<TurnoEstadoCodigo>.Ok((TurnoEstadoCodigo)valor);
	}
}
/*
public sealed record TurnoOutcomeEstado2025(
	TurnoEstadoCodigo Codigo,
	string Nombre
) : IComoTexto {
	public string ATexto() => Nombre;

	public static readonly TurnoOutcomeEstado2025 Programado = new(TurnoEstadoCodigo.Programado, "Programado");

	public static readonly TurnoOutcomeEstado2025 Ausente = new(TurnoEstadoCodigo.Ausente, "Ausente");

	public static readonly TurnoOutcomeEstado2025 Cancelado = new(TurnoEstadoCodigo.Cancelado, "Cancelado");

	public static readonly TurnoOutcomeEstado2025 Concretado = new(TurnoEstadoCodigo.Concretado, "Concretado");

	public static readonly TurnoOutcomeEstado2025 Reprogramado = new(TurnoEstadoCodigo.Reprogramado, "Reprogramado");


	public static readonly IReadOnlyList<TurnoOutcomeEstado2025> Todos = [
		Programado,
		Ausente,
		Cancelado,
		Concretado,
		Reprogramado
	];



	public static TurnoOutcomeEstado2025 CrearPorCodigo(TurnoEstadoCodigo codigo) 
		=> Todos.FirstOrDefault(e => e.Codigo == codigo);
}
*/