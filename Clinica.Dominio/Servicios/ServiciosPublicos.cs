using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.Servicios;

public static class ServiciosPublicos {

	//CRUD paciente
	//CRUD medico
	//CRUD turno
	//public static async Task<Result<IReadOnlyList<DisponibilidadEspecialidad2025>>> Crear(


	//Task<Result<Turno2025>> AgendarTurnoAsync(PacienteId pacienteId, MedicoId medicoId, EspecialidadCodigo2025 especialidadCodigo, DateTime desde, DateTime hasta);

	//Task<Result<Turno2025>> CancelarTurnoAsync(TurnoId id, Option<string> motivo);

	//Task<Result<Turno2025>> ReprogramarTurnoAsync(TurnoId id, DateTime nuevaFechaDesde, DateTime nuevaFechaHasta);

	//Task<Result<Turno2025>> MarcarComoAusente(TurnoId id, Option<string> motivo);
	//Task<Result<Turno2025>> MarcarComoConcretado(TurnoId id, Option<string> motivo);



	public static async Task<Result<Medico2025>> SelectMedicoWhereId(
		UsuarioBase2025 usuario, 
		RepositorioInterface repositorio, 
		MedicoId id
	) {
			if (usuario.EnumRole is not UsuarioEnumRole.Nivel1Admin and not UsuarioEnumRole.Nivel2Secretaria) {
				return new Result<Medico2025>.Error("No cuenta con permisos para ver esta entidad");
			}

			// --- Delegar la obtención de datos ---
			return await repositorio.SelectMedicoWhereId(id);
		}

	public static async Task<Result<IEnumerable<Paciente2025>>> SelectPacientes(
		UsuarioBase2025 usuario,
		RepositorioInterface repositorio
	) {
		if (usuario.EnumRole is not UsuarioEnumRole.Nivel1Admin and not UsuarioEnumRole.Nivel2Secretaria) {
			return new Result<IEnumerable<Paciente2025>>.Error("No cuenta con permisos para ver esta entidad");
		}
		return await repositorio.SelectPacientes();
	}

	public static async Task<Result<IEnumerable<Turno2025>>> SelectTurnosWherePacienteId(
		UsuarioBase2025 usuario,
		RepositorioInterface repositorio,
		PacienteId id
	) {
		if (usuario.EnumRole is not UsuarioEnumRole.Nivel1Admin and not UsuarioEnumRole.Nivel2Secretaria) {
			return new Result<IEnumerable<Turno2025>>.Error("No cuenta con permisos para ver esta entidad");
		}
		return await repositorio.SelectTurnosWherePacienteId(id);
	}





	public static async Task<Result<UsuarioBase2025>> ValidarCredenciales(string username, string password, RepositorioInterface repositorio) {
		Result<UsuarioBase2025> resultadoUsuario = await repositorio.SelectUsuarioWhereName(new NombreUsuario(username));

		return resultadoUsuario.Match(
			usuarioOk => usuarioOk.PasswordMatch(password),
			notFound => resultadoUsuario
		);
	}

