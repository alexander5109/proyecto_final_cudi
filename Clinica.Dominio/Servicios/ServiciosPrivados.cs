using System;
using System.Collections.Generic;
using System.Text;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Servicios;

public static class ServiciosPrivados {
	//------------------------------------------------PRIVATE-------------------------------------------------------//
	public static Result<IReadOnlyList<Turno2025>> _ValidarTurnos(IReadOnlyList<Result<Turno2025>> turnosResults) {
		List<string> errores = [];
		foreach (Result<Turno2025> turno in turnosResults) {
			if (turno is Result<Turno2025>.Error err)
				errores.Add(err.Mensaje);
			turno.PrintAndContinue("Turno domainizado");
		}
		if (errores.Count > 0)
			return new Result<IReadOnlyList<Turno2025>>.Error(string.Join("; ", errores));
		IReadOnlyList<Turno2025> turnosOk = [.. turnosResults
			.Cast<Result<Turno2025>.Ok>()
			.Select(ok => ok.Valor)];
		return new Result<IReadOnlyList<Turno2025>>.Ok(turnosOk);
	}

	public static Result<IReadOnlyList<Paciente2025>> _ValidaPacientes(IReadOnlyList<Result<Paciente2025>> pacientesResults) {
		List<string> errores = [];
		foreach (Result<Paciente2025> paciente in pacientesResults) {
			if (paciente is Result<Paciente2025>.Error err)
				errores.Add(err.Mensaje);
			paciente.PrintAndContinue("Paciente domainizado");
		}
		if (errores.Count > 0)
			return new Result<IReadOnlyList<Paciente2025>>.Error(string.Join("; ", errores));
		IReadOnlyList<Paciente2025> pacientesOk = [.. pacientesResults
			.Cast<Result<Paciente2025>.Ok>()
			.Select(ok => ok.Valor)];
		return new Result<IReadOnlyList<Paciente2025>>.Ok(pacientesOk);
	}


	public static Result<IReadOnlyList<Medico2025>> _ValidarMedicos(IReadOnlyList<Result<Medico2025>> medicosResults) {
		List<string> errores = [];
		foreach (Result<Medico2025> medico in medicosResults) {
			if (medico is Result<Medico2025>.Error err)
				errores.Add(err.Mensaje);
			medico.PrintAndContinue("Medico domainizado");
		}
		if (errores.Count > 0)
			return new Result<IReadOnlyList<Medico2025>>.Error(string.Join("; ", errores));
		IReadOnlyList<Medico2025> medicosOk = [.. medicosResults
			.Cast<Result<Medico2025>.Ok>()
			.Select(ok => ok.Valor)];
		return new Result<IReadOnlyList<Medico2025>>.Ok(medicosOk);
	}

	public static Result<DisponibilidadEspecialidad2025> _EncontrarProximaDisponibilidad(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcMedicosPorEspecialidad,
		Func<MedicoId, IEnumerable<HorarioMedico2025>> funcHorariosDeMedico,
		Func<MedicoId, DateTime, IEnumerable<Turno2025>> funcTurnosDelMedicoDesde
	) {
		DisponibilidadEspecialidad2025? proxima = _GenerarDisponibilidades(
			solicitudEspecialidad,
			solicitudFechaCreacion,
			funcMedicosPorEspecialidad,
			funcHorariosDeMedico,
			funcTurnosDelMedicoDesde
		).FirstOrDefault();
		if (proxima is null)
			return new Result<DisponibilidadEspecialidad2025>.Error("No se encontraron proximaDisponibilidad");
		return new Result<DisponibilidadEspecialidad2025>.Ok((DisponibilidadEspecialidad2025)proxima);
	}




	public static bool _DisponibilidadNoColisiona(
		MedicoId medicoId,
		EspecialidadMedica2025 especialidad,
		DateTime fechaHoraDesde,
		DateTime fechaHoraHasta,
		Func<MedicoId, DateTime, IEnumerable<Turno2025>> funcTurnosDelMedicoDesde
	) {
		//Faltaria validacion medicoId in Medicos

		foreach (Turno2025 turno in turnos) {

			// 1) Debe ser del mismo médico y especialidad
			if (turno.MedicoId != medicoId) continue;
			if (turno.Especialidad != especialidad) continue;

			// 2) Solo los medicosResults programados bloquean agenda
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
	public static IEnumerable<DisponibilidadEspecialidad2025> _GenerarDisponibilidades(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		Func<EspecialidadMedica2025, IEnumerable<Medico2025>> funcMedicosPorEspecialidad,
		Func<MedicoId, IEnumerable<HorarioMedico2025>> funcHorariosDeMedico,
		Func<MedicoId, DateTime, IEnumerable<Turno2025>> funcTurnosDelMedicoDesde
	) {
		foreach (Medico2025 medico in medicos)
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
							if (!_DisponibilidadNoColisiona(disp.MedicoId, disp.Especialidad, disp.FechaHoraDesde, disp.FechaHoraHasta, turnos))
								continue;

							yield return disp;
						}
					}
				}
			}
	}








}
