using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.ListasOrganizadoras;

public class ServicioTurnosManager(
	List<Turno2025> Turnos,
	IReadOnlyList<Medico2025> Medicos
//,IReadOnlyList<Paciente2025> Pacientes
) {
	public static Result<ServicioTurnosManager> CrearServicio(
		List<Result<Turno2025>> turnos
		, IReadOnlyList<Result<Medico2025>> medicos
	//,IReadOnlyList<Result<Paciente2025>> pacientes
	) {
		List<string> errores = [];

		// --- recopilar errores de turnos ---
		foreach (Result<Turno2025> turno in turnos) {
			if (turno is Result<Turno2025>.Error err)
				errores.Add(err.Mensaje);
			turno.PrintAndContinue("Turno domainizado");
		}

		// --- recopilar errores de medicos ---
		foreach (Result<Medico2025> medico in medicos) {
			if (medico is Result<Medico2025>.Error err)
				errores.Add(err.Mensaje);
			medico.PrintAndContinue("Medico domainizado");
		}

		// --- recopilar errores de pacientes ---
		//foreach (Result<Paciente2025> paciente in pacientes) {
		//	if (paciente is Result<Paciente2025>.Error err)
		//		errores.Add(err.Mensaje);
		//	paciente.PrintAndContinue("Paciente domainizado");
		//}

		// --- si hubo errores, devolverlos todos juntos ---
		if (errores.Count > 0)
			return new Result<ServicioTurnosManager>.Error(string.Join("; ", errores));

		// --- desempaquetar valores OK ---
		List<Turno2025> turnosOk = [.. turnos
			.Cast<Result<Turno2025>.Ok>()
			.Select(ok => ok.Valor)];

		ReadOnlyCollection<Medico2025> medicosOk = medicos
			.Cast<Result<Medico2025>.Ok>()
			.Select(ok => ok.Valor)
			.ToList()
			.AsReadOnly();

		//ReadOnlyCollection<Paciente2025> pacientesOk = pacientes
		//	.Cast<Result<Paciente2025>.Ok>()
		//	.Select(ok => ok.Valor)
		//	.ToList()
		//	.AsReadOnly();

		// --- crear instancia ---
		return new Result<ServicioTurnosManager>.Ok(
			new ServicioTurnosManager(
				turnosOk,
				medicosOk
			//,pacientesOk
			)
		);
	}






	private Result<ServicioTurnosManager> _AgendarTurno(Result<Turno2025> turnoResult) {
		return turnoResult.Match<Result<ServicioTurnosManager>>(
			ok => {
				Turnos.Add(ok);
				return new Result<ServicioTurnosManager>.Ok(this);
			},
			mensaje => new Result<ServicioTurnosManager>.Error(mensaje)
		);
	}


	public Result<IReadOnlyList<DisponibilidadEspecialidad2025>> SolicitarDisponibilidadesPara(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		int cuantos
	) {
		IReadOnlyList<DisponibilidadEspecialidad2025> disponibles = [.. _GenerarDisponibilidades(solicitudEspecialidad, solicitudFechaCreacion)
			.OrderBy(d => d.FechaHoraDesde)
			.Take(cuantos)];
		if (disponibles.Count > 0) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok(disponibles);
		} else {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error("No se encontraron disponibilidades");
		}
	}



	private IEnumerable<DisponibilidadEspecialidad2025> _GenerarDisponibilidades(EspecialidadMedica2025 solicitudEspecialidad, DateTime solicitudFechaCreacion) {
		foreach (Medico2025 medico in Medicos)
			foreach (EspecialidadMedica2025 especialidad in medico.Especialidades.Valores) {
				if (especialidad != solicitudEspecialidad)
					continue;

				int duracion = especialidad.DuracionConsultaMinutos;

				foreach (HorarioMedico2025 franja in medico.ListaHorarios.Valores) {
					DateTime fecha = solicitudFechaCreacion.Date;

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
							if (!_DisponibilidadNoColisiona(disp.MedicoId, disp.Especialidad, disp.FechaHoraDesde, disp.FechaHoraHasta))
								continue;

							yield return disp;
						}
					}
				}
			}
	}


	private Result<DisponibilidadEspecialidad2025> _EncontrarProximaDisponibilidad(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion
	) {
        DisponibilidadEspecialidad2025? proxima = _GenerarDisponibilidades(solicitudEspecialidad, solicitudFechaCreacion).FirstOrDefault();
		if (proxima is null)
			return new Result<DisponibilidadEspecialidad2025>.Error("No se encontraron disponibilidades");
		return new Result<DisponibilidadEspecialidad2025>.Ok((DisponibilidadEspecialidad2025)proxima);
	}



	private Result<Turno2025> _CambiarEstadoInterno(
		Turno2025 turnoOriginal,
		TurnoOutcomeEstado2025 outcomeEstado,
		DateTime outcomeFecha,
		string outcomeComentario
	) {
		int idx = Turnos.FindIndex(t => t.Id == turnoOriginal.Id);
		if (idx == -1)
			return new Result<Turno2025>.Error("El turno no existe en esta ListaTurnos.");

        Result<Turno2025> nuevoTurnoResult = turnoOriginal.CambiarEstado(outcomeEstado, outcomeFecha, outcomeComentario);
		if (nuevoTurnoResult.IsOk) {
			Turnos.RemoveAt(idx);
			Turnos.Add(((Result<Turno2025>.Ok)nuevoTurnoResult).Valor);
		}
		return nuevoTurnoResult;
	}

	private bool _DisponibilidadNoColisiona(MedicoId medicoId, EspecialidadMedica2025 especialidad, DateTime fechaHoraDesde, DateTime fechaHoraHasta) {
		//Faltaria validacion medicoId in Medicos

		foreach (Turno2025 turno in Turnos) {

			// 1) Debe ser del mismo médico y especialidad
			if (turno.MedicoId != medicoId) continue;
			if (turno.Especialidad != especialidad) continue;

			// 2) Solo los turnos programados bloquean agenda
			if (turno.OutcomeEstado != TurnoOutcomeEstado2025.Programado) continue;

			// 3) Chequeo de solapamiento clásico
			bool solapa =
				turno.FechaHoraAsignadaDesde < fechaHoraHasta &&
				fechaHoraDesde < turno.FechaHoraAsignadaHasta;

			if (solapa)
				return false;
		}

		return true;
	}

	public Result<Turno2025> SolicitarTurnoEnLaPrimeraDisponibilidad(PacienteId pacienteId, EspecialidadMedica2025 especialidadMedica, DateTime when) {
		//Faltaria validacion pacienteId in Pacientes

		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1 = _EncontrarProximaDisponibilidad(especialidadMedica, when);

		Result<Turno2025> turno = Turno2025.Crear(new TurnoId(1), pacienteId, when, disponibilidadParaPaciente1);

		_AgendarTurno(turno).PrintAndContinue("Agendando turno: ");

		return turno;
	}

	public Result<Turno2025> SolicitarReprogramacionALaPrimeraDisponibilidad(Turno2025 turnoOriginal, DateTime when, string why) {
        Result<Turno2025> turnoCanceladoResult = _CambiarEstadoInterno(turnoOriginal, TurnoOutcomeEstado2025.Reprogramado, when,why);
		if (turnoCanceladoResult.IsError) return turnoCanceladoResult;
		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)turnoCanceladoResult).Valor;

		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1_reprogramado = _EncontrarProximaDisponibilidad(turnoCancelado.Especialidad, when);


		Result<Turno2025> nuevoTurno = Turno2025.Crear(new TurnoId(2), turnoCancelado.PacienteId, when, disponibilidadParaPaciente1_reprogramado)
			.PrintAndContinue("Creando nuevo turno: ");

		this._AgendarTurno(nuevoTurno)
			.PrintAndContinue("Agendando nuevo turno: ");

		return nuevoTurno;
	}

	public Result<Turno2025> SolicitarCancelacion(Turno2025 turnoOriginal, DateTime solicitudFecha, string solicitudReason) {
		return _CambiarEstadoInterno(turnoOriginal, TurnoOutcomeEstado2025.Cancelado, solicitudFecha, solicitudReason);
	}
}