	public static async Task<Result<Turno2025>> SolicitarCancelacion(
		Result<Turno2025> turnoOriginalResult,
		DateTime outcomeFecha,
		string outcomeComentario,
		RepositorioInterface repositorio
	) {
		if (turnoOriginalResult.IsError) { return turnoOriginalResult; }
		Turno2025 turnoOriginal = turnoOriginalResult.UnwrapAsOk();
		// 1. Aplicar regla de dominio para cancelar
		Result<Turno2025> canceladoResult = turnoOriginal.SetOutcome(
			TurnoOutcomeEstado2025.Cancelado,
			outcomeFecha,
			outcomeComentario
		);

		if (canceladoResult is Result<Turno2025>.Error e1)
			return new Result<Turno2025>.Error(e1.Mensaje);

		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)canceladoResult).Valor;

		// 2. Guardar cambios (IO)
		Result<Unit> updateResult = await repositorio.UpdateTurnoWhereId(turnoCancelado);

		if (updateResult is Result<Unit>.Error e2)
			return new Result<Turno2025>.Error(
				$"Error al persistir la cancelación del turno: {e2.Mensaje}"
			);

		return new Result<Turno2025>.Ok(turnoCancelado);
	}


	public static async Task<Result<IReadOnlyList<DisponibilidadEspecialidad2025>>> SolicitarDisponibilidadesPara(
		EspecialidadMedica2025 solicitudEspecialidad,
		DateTime solicitudFechaCreacion,
		int cuantos,
		RepositorioInterface repositorio
	) {
		List<DisponibilidadEspecialidad2025> lista = new(capacity: cuantos);

		await foreach (DisponibilidadEspecialidad2025 disp in ServiciosPrivados.GenerarDisponibilidades(
			solicitudEspecialidad,
			solicitudFechaCreacion,
			repositorio
		)) {
			lista.Add(disp);

			if (lista.Count >= cuantos)
				break;
		}

		if (lista.Count > 0) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok(lista);
		} else {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error(
				"No se encontraron disponibilidades."
			);
		}
	}


	public static async Task<Result<Turno2025>> SolicitarTurnoEnLaPrimeraDisponibilidad(
		PacienteId pacienteId,
		EspecialidadMedica2025 solicitudEspecialidad,
		FechaRegistro2025 solicitudFechaCreacion,
		RepositorioInterface repositorio
	) {
		// 1. Buscar próxima disponibilidad
		Result<DisponibilidadEspecialidad2025> dispResult =
			await ServiciosPrivados.EncontrarProximaDisponibilidad(
				solicitudEspecialidad,
				solicitudFechaCreacion.Valor,
				repositorio
			);

		if (dispResult is Result<DisponibilidadEspecialidad2025>.Error errDisp)
			return new Result<Turno2025>.Error(errDisp.Mensaje);

		DisponibilidadEspecialidad2025 disp = ((Result<DisponibilidadEspecialidad2025>.Ok)dispResult).Valor;

		// 2. Crear turno provisorio desde el dominio
		Result<Turno2025> turnoResult = Turno2025.ProgramarNuevo(
			new TurnoId(-1),            // provisional
			pacienteId,
			solicitudFechaCreacion,
			disp
		);

		if (turnoResult is Result<Turno2025>.Error errTurno)
			return new Result<Turno2025>.Error(errTurno.Mensaje);

		Turno2025 turnoProvisorio = ((Result<Turno2025>.Ok)turnoResult).Valor;

		// 3. Persistir en la BD (insert que devuelve el TurnoId)
		Result<TurnoId> insertResult = await repositorio.InsertTurnoReturnId(turnoProvisorio);

		if (insertResult is Result<TurnoId>.Error errPersist)
			return new Result<Turno2025>.Error($"Error al persistir el nuevo turno: {errPersist.Mensaje}");

		TurnoId idReal = ((Result<TurnoId>.Ok)insertResult).Valor;

		// 4. Devolver el turno ya con su ID real seteado
		return new Result<Turno2025>.Ok(turnoProvisorio with { Id = idReal });
	}



	public static async Task<Result<Turno2025>> SolicitarReprogramacionALaPrimeraDisponibilidad(
		Result<Turno2025> turnoOriginalResult,
		DateTime outcomeFecha,
		string outcomeComentario,
		RepositorioInterface repositorio
	) {
		if (turnoOriginalResult.IsError) return turnoOriginalResult;
		Turno2025 turnoOriginal = turnoOriginalResult.UnwrapAsOk();

		Result<Turno2025> canceladoResult = turnoOriginal.SetOutcome(
			TurnoOutcomeEstado2025.Reprogramado,
			outcomeFecha,
			outcomeComentario
		);

		if (canceladoResult is Result<Turno2025>.Error e1)
			return new Result<Turno2025>.Error(e1.Mensaje);

		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)canceladoResult).Valor;


		Result<Unit> updateResult = await repositorio.UpdateTurnoWhereId(turnoCancelado);

		if (updateResult is Result<Unit>.Error e2)
			return new Result<Turno2025>.Error(
				$"Error al persistir la cancelación del turno: {e2.Mensaje}"
			);


		Result<DisponibilidadEspecialidad2025> dispResult = await ServiciosPrivados.EncontrarProximaDisponibilidad(
			turnoOriginal.Especialidad,
			outcomeFecha,
			repositorio
		);

		if (dispResult is Result<DisponibilidadEspecialidad2025>.Error e3)
			return new Result<Turno2025>.Error(e3.Mensaje);

		DisponibilidadEspecialidad2025 disponibilidad = ((Result<DisponibilidadEspecialidad2025>.Ok)dispResult).Valor;


		Result<Turno2025> provResult = turnoCancelado.Reprogramar(
			disponibilidad,
			new TurnoId(-1) // placeholder hasta persistencia
		);

		if (provResult is Result<Turno2025>.Error e4)
			return new Result<Turno2025>.Error(e4.Mensaje);

		Turno2025 turnoProvisorio = ((Result<Turno2025>.Ok)provResult).Valor;


		Result<TurnoId> insertResult = await repositorio.InsertTurnoReturnId(turnoProvisorio);

		if (insertResult is Result<TurnoId>.Error e5)
			return new Result<Turno2025>.Error(
				$"Error al persistir el nuevo turno reprogramado: {e5.Mensaje}"
			);

		TurnoId idReal = ((Result<TurnoId>.Ok)insertResult).Valor;


		return new Result<Turno2025>.Ok(
			turnoProvisorio with { Id = idReal }
		);
	}



	public static async Task<Result<Turno2025>> SolicitarCancelacion(
		Turno2025 turnoOriginal,
		DateTime outcomeFecha,
		string outcomeComentario,
		Func<Turno2025, Task<Result<Unit>>> funcUpdateTurnoWhereId
	) {
		// 1. Aplicar regla de dominio para cancelar
		Result<Turno2025> canceladoResult = turnoOriginal.SetOutcome(
			TurnoOutcomeEstado2025.Cancelado,
			outcomeFecha,
			outcomeComentario
		);

		if (canceladoResult is Result<Turno2025>.Error e1)
			return new Result<Turno2025>.Error(e1.Mensaje);

		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)canceladoResult).Valor;

		// 2. Guardar cambios (IO)
		Result<Unit> updateResult = await funcUpdateTurnoWhereId(turnoCancelado);

		if (updateResult is Result<Unit>.Error e2)
			return new Result<Turno2025>.Error(
				$"Error al persistir la cancelación del turno: {e2.Mensaje}"
			);

		return new Result<Turno2025>.Ok(turnoCancelado);
	}

}
