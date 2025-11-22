using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Servicios;

public sealed record ServicioDisponibilidadesSearcher(
	IReadOnlyList<DisponibilidadEspecialidad2025> Valores
) : IComoTexto {
	public string ATexto() =>
		"Lista de disponibilidades:\n" +
		string.Join("\n", Valores.Select(d => "- " + d.ATexto()));

	public static IEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
		SolicitudDeTurno solicitud,
		IReadOnlyList<Medico2025> medicos,
		ServicioTurnosManager turnosActuales
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
								medico.Id,
								slot,
								slot.AddMinutes(duracion)
							);

							// chequeo de colisión inline
							if (!turnosActuales.DisponibilidadNoColisiona(disp.MedicoId, disp.Especialidad, disp.FechaHoraDesde, disp.FechaHoraHasta))
								continue;

							yield return disp;
						}
					}
				}
			}
	}

	public static Result<ServicioDisponibilidadesSearcher> Crear(
		IReadOnlyList<DisponibilidadEspecialidad2025> disponibilidades
	) {
		//var lista = disponibilidades.ToList();
		if (disponibilidades.Count == 0)
			return new Result<ServicioDisponibilidadesSearcher>.Error("No se encontraron disponibilidades.");
		return new Result<ServicioDisponibilidadesSearcher>.Ok(
			new ServicioDisponibilidadesSearcher(disponibilidades)
		);
	}

	public static Result<ServicioDisponibilidadesSearcher> Buscar(
		Result<SolicitudDeTurno> solicitudResult,
		IReadOnlyList<Medico2025> medicos,
		ServicioTurnosManager turnos,
		int cuantos
	) {
		if (solicitudResult is Result<SolicitudDeTurno>.Error err)
			return new Result<ServicioDisponibilidadesSearcher>.Error(err.Mensaje);
		SolicitudDeTurno solicitud = ((Result<SolicitudDeTurno>.Ok)solicitudResult).Valor;
		List<DisponibilidadEspecialidad2025> disponibles = [.. GenerarDisponibilidades(solicitud, medicos, turnos)
			.OrderBy(d => d.FechaHoraDesde)
			.Take(cuantos)];

		return ServicioDisponibilidadesSearcher.Crear(disponibles);
	}

}



public static class DisponibilidadesExtensions {

	public static Result<DisponibilidadEspecialidad2025> TomarPrimera(this Result<ServicioDisponibilidadesSearcher> listadoResult) {

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

	public static Result<ServicioDisponibilidadesSearcher> AplicarFiltrosOpcionales(this Result<ServicioDisponibilidadesSearcher> disponibilidadesResult,SolicitudDeTurnoPreferencias preferencias) {
		if (disponibilidadesResult is Result<ServicioDisponibilidadesSearcher>.Error err)
			return new Result<ServicioDisponibilidadesSearcher>.Error(err.Mensaje);

		ServicioDisponibilidadesSearcher lista = ((Result<ServicioDisponibilidadesSearcher>.Ok)disponibilidadesResult).Valor;
		IEnumerable<DisponibilidadEspecialidad2025> filtradas = lista.Valores;
		if (preferencias.DiaPreferido is DiaSemana2025 dia)
			filtradas = filtradas.Where(d => d.FechaHoraDesde.DayOfWeek == dia.Valor);

		if (preferencias.MomentoPreferido is TardeOMañana momento)
			filtradas = filtradas.Where(d => momento.AplicaA(d.FechaHoraDesde));


		return ServicioDisponibilidadesSearcher.Crear([.. filtradas]);
	}
}