public static class DisponibilidadesExtensions {

	public static Result<DisponibilidadEspecialidad2025> TomarPrimera(this Result<IReadOnlyList<DisponibilidadEspecialidad2025>> listadoResult) {
		return listadoResult.Match<Result<DisponibilidadEspecialidad2025>>(
			ok => {
				// la lista existe, ahora chequeamos si tiene elementos
				if (ok.Count == 0)
					return new Result<DisponibilidadEspecialidad2025>.Error(
						"No hay disponibilidades para seleccionar."
					);
				return new Result<DisponibilidadEspecialidad2025>.Ok(ok[0]);
			},
			mensajeError =>
				new Result<DisponibilidadEspecialidad2025>.Error(mensajeError)
		);
	}

	public static Result<IReadOnlyList<DisponibilidadEspecialidad2025>> AplicarFiltrosOpcionales(this Result<IReadOnlyList<DisponibilidadEspecialidad2025>> disponibilidadesResult, SolicitudDeTurnoPreferencias preferencias) {
		if (disponibilidadesResult is Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error err)
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error(err.Mensaje);

		IReadOnlyList<DisponibilidadEspecialidad2025> lista = ((Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok)disponibilidadesResult).Valor;
		IEnumerable<DisponibilidadEspecialidad2025> filtradas = lista;
		if (preferencias.DiaPreferido is DiaSemana2025 dia)
			filtradas = filtradas.Where(d => d.FechaHoraDesde.DayOfWeek == dia.Valor);

		if (preferencias.MomentoPreferido is TardeOMañana momento)
			filtradas = filtradas.Where(d => momento.AplicaA(d.FechaHoraDesde));

		if (filtradas.Any()) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok([.. filtradas]);
		} else {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error("No se encontraron disponibilidades");
		}
	}
}
