using System.Collections.Generic;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.Persistencia;


namespace Clinica.Infrastructure.ServiciosAsync;

public class ServicioCasosDeUsoAsync(
	MedicoRepository medicos,
	TurnoRepository turnos
) {
	private readonly MedicoRepository _medicos = medicos;
	private readonly TurnoRepository _turnos = turnos;

	public async Task<Result<Turno2025>> SolicitarTurnoEnLaPrimeraDisponibilidad(
		PacienteId pacienteId,
		EspecialidadMedica2025 especialidad,
		DateTime fechaSolicitud
	) {
		IEnumerable<MedicoDto> medicosDtos = await _medicos.GetPorEspecialidad(especialidad);
		IEnumerable<TurnoDto> turnosDtos = await _turnos.GetTurnosPorMedicos(
			medicosDtos.Select(medicoDto => medicoDto.Id)
		);

		IReadOnlyList<Result<Medico2025>> medicos = [.. medicosDtos.Select(medicoDto => medicoDto.ToDomain())];
		IReadOnlyList<Result<Turno2025>> turnos = [.. turnosDtos.Select(turnoDto => turnoDto.ToDomain())];

		// 3. Ejecuta el dominio
		return ServicioCasosDeUso.SolicitarTurnoEnLaPrimeraDisponibilidad(
			pacienteId,
			especialidad,
			fechaSolicitud,
			medicos,
			turnos
		);
	}

	public async Task<Result<Turno2025>> SolicitarReprogramacionALaPrimeraDisponibilidad(
		Result<Turno2025> turnoOriginalResult,
		DateTime when,
		string why
	) {
		switch (turnoOriginalResult) {
			case Result<Turno2025>.Error turnoError: {
				return new Result<Turno2025>.Error($"SolicitarReprogramacionALaPrimeraDisponibilidad fallido porque turno ya traia error: \n{turnoError.Mensaje}");
			}
			case Result<Turno2025>.Ok turnoOk: {
				IEnumerable<MedicoDto> medicosDtos = await _medicos.GetPorEspecialidad(turnoOk.Valor.Especialidad);
				IEnumerable<TurnoDto> turnosDtos = await _turnos.GetTurnosPorMedicos(
					medicosDtos.Select(medicoDto => medicoDto.Id)
				);
				IReadOnlyList<Result<Medico2025>> medicosResults = [.. medicosDtos.Select(medicoDto => medicoDto.ToDomain())];
				IReadOnlyList<Result<Turno2025>> turnosResults = [.. turnosDtos.Select(turnoDto => turnoDto.ToDomain())];
				return ServicioCasosDeUso.SolicitarReprogramacionALaPrimeraDisponibilidad(
					turnoOriginalResult,
					when,
					why,
					medicosResults,
					turnosResults
				);
			}
			default: throw new InvalidOperationException(); //impossible to occur
		}
	}

	public async Task<Result<Turno2025>> SolicitarCancelacion(Result<Turno2025> turnoOriginalResult, DateTime solicitudFecha, string solicitudReason) {
		switch (turnoOriginalResult) {
			case Result<Turno2025>.Error turnoError: {
				return new Result<Turno2025>.Error($"SolicitarCancelacion fallido porque turno ya traia error: \n{turnoError.Mensaje}");
			}
			case Result<Turno2025>.Ok turnoOk: {
				IEnumerable<MedicoDto> medicosDtos = await _medicos.GetPorEspecialidad(turnoOk.Valor.Especialidad);
				IEnumerable<TurnoDto> turnosDtos = await _turnos.GetTurnosPorMedicos(
					medicosDtos.Select(medicoDto => medicoDto.Id)
				);
				IReadOnlyList<Result<Turno2025>> turnosResults = [.. turnosDtos.Select(turnoDto => turnoDto.ToDomain())];
				return ServicioCasosDeUso.SolicitarCancelacion(
					turnoOriginalResult,
					solicitudFecha,
					solicitudReason,
					turnosResults
				);
			}
			default: throw new InvalidOperationException(); //impossible to occur
		}
	}


}
