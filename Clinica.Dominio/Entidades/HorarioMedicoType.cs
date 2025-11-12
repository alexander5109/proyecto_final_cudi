using System.Globalization;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;
public readonly record struct HorarioMedicoType(
	HorarioDiaSemanaType DiaSemana,
	HorarioHoraType Desde,
	HorarioHoraType Hasta
);
public static class HorarioMedico2025 {
	public static string AString(this HorarioMedicoType horario) => $"{horario.DiaSemana.NombreDia()} {horario.Desde.AString()}–{horario.Hasta.AString()}";

	public static bool SeSolapaCon(this HorarioMedicoType uno, HorarioMedicoType otro)
		=> uno.DiaSemana == otro.DiaSemana
		&& uno.Desde.Value < otro.Hasta.Value
		&& otro.Desde.Value < uno.Hasta.Value;

	public static Result<HorarioMedicoType> Create(
		HorarioDiaSemanaType dia,
		HorarioHoraType desde,
		HorarioHoraType hasta) {
		if (desde.Value >= hasta.Value)
			return new Result<HorarioMedicoType>.Error("La hora de inicio debe ser anterior a la hora de fin.");

		return new Result<HorarioMedicoType>.Ok(new HorarioMedicoType(dia, desde, hasta));
	}

	public static Result<HorarioMedicoType> Create(string dia, string desde, string hasta)
		=> HorarioDiaSemana2025.Create(dia).Bind(diaOk =>
		   HorarioHora2025.Create(desde).Bind(desdeOk =>
		   HorarioHora2025.Create(hasta).Map(hastaOk =>
			   new HorarioMedicoType(diaOk, desdeOk, hastaOk))));
}
