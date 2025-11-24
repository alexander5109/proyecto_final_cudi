using System;
using System.Collections.Generic;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.Dtos;
using Clinica.Infrastructure.Persistencia;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Clinica.Infrastructure.ServiciosAsync;

public partial class ServiciosPublicosAsync(BaseDeDatosRepositorio baseDeDatos) {
	private readonly BaseDeDatosRepositorio BaseDeDatos = baseDeDatos;


	public async Task<Result<Turno2025>> SolicitarTurnoEnLaPrimeraDisponibilidad(
		PacienteId pacienteId,
		EspecialidadMedica2025 especialidad,
		DateTime fechaSolicitud
	) {
		//IEnumerable<MedicoDto> medicosDtos = await BaseDeDatos.SelectMedicosWhereEspecialidad(especialidad);
		//IEnumerable<TurnoDto> turnosDtos = await BaseDeDatos.SelectTurnosWhereMedicosIds(
		//medicosDtos.Select(medicoDto => medicoDto.Id)
		//);

		//IReadOnlyList<Result<Medico2025>> medicos = [.. medicosDtos.Select(medicoDto => medicoDto.ToDomain())];
		//IReadOnlyList<Result<Turno2025>> turnos = [.. turnosDtos.Select(turnoDto => turnoDto.ToDomain())];

		// 3. Ejecuta el dominio
		return await ServiciosPublicos.SolicitarTurnoEnLaPrimeraDisponibilidad(
			pacienteId,
			especialidad,
			fechaSolicitud,
			funcSelectMedicosWhereEspecialidad: FunctorSelectMedicosWhereEspecialidad(),
			funcSelectHorariosWhereMedicoIdInVigencia: FunctorSelectHorariosWhereMedicoIdInVigencia(),
			funcSelectTurnosWhereMedicoIdBetweenFechas: FunctorSelectTurnosWhereMedicoIdBetweenFechas(),
			funcInsertTurnoReturnId: BaseDeDatos.InsertTurnoReturnId
		);
	}


	public async Task<Result<Turno2025>> SolicitarReprogramacionALaPrimeraDisponibilidad(
		Result<Turno2025> turnoOriginalResult,
		DateTime outcomeFecha,
		string outcomeComentario
	) {
		switch (turnoOriginalResult) {
			case Result<Turno2025>.Error turnoError: {
				return new Result<Turno2025>.Error($"SolicitarReprogramacionALaPrimeraDisponibilidad fallido porque turno ya traia error: \n{turnoError.Mensaje}");
			}
			case Result<Turno2025>.Ok turnoOk: {
				//IEnumerable<MedicoDto> medicosDtos = await BaseDeDatos.SelectMedicosWhereEspecialidad(turnoOk.Valor.Especialidad);
				//IEnumerable<TurnoDto> turnosDtos = await BaseDeDatos.SelectTurnosWhereMedicosIds(
				//	medicosDtos.Select(medicoDto => medicoDto.Id)
				//);
				//IReadOnlyList<Result<Medico2025>> medicosResults = [.. medicosDtos.Select(medicoDto => medicoDto.ToDomain())];
				//IReadOnlyList<Result<Turno2025>> turnosResults = [.. turnosDtos.Select(turnoDto => turnoDto.ToDomain())];


				//Result<IReadOnlyList<Medico2025>> medicosVerificados = ServiciosPrivados._ValidarMedicos(medicosResults);
				//if (medicosVerificados.IsError) {
				//	return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Medico2025>>.Error)medicosVerificados).Mensaje}");
				//}
				//IReadOnlyList<Medico2025> medicos = ((Result<IReadOnlyList<Medico2025>>.Ok)medicosVerificados).Valor;


				//Result<IReadOnlyList<Turno2025>> turnosVerificados = ServiciosPrivados._ValidarTurnos(turnosResults);
				//if (turnosVerificados.IsError) {
				//	return new Result<Turno2025>.Error($"Error en la lista de turnos: {((Result<IReadOnlyList<Turno2025>>.Error)turnosVerificados).Mensaje}");
				//}
				//IReadOnlyList<Turno2025> turnos = ((Result<IReadOnlyList<Turno2025>>.Ok)turnosVerificados).Valor;


				return await ServiciosPublicos.SolicitarReprogramacionALaPrimeraDisponibilidad(
					turnoOk.Valor,
					outcomeFecha,
					outcomeComentario,
					funcSelectMedicosWhereEspecialidad: FunctorSelectMedicosWhereEspecialidad(),
					funcSelectHorariosWhereMedicoIdInVigencia: FunctorSelectHorariosWhereMedicoIdInVigencia(),
					funcSelectTurnosWhereMedicoIdBetweenFechas: FunctorSelectTurnosWhereMedicoIdBetweenFechas(),
					funcUpdateTurnoWhereId: BaseDeDatos.UpdateTurnoWhereId,
					funcInsertTurnoReturnId: BaseDeDatos.InsertTurnoReturnId
				);
			}
			default: throw new InvalidOperationException(); //impossible to occur
		}
	}

	public async Task<Result<Turno2025>> SolicitarCancelacion(Result<Turno2025> turnoOriginalResult, DateTime outcomeFecha, string outcomeComentario) {
		switch (turnoOriginalResult) {
			case Result<Turno2025>.Error turnoError: {
				return new Result<Turno2025>.Error($"SolicitarCancelacion fallido porque turno ya traia error: \n{turnoError.Mensaje}");
			}
			case Result<Turno2025>.Ok turnoOk: {
				IEnumerable<MedicoDto> medicosDtos = await BaseDeDatos.SelectMedicosWhereEspecialidad(turnoOk.Valor.Especialidad);
				return await ServiciosPublicos.SolicitarCancelacion(
					turnoOriginalResult,
					outcomeFecha,
					outcomeComentario,
					funcUpdateTurnoWhereId: BaseDeDatos.UpdateTurnoWhereId
				);
			}
			default: throw new InvalidOperationException(); //impossible to occur
		}
	}


}
