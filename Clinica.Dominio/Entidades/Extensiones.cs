using System.Globalization;

namespace Clinica.Dominio.Entidades;

public static class Extensiones {

	public static Dictionary<string, DayOfWeek> DiasDeLaSemanaToEnum = new(StringComparer.OrdinalIgnoreCase) {
		["domingo"] = DayOfWeek.Sunday,
		["lunes"] = DayOfWeek.Monday,
		["martes"] = DayOfWeek.Tuesday,
		["miercoles"] = DayOfWeek.Wednesday,
		["miércoles"] = DayOfWeek.Wednesday,
		["jueves"] = DayOfWeek.Thursday,
		["viernes"] = DayOfWeek.Friday,
		["sabado"] = DayOfWeek.Saturday,
		["sábado"] = DayOfWeek.Saturday,
		// Inglés
		["sunday"] = DayOfWeek.Sunday,
		["monday"] = DayOfWeek.Monday,
		["tuesday"] = DayOfWeek.Tuesday,
		["wednesday"] = DayOfWeek.Wednesday,
		["thursday"] = DayOfWeek.Thursday,
		["friday"] = DayOfWeek.Friday,
		["saturday"] = DayOfWeek.Saturday
	};
	public static string AEspañol(this DayOfWeek dia) => dia switch {
		DayOfWeek.Monday => "Lunes",
		DayOfWeek.Tuesday => "Martes",
		DayOfWeek.Wednesday => "Miércoles",
		DayOfWeek.Thursday => "Jueves",
		DayOfWeek.Friday => "Viernes",
		DayOfWeek.Saturday => "Sábado",
		DayOfWeek.Sunday => "Domingo",
		_ => throw new ArgumentOutOfRangeException(nameof(dia), dia, null)
	};



	public static string AString(this FechaDeNacimiento2025 fecha) => fecha.Valor.ToString("dd/MM/yyyy");
	public static string AString(this FechaIngreso2025 fecha) => fecha.Valor.ToString("dd/MM/yyyy");
	public static string AString(this ProvinciaArgentina2025 provincia) => provincia.Nombre;
	public static string AString(this DiaSemana2025 diaSemana) => CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetDayName(diaSemana.Valor);
	public static string AString(this Turno2025 turno) {
		var medicoTexto = turno.MedicoAsignado is null ? $"Especialidad {turno.Especialidad.Titulo}" : turno.MedicoAsignado.ToString();
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