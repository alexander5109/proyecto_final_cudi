using System.Globalization;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public readonly record struct HorarioMedicoType(
	DiaSemanaType DiaSemana,
	HorarioHoraType Desde,
	HorarioHoraType Hasta
);

public static class HorarioMedico2025 {
	public static string AString(this HorarioMedicoType horario)
		=> $"{horario.DiaSemana.AString()} {horario.Desde.AString()}–{horario.Hasta.AString()}";

	public static bool SeSolapaCon(this HorarioMedicoType uno, HorarioMedicoType otro)
		=> uno.DiaSemana == otro.DiaSemana
		&& uno.Desde.Valor < otro.Hasta.Valor
		&& otro.Desde.Valor < uno.Hasta.Valor;

	// ✅ Versión simple (usa tipos ya validados)
	public static Result<HorarioMedicoType> Crear(
		DiaSemanaType dia,
		HorarioHoraType desde,
		HorarioHoraType hasta) {
		if (desde.Valor >= hasta.Valor)
			return new Result<HorarioMedicoType>.Error("La hora de inicio debe ser anterior a la hora de fin.");

		return new Result<HorarioMedicoType>.Ok(new HorarioMedicoType(dia, desde, hasta));
	}

	// ✅ Versión que toma los Result<SubTipo> — ideal para los mappers
	public static Result<HorarioMedicoType> Crear(
		Result<DiaSemanaType> diaResult,
		Result<HorarioHoraType> desdeResult,
		Result<HorarioHoraType> hastaResult)
		=> diaResult.Bind(diaOk =>
		   desdeResult.Bind(desdeOk =>
		   hastaResult.Bind(hastaOk =>
			   Crear(diaOk, desdeOk, hastaOk))));

	// ✅ Versión “desde strings” — útil para tests, carga desde BD, o CSV
	public static Result<HorarioMedicoType> Crear(string dia, string desde, string hasta)
		=> DiaSemana2025.Crear(dia).Bind(diaOk =>
		   HorarioHora2025.Crear(desde).Bind(desdeOk =>
		   HorarioHora2025.Crear(hasta).Bind(hastaOk =>
			   Crear(diaOk, desdeOk, hastaOk))));
}
