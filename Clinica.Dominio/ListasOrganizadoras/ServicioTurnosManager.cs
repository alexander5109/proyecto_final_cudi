using System.Collections.ObjectModel;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.ListasOrganizadoras;

public class ServicioTurnosManager(
	List<Turno2025> Turnos,
	IReadOnlyList<Medico2025> Medicos,
	IReadOnlyList<Paciente2025> Pacientes
) {
	public static Result<ServicioTurnosManager> Crear(
		List<Result<Turno2025>> turnos,
		IReadOnlyList<Result<Medico2025>> medicos,
		IReadOnlyList<Result<Paciente2025>> pacientes
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
		foreach (Result<Paciente2025> paciente in pacientes) {
			if (paciente is Result<Paciente2025>.Error err)
				errores.Add(err.Mensaje);
			paciente.PrintAndContinue("Paciente domainizado");
		}

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

		ReadOnlyCollection<Paciente2025> pacientesOk = pacientes
			.Cast<Result<Paciente2025>.Ok>()
			.Select(ok => ok.Valor)
			.ToList()
			.AsReadOnly();

		// --- crear instancia ---
		return new Result<ServicioTurnosManager>.Ok(
			new ServicioTurnosManager(turnosOk, medicosOk, pacientesOk)
		);
	}
	public Result<ServicioTurnosManager> AgendarTurno(Result<Turno2025> turnoResult) {
		return turnoResult.Match<Result<ServicioTurnosManager>>(
			ok => {
				Turnos.Add(ok);
				return new Result<ServicioTurnosManager>.Ok(this);
			},
			mensaje => new Result<ServicioTurnosManager>.Error(mensaje)
		);
	}

	//public Result<Turno2025> SolicitarYAgendarTurno(
	//	Result<SolicitudDeTurno> solicitud, int cantidadDias) {
	//	var disp = ListaDisponibilidades2025
	//		.Buscar(solicitud, _medicos, this, cantidadDias)
	//		.TomarPrimera();

	//	return disp.Then(d => Turno2025.Crear(solicitud, d))
	//			   .Then(turno => AgendarTurno(turno));
	//}




	public Result<ServicioTurnosManager> CancelarTurno(
		Result<Turno2025> turnoResult,
		DateTime fecha,
		string comentario
	) {
		return CambiarEstadoInterno(
			turnoResult,
			TurnoOutcomeEstado2025.Cancelado,
			Option<DateTime>.Some(fecha),
			Option<string>.Some(comentario)
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

	//	Turnos.Add(nuevoTurno);

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
				int idx = Turnos.FindIndex(t => t.Id == turnoOriginal.Id);
				if (idx == -1)
					return new Result<ServicioTurnosManager>.Error("El turno no existe en esta ListaTurnos.");
				// 2) Intentar cambiar estado
				Result<Turno2025> nuevoTurnoResult = turnoOriginal.CambiarEstado(outcomeEstado, outcomeFecha, outcomeComentario);
				return nuevoTurnoResult.Match<Result<ServicioTurnosManager>>(
					nuevoTurno => {
						// 3) Remover el turno viejo
						Turnos.RemoveAt(idx);
						// 4) Insertar el turno actualizado
						Turnos.Add(nuevoTurno);
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


		Result<SolicitudDeTurno> solicitudPaciente1 = SolicitudDeTurno.Crear(pacienteId, especialidadMedica, when)
			.PrintAndContinue($"paciente Id <{pacienteId}> intenta solicitar turno: ");

		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1 = ServicioDisponibilidadesSearcher
			.Buscar(solicitudPaciente1, Medicos, this, 3)
			.PrintAndContinue("Buscando disponibilidades: ")
			//.AplicarFiltrosOpcionales(new(DiaSemana2025.Lunes, new TardeOMañana(false)))
			//.PrintAndContinue("AplicarFiltrosOpcionales: ")
			.TomarPrimera()
			.PrintAndContinue("Tomando la primera: ");

		Result<Turno2025> turno = Turno2025.Crear(new TurnoId(1), solicitudPaciente1, disponibilidadParaPaciente1)
			.PrintAndContinue("Creando turno: ");

		this.AgendarTurno(turno).PrintAndContinue("Agendando turno: ");

		return turno;
	}

	public Result<Turno2025> SolicitarReprogramacionALaPrimeraDisponibilidad(Turno2025 turno, DateTime dateTime) {

		// Paciente quiere reprogramar.
		Result<SolicitudDeTurno> reprogramacion = SolicitudDeTurno.Crear(
			turno.PacienteId, //el mismo paciente
			turno.Especialidad,
			dateTime
		).PrintAndContinue("paciente1 intenta solicitar un nuevo turno: ");

		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1_reprogramado = ServicioDisponibilidadesSearcher
			.Buscar(reprogramacion, Medicos, this, 3)
			.PrintAndContinue("Buscando disponibilidades: ")
			//.AplicarFiltrosOpcionales(new(DiaSemana2025.Lunes, new TardeOMañana(false)))
			//.PrintAndContinue("AplicarFiltrosOpcionales: ")
			.TomarPrimera()
			.PrintAndContinue("Tomando la primera: ");


		Result<Turno2025> nuevoTurno = Turno2025.Crear(new TurnoId(2), reprogramacion, disponibilidadParaPaciente1_reprogramado)
			.PrintAndContinue("Creando nuevo turno: ");

		this.AgendarTurno(nuevoTurno)
			.PrintAndContinue("Agendando nuevo turno: ");

		return nuevoTurno;
	}
}
