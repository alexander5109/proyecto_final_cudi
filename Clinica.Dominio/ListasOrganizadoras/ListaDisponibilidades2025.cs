using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.ListasOrganizadoras;

public sealed record ListaDisponibilidades2025(
	IReadOnlyList<DisponibilidadEspecialidad2025> Valores
) : IComoTexto {
	public string ATexto() =>
		"Lista de disponibilidades:\n" +
		string.Join("\n", Valores.Select(d => "- " + d.ATexto()));

	public static IEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
		SolicitudDeTurno solicitud,
		IReadOnlyList<Medico2025> medicos,
		ListaTurnos2025 turnosActuales
	) {
		foreach (var medico in medicos)
			foreach (var especialidad in medico.Especialidades.Valores) {
				if (especialidad != solicitud.Especialidad)
					continue;

				int duracion = especialidad.DuracionConsultaMinutos;

				foreach (var franja in medico.ListaHorarios.Valores) {
					DateTime fecha = solicitud.FechaCreacion.Date;

					// Ajustar al próximo día válido
					while (fecha.DayOfWeek != franja.DiaSemana.Valor)
						fecha = fecha.AddDays(1);

					for (int semana = 0; semana < 30; semana++, fecha = fecha.AddDays(7)) {
						DateTime desde = fecha + franja.Desde.Valor.ToTimeSpan();
						DateTime hasta = fecha + franja.Hasta.Valor.ToTimeSpan();

						// evitar slots pasados
						if (desde < DateTime.Now)
							continue;

						for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
							var disp = new DisponibilidadEspecialidad2025(
								especialidad,
								medico,
								slot,
								slot.AddMinutes(duracion)
							);

							// chequeo de colisión inline
							if (!turnosActuales.DisponibilidadNoColisiona(disp.Medico, disp.Especialidad, disp.FechaHoraDesde, disp.FechaHoraHasta))
								continue;

							yield return disp;
						}
					}
				}
			}
	}

	public static Result<ListaDisponibilidades2025> Crear(
		IReadOnlyList<DisponibilidadEspecialidad2025> disponibilidades
	) {
		//var lista = disponibilidades.ToList();
		if (disponibilidades.Count == 0)
			return new Result<ListaDisponibilidades2025>.Error("No se encontraron disponibilidades.");
		return new Result<ListaDisponibilidades2025>.Ok(
			new ListaDisponibilidades2025(disponibilidades)
		);
	}

	public static Result<ListaDisponibilidades2025> Buscar(
		Result<SolicitudDeTurno> solicitudResult,
		IReadOnlyList<Medico2025> medicos,
		ListaTurnos2025 turnos,
		int cuantos
	) {
		if (solicitudResult is Result<SolicitudDeTurno>.Error err)
			return new Result<ListaDisponibilidades2025>.Error(err.Mensaje);
		SolicitudDeTurno solicitud = ((Result<SolicitudDeTurno>.Ok)solicitudResult).Valor;
		List<DisponibilidadEspecialidad2025> disponibles = [.. GenerarDisponibilidades(solicitud, medicos, turnos)
			.OrderBy(d => d.FechaHoraDesde)
			.Take(cuantos)];

		return ListaDisponibilidades2025.Crear(disponibles);
	}

}



public static class DisponibilidadesExtensions {

	public static Result<DisponibilidadEspecialidad2025> TomarPrimera(
		this Result<ListaDisponibilidades2025> listadoResult
	) {

		return listadoResult.Match<Result<DisponibilidadEspecialidad2025>>(
			ok => {
				// la lista existe, ahora chequeamos si tiene elementos
				if (ok.Valores.Count == 0)
					return new Result<DisponibilidadEspecialidad2025>.Error(
						"No hay disponibilidades para seleccionar."
					);
				return new Result<DisponibilidadEspecialidad2025>.Ok(ok.Valores[0]);
			},

			mensajeError =>
				new Result<DisponibilidadEspecialidad2025>.Error(mensajeError)
		);
	}

	public static Result<ListaDisponibilidades2025> AplicarFiltrosOpcionales(
		this Result<ListaDisponibilidades2025> disponibilidadesResult,
		SolicitudDeTurnoPreferencias preferencias
	) {
		if (disponibilidadesResult is Result<ListaDisponibilidades2025>.Error err)
			return new Result<ListaDisponibilidades2025>.Error(err.Mensaje);

		ListaDisponibilidades2025 lista = ((Result<ListaDisponibilidades2025>.Ok)disponibilidadesResult).Valor;
		IEnumerable<DisponibilidadEspecialidad2025> filtradas = lista.Valores;
		if (preferencias.DiaPreferido is DiaSemana2025 dia)
			filtradas = filtradas.Where(d => d.FechaHoraDesde.DayOfWeek == dia.Valor);

		if (preferencias.MomentoPreferido is TardeOMañana momento)
			filtradas = filtradas.Where(d => momento.AplicaA(d.FechaHoraDesde));


		return ListaDisponibilidades2025.Crear([.. filtradas]);
	}
}
