using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

public record struct HorarioId(int Valor) {
	public static Result<HorarioId> Crear(int? id) =>
		id is int idGood
		? new Result<HorarioId>.Ok(new HorarioId(idGood))
		: new Result<HorarioId>.Error("El id no puede ser nulo.");
	public static bool TryParse(string? s, out HorarioId id) {
		if (int.TryParse(s, out int value)) {
			id = new HorarioId(value);
			return true;
		}

		id = default;
		return false;
	}
	public override string ToString() => Valor.ToString();
}


public readonly record struct HorarioMedico2025(
	HorarioId Id,
	DiaSemana2025 DiaSemana,
	HorarioHora2025 HoraDesde,
	HorarioHora2025 HoraHasta,
	VigenciaHorario2025 VigenteDesde,
	VigenciaHorario2025 VigenteHasta
) : IComoTexto {
	public string ATexto()
		=> $"{DiaSemana.ATexto()}: {HoraDesde.ATexto()} — {HoraHasta.ATexto()} (vigencia {VigenteDesde.ATexto()} → {VigenteHasta.ATexto()}";

	public static Result<HorarioMedico2025> Crear(
		HorarioId id,
		DiaSemana2025 dia,
		HorarioHora2025 desde,
		HorarioHora2025 hasta,
		VigenciaHorario2025 vigenteDesde,
		VigenciaHorario2025 vigenteHasta
	) {
		if (desde.Valor >= hasta.Valor)
			return new Result<HorarioMedico2025>.Error("La hora de inicio debe ser anterior a la hora de fin.");

		if (vigenteDesde.Valor >= vigenteHasta.Valor)
			return new Result<HorarioMedico2025>.Error("La fecha de inicio de vigencia debe ser anterior a la fecha de fin.");

		return new Result<HorarioMedico2025>.Ok(
			new HorarioMedico2025(id, dia, desde, hasta, vigenteDesde, vigenteHasta)
		);
	}


	// ✅ Versión que toma los Result<SubTipo> — ideal para los mappers
	public static Result<HorarioMedico2025> Crear(
		Result<HorarioId> idResult,
		Result<DiaSemana2025> diaResult,
		Result<HorarioHora2025> desdeResult,
		Result<HorarioHora2025> hastaResult,
		Result<VigenciaHorario2025> vigenteDesdeResult,
		Result<VigenciaHorario2025> vigenteHastaResult
		)
		=> idResult.Bind(idOk =>
		   diaResult.Bind(diaOk =>
		   desdeResult.Bind(desdeOk =>
		   hastaResult.Bind(hastaOk =>
		   vigenteDesdeResult.Bind(vigenteDesde =>
		   vigenteHastaResult.Bind(vigenteHasta =>
			   Crear(idOk, diaOk, desdeOk, hastaOk, vigenteDesde, vigenteHasta)))))));

	// ✅ Versión “desde strings” — útil para tests, carga desde BD, o CSV
	public static Result<HorarioMedico2025> Crear(int id, string dia, string desde, string hasta, string vigenteDesde, string vigenteHasta)
		=> DiaSemana2025.Crear(dia).Bind(diaOk =>
		   HorarioId.Crear(id).Bind(idOk =>
		   HorarioHora2025.Crear(desde).Bind(desdeOk =>
		   HorarioHora2025.Crear(hasta).Bind(hastaOk =>
		   VigenciaHorario2025.Crear(vigenteDesde).Bind(vigenteDesdeOk =>
		   VigenciaHorario2025.Crear(vigenteHasta).Bind(vigenteHastaOk =>
			   Crear(idOk, diaOk, desdeOk, hastaOk, vigenteDesdeOk, vigenteHastaOk)))))));
}
