using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.DtosEntidades;
using Clinica.Infrastructure.Persistencia;
using static Clinica.Infrastructure.DtosEntidades.DtosEntidades;


namespace Clinica.Infrastructure.ServiciosAsync;


public partial class ServiciosPublicosAsync(BaseDeDatosRepositorio BaseDeDatos) {

	public async Task<Result<Turno2025>> CancelarTurnoAsync(int id, Option<string> option) {
		throw new NotImplementedException();
	}

	public async Task<Result<Turno2025>> MarcarTurnoComoConcretadoAsync(int id, Option<string> option) {
		throw new NotImplementedException();
	}

	public async Task<Result<Turno2025>> MarcarTurnoComoAusenteAsync(int id, Option<string> option) {
		throw new NotImplementedException();
	}
	public async Task<Result<Turno2025>> AgendarTurnoAsync(int pacienteId, int medicoId, EspecialidadMedica2025 especialidad, DateTime desde, DateTime hasta) {
		throw new NotImplementedException();
	}

	public async Task<Result<Turno2025>> ReprogramarTurnoAsync(int id, object nuevaFechaDesde, object nuevaFechaHasta) {
		throw new NotImplementedException();
	}

	public async Task<Result<Turno2025>> ObtenerTurnoPorIdAsync(int id) {
		throw new NotImplementedException();
	}

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
			funcSelectHorariosVigentesBetweenFechasWhereMedicoId: FunctorSelectHorariosVigentesBetweenFechasWhereMedicoId(),
			funcSelectTurnosProgramadosBetweenFechasWhereMedicoId: FunctorSelectTurnosProgramadosBetweenFechasWhereMedicoId()
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
			funcSelectHorariosVigentesBetweenFechasWhereMedicoId: FunctorSelectHorariosVigentesBetweenFechasWhereMedicoId(),
			funcSelectTurnosProgramadosBetweenFechasWhereMedicoId: FunctorSelectTurnosProgramadosBetweenFechasWhereMedicoId(),
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
					funcSelectHorariosVigentesBetweenFechasWhereMedicoId: FunctorSelectHorariosVigentesBetweenFechasWhereMedicoId(),
					funcSelectTurnosWhereMedicoIdBetweenFechas: FunctorSelectTurnosProgramadosBetweenFechasWhereMedicoId(),
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


	//-------------------------PRIVATE--------------------------//

	private Func<EspecialidadMedica2025, IEnumerable<Medico2025>> FunctorSelectMedicosWhereEspecialidad() {
		return especialidad => {
			return Enumerar();
			IEnumerable<Medico2025> Enumerar() {
				IEnumerable<MedicoDto> dtos = BaseDeDatos.SelectMedicosWhereEspecialidad(especialidad).Result;
				foreach (MedicoDto dto in dtos) {
					Result<Medico2025> dom = dto.ToDomain();
					if (dom is Result<Medico2025>.Ok ok)
						yield return ok.Valor;
				}
			}
		};
	}


	private Func<MedicoId, DateTime, DateTime, IEnumerable<Turno2025>> FunctorSelectTurnosProgramadosBetweenFechasWhereMedicoId() {
		return (medicoId, fechaDesde, fechaHasta) => {
			return Enumerar();

			IEnumerable<Turno2025> Enumerar() {
				IEnumerable<TurnoDto> dtos = BaseDeDatos
					.SelectTurnosProgramadosBetweenFechasWhereMedicoId(medicoId, fechaDesde, fechaHasta)
					.Result;

				foreach (TurnoDto dto in dtos) {
					Result<Turno2025> dom = dto.ToDomain();
					if (dom is Result<Turno2025>.Ok ok)
						yield return ok.Valor;
				}
			}
		};
	}

	private Func<MedicoId, DateTime, DateTime, IEnumerable<HorarioMedico2025>> FunctorSelectHorariosVigentesBetweenFechasWhereMedicoId() {
		return (medicoId, fechaDesde, fechaHasta) => {
			return Enumerar();

			IEnumerable<HorarioMedico2025> Enumerar() {
				IEnumerable<HorarioMedicoDto> dtos = BaseDeDatos
					.SelectHorariosVigentesBetweenFechasWhereMedicoId(medicoId, fechaDesde, fechaHasta)
					.Result;

				foreach (HorarioMedicoDto dto in dtos) {
					Result<HorarioMedico2025> dom = dto.ToDomain();
					if (dom is Result<HorarioMedico2025>.Ok ok)
						yield return ok.Valor;
				}
			}
		};
	}
}
