using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using System.Globalization;

namespace Clinica.Dominio.Extentions;

public static class TypeExtentionsMethods {

	public static string AString(this FechaDeNacimiento2025 fecha) => fecha.Valor.ToString("dd/MM/yyyy");
	public static string AString(this FechaIngreso2025 fecha) => fecha.Valor.ToString("dd/MM/yyyy");
	public static string AString(this ProvinciaArgentina2025 provincia) => provincia.Nombre;
	public static string AString(this DiaSemana2025 diaSemana) => CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetDayName(diaSemana.Valor);
	public static string AString(this Turno2025 turno) {
		var medicoTexto = turno.MedicoAsignado is null ? $"Especialidad {turno.Especialidad.Titulo}" : turno.MedicoAsignado.Value.NombreCompleto.ToString();
		return $"Turno de {turno.Paciente.NombreCompleto} con {medicoTexto} el {turno.FechaYHora:g}";
	}

	public static string AString(this HorarioHora2025 hora) => hora.Valor.ToString("HH:mm");
	public static string AString(this ListaHorarioMedicos2025 horarios) => string.Join(", ", horarios.Valores.Select(h => h.ToString()));

	public static string AString(this MedicoSueldoMinimo2025 sueldo) => sueldo.Valor.ToString("C", CultureInfo.CurrentCulture);



	public static bool SeSolapaCon(this HorarioMedico2025 uno, HorarioMedico2025 otro)
		=> uno.DiaSemana == otro.DiaSemana
		&& uno.Desde.Valor < otro.Hasta.Valor
		&& otro.Desde.Valor < uno.Hasta.Valor;

	public static bool TienenDisponibilidad(this ListaHorarioMedicos2025 listaHorarios,DateTime fechaYHora,TimeSpan duracion) {
		if (listaHorarios.Valores is null || listaHorarios.Valores.Count == 0)
			return false;

		var diaSemana = new DiaSemana2025(fechaYHora.DayOfWeek);
		var desde = new HorarioHora2025(TimeOnly.FromDateTime(fechaYHora));
		var hasta = new HorarioHora2025(desde.Valor.Add(duracion));

		// Hay disponibilidad si existe al menos un horario que cubra ese rango
		return listaHorarios.Valores.Any(h =>
			h.DiaSemana == diaSemana &&
			h.Desde.Valor <= desde.Valor &&
			h.Hasta.Valor >= hasta.Valor
		);
	}



}