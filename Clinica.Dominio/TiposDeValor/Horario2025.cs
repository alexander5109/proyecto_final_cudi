using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.TiposDeValor;

public record struct HorarioMedicoId(int Valor) {
	public static Result<HorarioMedicoId> Crear(int? id) =>
		id is int idGood
		? new Result<HorarioMedicoId>.Ok(new HorarioMedicoId(idGood))
		: new Result<HorarioMedicoId>.Error("El id no puede ser nulo.");
	public static bool TryParse(string? s, out HorarioMedicoId id) {
		if (int.TryParse(s, out int value)) {
			id = new HorarioMedicoId(value);
			return true;
		}

		id = default;
		return false;
	}
	public override string ToString() => Valor.ToString();
}
//public readonly record HorarioMedico2025(
//	HorarioMedicoId Id,
//	MedicoId MedicoId,
//	Horario2025 Horario
//);

public readonly record struct Horario2025(
	HorarioMedicoId Id,
	MedicoId MedicoId,
	DiaSemana2025 DiaSemana,
	HorarioHora2025 HoraDesde,
	HorarioHora2025 HoraHasta,
	VigenciaHorario2025 VigenteDesde,
	VigenciaHorario2025 VigenteHasta
) : IComoTexto {
	public string ATexto()
		=> $"{DiaSemana.ATexto()}: {HoraDesde.ATexto()} — {HoraHasta.ATexto()} (vigencia {VigenteDesde.ATexto()} → {VigenteHasta.ATexto()}";

	public static Result<Horario2025> CrearResult(
		HorarioMedicoId id,
		MedicoId medicoId,
		DiaSemana2025 dia,
		HorarioHora2025 desde,
		HorarioHora2025 hasta,
		VigenciaHorario2025 vigenteDesde,
		VigenciaHorario2025 vigenteHasta
	) {
		if (desde.Valor >= hasta.Valor)
			return new Result<Horario2025>.Error("La hora de inicio debe ser anterior a la hora de fin.");

		if (vigenteDesde.Valor >= vigenteHasta.Valor)
			return new Result<Horario2025>.Error("La fecha de inicio de vigencia debe ser anterior a la fecha de fin.");

		return new Result<Horario2025>.Ok(
			new Horario2025(id, medicoId, dia, desde, hasta, vigenteDesde, vigenteHasta)
		);
	}

	public static Result<Horario2025> CrearResult(
		Result<HorarioMedicoId> idResult,
		Result<MedicoId> medicoIdResult,
		Result<DiaSemana2025> diaResult,
		Result<HorarioHora2025> desdeResult,
		Result<HorarioHora2025> hastaResult,
		Result<VigenciaHorario2025> vigenteDesdeResult,
		Result<VigenciaHorario2025> vigenteHastaResult
		)
		=> idResult.Bind(idOk =>
		   medicoIdResult.Bind(medicoIdOk =>
		   diaResult.Bind(diaOk =>
		   desdeResult.Bind(desdeOk =>
		   hastaResult.Bind(hastaOk =>
		   vigenteDesdeResult.Bind(vigenteDesde =>
		   vigenteHastaResult.Bind(vigenteHasta =>
			   CrearResult(idOk, medicoIdOk, diaOk, desdeOk, hastaOk, vigenteDesde, vigenteHasta))))))));
}
