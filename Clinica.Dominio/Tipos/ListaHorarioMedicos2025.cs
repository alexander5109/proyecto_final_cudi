using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.Tipos;

public readonly record struct ListaHorarioMedicos2025(
	IReadOnlyList<HorarioMedico2025> Valores
) {
	public static Result<ListaHorarioMedicos2025> Crear(IReadOnlyList<Result<HorarioMedico2025>> horariosResult)
		=> horariosResult.Bind(horariosOk =>
			new Result<ListaHorarioMedicos2025>.Ok(new ListaHorarioMedicos2025(horariosOk))
		);
	// ✅ 1. Factory desde lista de HorarioMedico2025 ya válidos
	public static Result<ListaHorarioMedicos2025> Crear(
		IEnumerable<HorarioMedico2025> valores) {
		if (valores is null)
			return new Result<ListaHorarioMedicos2025>.Error("La lista de horarios no puede ser nula.");

		return new Result<ListaHorarioMedicos2025>.Ok(new ListaHorarioMedicos2025(valores.ToList()));
	}

	// ✅ 2. Factory desde lista de Result<HorarioMedico2025>
	public static Result<ListaHorarioMedicos2025> Crear(
		IEnumerable<Result<HorarioMedico2025>> resultados) {
		if (resultados is null)
			return new Result<ListaHorarioMedicos2025>.Error("La lista de resultados no puede ser nula.");

		var errores = resultados.OfType<Result<HorarioMedico2025>.Error>().ToList();
		if (errores.Count != 0) {
			var mensaje = string.Join(" | ", errores.Select(e => e.Mensaje));
			return new Result<ListaHorarioMedicos2025>.Error($"Errores en horarios: {mensaje}");
		}

		var valoresOk = resultados
			.OfType<Result<HorarioMedico2025>.Ok>()
			.Select(r => r.Valor)
			.ToList();

		return new Result<ListaHorarioMedicos2025>.Ok(new ListaHorarioMedicos2025(valoresOk));
	}

	// ✅ 3. Factory “desde strings” (útil para CSV, test o carga directa)
	public static Result<ListaHorarioMedicos2025> Crear(
		IEnumerable<(string Dia, string Desde, string Hasta)> entradas) {
		if (entradas is null)
			return new Result<ListaHorarioMedicos2025>.Error("La lista de entradas no puede ser nula.");

		var resultados = entradas
			.Select(e => HorarioMedico2025.Crear(e.Dia, e.Desde, e.Hasta))
			.ToList();

		return Crear(resultados); // Reutiliza la factory #2
	}

	// ✅ 4. Versión vacía explícita
	public static Result<ListaHorarioMedicos2025> CrearVacia()
		=> new Result<ListaHorarioMedicos2025>.Ok(
			new ListaHorarioMedicos2025([])
		);
}
