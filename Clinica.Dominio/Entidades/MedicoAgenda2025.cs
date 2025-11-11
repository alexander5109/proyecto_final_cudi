using Clinica.Dominio.Comun;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoAgenda2025 {
	public IReadOnlyList<MedicoDisponibilidadEnDia2025> DisponibilidadEnDia { get; }

	private MedicoAgenda2025(IReadOnlyList<MedicoDisponibilidadEnDia2025> disponibilidades) {
		DisponibilidadEnDia = disponibilidades;
	}

	// 🏭 Factory controlada
	public static Result<MedicoAgenda2025> Crear(Result<IReadOnlyList<MedicoDisponibilidadEnDia2025>> disponibilidadesResult)
		=> disponibilidadesResult.Map(disponibilidadesOk => new MedicoAgenda2025(disponibilidadesOk));

	public bool EstaDisponibleEn(DateTime fechaYHora, TimeSpan duracion) {
		// Si no tiene agenda o está vacía
		if (DisponibilidadEnDia is null || DisponibilidadEnDia.Count == 0)
			return false;

		var dia = new MedicoDiaDeLaSemana2025(fechaYHora.DayOfWeek);
		var hora = TimeOnly.FromDateTime(fechaYHora);

		// Buscar si el médico trabaja ese día
		var diaAgenda = DisponibilidadEnDia
			.FirstOrDefault(d => d.DiaSemana.Value == dia.Value);

		if (diaAgenda.FranjasHorarias is null || diaAgenda.FranjasHorarias.Count == 0)
			return false;

		// Verificar si alguna franja cubre el rango solicitado
		foreach (var franja in diaAgenda.FranjasHorarias) {
			if (hora >= franja.Desde && hora.Add(duracion) <= franja.Hasta)
				return true;
		}

		return false;
	}
}
