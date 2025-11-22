using System.Reflection.Metadata.Ecma335;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct ListaHorarioMedicos2025(
	IReadOnlyList<HorarioMedico2025> Valores
) : IComoTexto {
	public string ATexto() {
		if (Valores.Count == 0)
			return "Lista de horarios: (vacía)";
		var lineas = Valores
			.Select(v => "- " + v.ATexto());
		return "Lista de horarios:\n" + string.Join("\n", lineas);
	}

	public static Result<ListaHorarioMedicos2025> Crear(
		IEnumerable<Result<HorarioMedico2025>> horariosResult)
		=> horariosResult.Bind(horariosOk =>
			new Result<ListaHorarioMedicos2025>.Ok(new ListaHorarioMedicos2025(horariosOk))
		);
	public static Result<ListaHorarioMedicos2025> CrearVacia()
		=> new Result<ListaHorarioMedicos2025>.Ok(
			new ListaHorarioMedicos2025([])
		);
}


public readonly record struct HorarioMedico2025(
	DiaSemana2025 DiaSemana,
	HorarioHora2025 Desde,
	HorarioHora2025 Hasta
) : IComoTexto {
	public string ATexto() => $"{DiaSemana.ATexto()}: {Desde.ATexto()} — {Hasta.ATexto()}";

	// ✅ Versión simple (usa tipos ya validados)
	public static Result<HorarioMedico2025> Crear(
		DiaSemana2025 dia,
		HorarioHora2025 desde,
		HorarioHora2025 hasta) {
		if (desde.Valor >= hasta.Valor)
			return new Result<HorarioMedico2025>.Error("La hora de inicio debe ser anterior a la hora de fin.");

		return new Result<HorarioMedico2025>.Ok(new HorarioMedico2025(dia, desde, hasta));
	}

	// ✅ Versión que toma los Result<SubTipo> — ideal para los mappers
	public static Result<HorarioMedico2025> Crear(
		Result<DiaSemana2025> diaResult,
		Result<HorarioHora2025> desdeResult,
		Result<HorarioHora2025> hastaResult)
		=> diaResult.Bind(diaOk =>
		   desdeResult.Bind(desdeOk =>
		   hastaResult.Bind(hastaOk =>
			   Crear(diaOk, desdeOk, hastaOk))));

	// ✅ Versión “desde strings” — útil para tests, carga desde BD, o CSV
	public static Result<HorarioMedico2025> Crear(string dia, string desde, string hasta)
		=> DiaSemana2025.Crear(dia).Bind(diaOk =>
		   HorarioHora2025.Crear(desde).Bind(desdeOk =>
		   HorarioHora2025.Crear(hasta).Bind(hastaOk =>
			   Crear(diaOk, desdeOk, hastaOk))));
}
