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



	public async Task<Result<IReadOnlyList<DisponibilidadEspecialidad2025>>> SolicitarDisponibilidadesPara(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		int cuantos
	) {
		return await ServiciosPublicos.SolicitarDisponibilidadesPara(
			solicitudEspecialidad,
			solicitudFechaCreacion,
			cuantos,
			funcSelectMedicosWhereEspecialidad: FunctorSelectMedicosWhereEspecialidad(),
			funcSelectHorariosWhereMedicoIdInVigencia: FunctorSelectHorariosWhereMedicoIdInVigencia(),
			funcSelectTurnosWhereMedicoIdBetweenFechas: FunctorSelectTurnosWhereMedicoIdBetweenFechas()
		);
	}

	public async Task<Result<Turno2025>> SolicitarTurnoEnLaPrimeraDisponibilidad(
		PacienteId pacienteId,
		EspecialidadMedica2025 especialidad,
		FechaRegistro2025 fechaSolicitud
	) {
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
					turnoOk.Valor,
					outcomeFecha,
					outcomeComentario,
					funcUpdateTurnoWhereId: BaseDeDatos.UpdateTurnoWhereId
				);
			}
			default: throw new InvalidOperationException(); //impossible to occur
		}
	}


}
