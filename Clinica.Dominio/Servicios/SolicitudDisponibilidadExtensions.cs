using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Repositorios;
using Clinica.Dominio.TiposDeValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinica.Dominio.Servicios;

public record EspecialidadDisponibilidadHoraria(
	Medico2025 Medico,
	DateTime Inicio,
	DateTime Fin
);

public static class SolicitudDisponibilidadExtensions {
	// Generador on-demand: devuelve, de manera perezosa, disponibles desde la fecha solicitada hacia adelante.
	// Cada vez que se itera, se calcula para el siguiente día con posibles slots según disponibilidad y turnos ya ocupados.
	public static Result<IEnumerable<EspecialidadDisponibilidadHoraria>> BuscarDisponibilidades(
		this SolicitudConsulta2025 solicitud,
		IRepositorioMedicos repoMedicos,
		IRepositorioTurnos repoTurnos,
		DateTime? desde = null,
		int diasHaciaAdelante = 30
	) {
		IEnumerable<EspecialidadDisponibilidadHoraria> Generator() {
			var start = desde?.Date ?? solicitud.FechaDeseada.Date;
			var especialidad = solicitud.Especialidad;

			// Obtener médicos que ejercen la especialidad
			var medicosR = repoMedicos.ObtenerMedicosPorEspecialidad(especialidad.Titulo);
			if (medicosR is Result<IReadOnlyList<Medico2025>>.Error) yield break;
			var medicos = ((Result<IReadOnlyList<Medico2025>>.Ok)medicosR).Valor;

			// Obtener turnos de la especialidad en el rango
			var turnosR = repoTurnos.ObtenerTurnosPorEspecialidad(especialidad.Titulo, start, start.AddDays(diasHaciaAdelante));
			var turnos = turnosR is Result<IReadOnlyList<Turno2025>>.Ok ok ? ok.Valor : [];

			var duracion = TimeSpan.FromMinutes(especialidad.DuracionConsultaMinutos);

			for (var fecha = start; fecha <= start.AddDays(diasHaciaAdelante); fecha = fecha.AddDays(1)) {
				var diaSemana = fecha.DayOfWeek;
				foreach (var medico in medicos) {
					var horarios = medico.ListaHorarios.Valores.Where(h => h.DiaSemana.Valor == diaSemana);
					foreach (var horario in horarios) {
						var cursor = horario.Desde.Valor;
						while (cursor.Add(duracion) <= horario.Hasta.Valor) {
							var slotStart = DateTime.SpecifyKind(fecha.Add(cursor.ToTimeSpan()), DateTimeKind.Local);
							var slotEnd = slotStart.Add(duracion);

							// verificar solapamiento con turnos existentes del médico
							var ocupado = turnos.Any(t => t.MedicoAsignado is not null && t.MedicoAsignado.Value.Dni == medico.Dni && t.FechaYHora < slotEnd && t.FechaYHora.Add(TimeSpan.FromMinutes(t.Especialidad.DuracionConsultaMinutos)) > slotStart);
							if (!ocupado) {
								yield return new EspecialidadDisponibilidadHoraria(medico, slotStart, slotEnd);
							}

							cursor = cursor.Add(duracion);
						}
					}
				}
			}

		}

        // Probar si hay al menos un elemento disponible
        using var enumerator = Generator().GetEnumerator();
        if (!enumerator.MoveNext()) return new Result<IEnumerable<EspecialidadDisponibilidadHoraria>>.Error("No hay disponibilidades encontradas."); // Mensaje de error dummy

        // Si hay al menos un elemento, envolver en un IEnumerable que emite primero el elemento actual
        IEnumerable<EspecialidadDisponibilidadHoraria> Wrapper() {
            yield return enumerator.Current;
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

        return new Result<IEnumerable<EspecialidadDisponibilidadHoraria>>.Ok(Wrapper());
    }
}
