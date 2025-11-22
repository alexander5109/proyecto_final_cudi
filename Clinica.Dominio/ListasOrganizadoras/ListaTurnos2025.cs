using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.ListasOrganizadoras;

public class ListaTurnos2025(
	List<Turno2025> Valores
) {
	public static ListaTurnos2025 Crear() => new([]);

	public Result<ListaTurnos2025> AgendarTurno(Result<Turno2025> turnoResult) {
		return turnoResult.Match<Result<ListaTurnos2025>>(
			ok => {
				Valores.Add(ok);
				return new Result<ListaTurnos2025>.Ok(this);
			},
			mensaje => new Result<ListaTurnos2025>.Error(mensaje)
		);
	}

	public Result<ListaTurnos2025> CancelarTurno(
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
	public Result<ListaTurnos2025> MarcarTurnoComoAusente(
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


	//public Result<(ListaTurnos2025 Lista, Turno2025 NuevoTurno)> ReprogramarTurno(
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

	//	if (cambio is Result<ListaTurnos2025>.Error err)
	//		return new Result<(ListaTurnos2025, Turno2025)>.Error(err.Mensaje);

	//	var turnoViejo = turnoResult.GetOrRaise();
	//	var disp = dispResult.GetOrRaise();

		//var nuevoTurnoResult = Turno2025.ReprogramarDesde(turnoViejo, disp);

	//	if (nuevoTurnoResult is Result<Turno2025>.Error err2)
	//		return new Result<(ListaTurnos2025, Turno2025)>.Error(err2.Mensaje);

	//	var nuevoTurno = ((Result<Turno2025>.Ok)nuevoTurnoResult).Valor;

	//	Valores.Add(nuevoTurno);

	//	return new Result<(ListaTurnos2025 Lista, Turno2025 NuevoTurno)>.Ok((this, nuevoTurno));
	//}


	public Result<ListaTurnos2025> MarcarTurnoComoConcretado(
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

	private Result<ListaTurnos2025> CambiarEstadoInterno(
		Result<Turno2025> turnoResult,
		TurnoOutcomeEstado2025 outcomeEstado,
		Option<DateTime> outcomeFecha,
		Option<string> outcomeComentario
	) {
		return turnoResult.Match<Result<ListaTurnos2025>>(
			turnoOriginal => {
				// 1) Encontrar el turno en la lista
				int idx = Valores.FindIndex(t => t.Id == turnoOriginal.Id);
				if (idx == -1)
					return new Result<ListaTurnos2025>.Error("El turno no existe en esta ListaTurnos.");
				// 2) Intentar cambiar estado
				Result<Turno2025> nuevoTurnoResult = turnoOriginal.CambiarEstado(outcomeEstado, outcomeFecha, outcomeComentario);
				return nuevoTurnoResult.Match<Result<ListaTurnos2025>>(
					nuevoTurno => {
						// 3) Remover el turno viejo
						Valores.RemoveAt(idx);
						// 4) Insertar el turno actualizado
						Valores.Add(nuevoTurno);
						return new Result<ListaTurnos2025>.Ok(this);
					},
					mensajeError =>
						new Result<ListaTurnos2025>.Error(mensajeError)
				);
			},
			mensaje => new Result<ListaTurnos2025>.Error(mensaje)
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
