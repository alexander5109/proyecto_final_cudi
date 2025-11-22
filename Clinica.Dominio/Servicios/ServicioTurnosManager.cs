using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.Servicios;

public class ServicioTurnosManager(
	List<Turno2025> Valores
) {
	public static ServicioTurnosManager Crear() => new([]);

	public Result<ServicioTurnosManager> AgendarTurno(Result<Turno2025> turnoResult) {
		return turnoResult.Match<Result<ServicioTurnosManager>>(
			ok => {
				Valores.Add(ok);
				return new Result<ServicioTurnosManager>.Ok(this);
			},
			mensaje => new Result<ServicioTurnosManager>.Error(mensaje)
		);
	}

	public Result<ServicioTurnosManager> CancelarTurno(
		Result<Turno2025> turnoResult,
		Option<DateTime> fecha,
		Option<string> comentario
	) {
		return CambiarEstadoInterno(
			turnoResult,
			TurnoOutcomeEstado2025.Cancelado,
			fecha,
			comentario
		);
	}
	public Result<ServicioTurnosManager> MarcarTurnoComoAusente(
		Result<Turno2025> turnoResult,
		Option<DateTime> fecha,
		Option<string> comentario
	) {
		return CambiarEstadoInterno(
			turnoResult,
			TurnoOutcomeEstado2025.Ausente,
			fecha,
			comentario
		);
	}


	//public Result<(ServicioTurnosManager Lista, Turno2025 NuevoTurno)> ReprogramarTurno(
	//	Result<Turno2025> turnoResult,
	//	Result<DisponibilidadEspecialidad2025> dispResult,
	//	Option<DateTime> fecha,
	//	Option<string> comentario
	//) {
	//	var cambio = CambiarEstadoInterno(
	//		turnoResult,
	//		TurnoOutcomeEstado2025.Reprogramado,
	//		fecha,
	//		comentario
	//	);

	//	if (cambio is Result<ServicioTurnosManager>.Error err)
	//		return new Result<(ServicioTurnosManager, Turno2025)>.Error(err.Mensaje);

	//	var turnoViejo = turnoResult.GetOrRaise();
	//	var disp = dispResult.GetOrRaise();

		//var nuevoTurnoResult = Turno2025.ReprogramarDesde(turnoViejo, disp);

	//	if (nuevoTurnoResult is Result<Turno2025>.Error err2)
	//		return new Result<(ServicioTurnosManager, Turno2025)>.Error(err2.Mensaje);

	//	var nuevoTurno = ((Result<Turno2025>.Ok)nuevoTurnoResult).Valor;

	//	Valores.Add(nuevoTurno);

	//	return new Result<(ServicioTurnosManager Lista, Turno2025 NuevoTurno)>.Ok((this, nuevoTurno));
	//}


	public Result<ServicioTurnosManager> MarcarTurnoComoConcretado(
		Result<Turno2025> turnoResult,
		Option<DateTime> fecha,
		Option<string> comentario
	) {
		return CambiarEstadoInterno(
			turnoResult,
			TurnoOutcomeEstado2025.Concretado,
			fecha,
			comentario
		);
	}

	private Result<ServicioTurnosManager> CambiarEstadoInterno(
		Result<Turno2025> turnoResult,
		TurnoOutcomeEstado2025 outcomeEstado,
		Option<DateTime> outcomeFecha,
		Option<string> outcomeComentario
	) {
		return turnoResult.Match<Result<ServicioTurnosManager>>(
			turnoOriginal => {
				// 1) Encontrar el turno en la lista
				int idx = Valores.FindIndex(t => t.Id == turnoOriginal.Id);
				if (idx == -1)
					return new Result<ServicioTurnosManager>.Error("El turno no existe en esta ListaTurnos.");
				// 2) Intentar cambiar estado
				Result<Turno2025> nuevoTurnoResult = turnoOriginal.CambiarEstado(outcomeEstado, outcomeFecha, outcomeComentario);
				return nuevoTurnoResult.Match<Result<ServicioTurnosManager>>(
					nuevoTurno => {
						// 3) Remover el turno viejo
						Valores.RemoveAt(idx);
						// 4) Insertar el turno actualizado
						Valores.Add(nuevoTurno);
						return new Result<ServicioTurnosManager>.Ok(this);
					},
					mensajeError =>
						new Result<ServicioTurnosManager>.Error(mensajeError)
				);
			},
			mensaje => new Result<ServicioTurnosManager>.Error(mensaje)
		);
	}

	public bool DisponibilidadNoColisiona(MedicoId medicoId, EspecialidadMedica2025 especialidad, DateTime fechaHoraDesde, DateTime fechaHoraHasta) {
		foreach (var turno in Valores) {

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
}
