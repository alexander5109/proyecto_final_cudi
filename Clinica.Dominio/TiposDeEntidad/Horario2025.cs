using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposExtensiones;

namespace Clinica.Dominio.TiposDeEntidad;
public readonly record struct Horario2025(
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
