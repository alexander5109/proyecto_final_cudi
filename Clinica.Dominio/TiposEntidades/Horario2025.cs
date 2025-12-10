using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;

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
//public readonly record HorarioMedico2025(
//	HorarioId Id,
//	MedicoId MedicoId,
//	Horario2025 Horario
//);
public readonly record struct Horario2025Agg(HorarioId Id, Horario2025 Horario) {
	public static Horario2025Agg Crear(HorarioId id, Horario2025 turno) => new(id, turno);
}
public readonly record struct Horario2025(
   //HorarioId Id,
   MedicoId MedicoId,
   DayOfWeek DiaSemana,
   TimeOnly HoraDesde,
   TimeOnly HoraHasta,
   DateOnly VigenteDesde,
   DateOnly VigenteHasta
) : IComoTexto {
	public string ATexto()
		=> $"{DiaSemana.ATexto()}: {HoraDesde.ATextoHoras()} — {HoraHasta.ATextoHoras()} (vigencia {VigenteDesde.ATexto()} → {VigenteHasta.ATexto()}";

	public static Result<Horario2025> CrearResult(
		MedicoId medicoId,
		DayOfWeek dia,
		TimeOnly desde,
		TimeOnly hasta,
		DateOnly vigenteDesde,
		DateOnly vigenteHasta
	) {
		if (desde >= hasta)
			return new Result<Horario2025>.Error("La hora de inicio debe ser anterior a la hora de fin.");

		if (vigenteDesde >= vigenteHasta)
			return new Result<Horario2025>.Error("La fecha de inicio de vigencia debe ser anterior a la fecha de fin.");

		return new Result<Horario2025>.Ok(
			new Horario2025(medicoId, dia, desde, hasta, vigenteDesde, vigenteHasta)
		);
	}
}
