using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.Servicios;

public class ServiciosPublicos : IServiciosPublicos {
	async Task<Result<Usuario2025Agg>> IServiciosAuth.ValidarCredenciales(string username, string password, IRepositorioDomainServiciosPrivados repositorio) {
		Result<Usuario2025Agg> resultadoUsuario =
			await repositorio.SelectUsuarioWhereNombreAsDomain(new NombreUsuario(username));

		return resultadoUsuario.MatchAndSet(
			okValue => okValue.Usuario.PasswordMatch(password)
						? new Result<Usuario2025Agg>.Ok(okValue)
						: new Result<Usuario2025Agg>.Error("Usuario o contraseña incorrectos"),
			err => resultadoUsuario
		);
	}



	async Task<Result<IReadOnlyList<Disponibilidad2025>>> IServiciosDisponibilidades.SolicitarDisponibilidades(EspecialidadCodigo solicitudEspecialidadCodigo, DateTime aPartirDeCuando, int cuantos, IRepositorioDomainServiciosPrivados repositorio) {

		if (cuantos > 50) {
			return new Result<IReadOnlyList<Disponibilidad2025>>.Error("No vamos a producir tantas disponibilidades. Si quiere, adelante la fecha");
		}

		Result<Especialidad2025> solicitudEspecialidadResult = Especialidad2025.CrearResultPorCodigoInterno(solicitudEspecialidadCodigo);
		if (solicitudEspecialidadResult.IsError) return new Result<IReadOnlyList<Disponibilidad2025>>.Error(solicitudEspecialidadResult.UnwrapAsError());
		Especialidad2025 solicitudEspecialidad = solicitudEspecialidadResult.UnwrapAsOk();


		List<Disponibilidad2025> lista = new(capacity: cuantos);

		await foreach (Result<Disponibilidad2025> dispResult in
			_ServiciosPrivados.GenerarDisponibilidades(
				solicitudEspecialidad,
				aPartirDeCuando,
				repositorio)) {
			if (dispResult.IsError) {
				// Propagamos el error aguas arriba
				return new Result<IReadOnlyList<Disponibilidad2025>>
					.Error(dispResult.UnwrapAsError());
			}

			lista.Add(dispResult.UnwrapAsOk());

			if (lista.Count >= cuantos)
				break;
		}

		if (lista.Count > 0) {
			return new Result<IReadOnlyList<Disponibilidad2025>>.Ok(lista);
		}

		return new Result<IReadOnlyList<Disponibilidad2025>>.Error("No se encontraron disponibilidades.");
	}






	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.CancelarTurnoAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string outcomeComentario,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		Result<Turno2025Agg> turnoOriginalResult = await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId);
		if (turnoOriginalResult.IsError) return new Result<Turno2025Agg>.Error(turnoOriginalResult.UnwrapAsError());
		Turno2025Agg agggrgOriginal = turnoOriginalResult.UnwrapAsOk();
		// 1. Aplicar regla de dominio para cancelar
		Result<Turno2025> canceladoResult = agggrgOriginal.Turno.SetOutcome(
			TurnoOutcomeEstado2025.Cancelado,
			outcomeFecha,
			outcomeComentario
		);
		if (canceladoResult.IsError)
			return new Result<Turno2025Agg>.Error(canceladoResult.UnwrapAsError());
		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)canceladoResult).Valor;

		// 2. Guardar cambios (IO)
		Result<Turno2025Agg> updateResult = await repositorio.UpdateTurnoWhereId(turnoOriginalId, turnoCancelado);
		if (updateResult.IsError)
			return new Result<Turno2025Agg>.Error($"Error al persistir la cancelación del turno: {updateResult.UnwrapAsError()}");

		return new Result<Turno2025Agg>.Ok(updateResult.UnwrapAsOk());
	}







	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.ReprogramarTurnoAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string outcomeComentario,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		Result<Turno2025Agg> turnoOriginalResult = await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId);
		if (turnoOriginalResult.IsError) return turnoOriginalResult;
		//if (turnoOriginalResult.IsError) return new Result<Turno2025>.Error($"No se encontró el turno original: {turnoOriginalResult.UnwrapAsError()}");
		Turno2025Agg aggrgOriginal = turnoOriginalResult.UnwrapAsOk();
		Result<Turno2025> canceladoResult = aggrgOriginal.Turno.SetOutcome(TurnoOutcomeEstado2025.Reprogramado, outcomeFecha, outcomeComentario);
		if (canceladoResult.IsError) return new Result<Turno2025Agg>.Error($"Error de dominio:: \n\t{canceladoResult.UnwrapAsError()}");
		//if (canceladoResult.IsError) return new Result<Turno2025>.Error($"No se puede cancelar el turno: {canceladoResult.UnwrapAsError()}");
		Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)canceladoResult).Valor;
		Result<Turno2025Agg> updateResult = await repositorio.UpdateTurnoWhereId(turnoOriginalId, turnoCancelado);
		if (updateResult.IsError) return new Result<Turno2025Agg>.Error($"Error al persistir la cancelación del turno: \n\t{updateResult.UnwrapAsError()}");
		Result<Disponibilidad2025> dispResult = await _ServiciosPrivados.EncontrarProximaDisponibilidad(aggrgOriginal.Turno.Especialidad, outcomeFecha, repositorio);
		if (dispResult is Result<Disponibilidad2025>.Error e3) return new Result<Turno2025Agg>.Error(e3.Mensaje);
		Disponibilidad2025 disponibilidad = ((Result<Disponibilidad2025>.Ok)dispResult).Valor;
		Result<Turno2025> provResult = aggrgOriginal.Turno.Reprogramar(disponibilidad);
		if (provResult.IsError) return new Result<Turno2025Agg>.Error(provResult.UnwrapAsError());
		Turno2025 turnoTentativo = provResult.UnwrapAsOk();
		Result<TurnoId> turnoConfirmado = await repositorio.InsertTurnoReturnId(turnoTentativo);
		if (turnoConfirmado.IsError) return new Result<Turno2025Agg>.Error($"Error al persistir el nuevo turno reprogramado: {turnoConfirmado.UnwrapAsError()}");
		TurnoId idReal = ((Result<TurnoId>.Ok)turnoConfirmado).Valor;
		return new Result<Turno2025Agg>.Ok(Turno2025Agg.Crear(idReal, turnoTentativo));
	}











	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.MarcarComoAusente(
		TurnoId turnoOriginalId, 
		DateTime outcomeFecha, 
		string outcomeComentario, 
		IRepositorioDomainServiciosPrivados repositorio
	) {
		throw new NotImplementedException();
	}








	async Task<Result<Turno2025Agg>> IServiciosDisponibilidades.AgendarTurnoAsync(
		PacienteId pacienteId,
		DateTime fechaSolicitud,
		Disponibilidad2025 disponibilidad,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		Turno2025 turno = Turno2025.ProgramarNuevo(
			pacienteId,
			FechaRegistro2025.Crear(fechaSolicitud),
			disponibilidad
		).UnwrapAsOk();

	}












	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.MarcarComoConcretado(
		TurnoId turnoOriginalId, 
		DateTime outcomeFecha, 
		string? outcomeComentario, 
		IRepositorioDomainServiciosPrivados repositorio
	) {
		Result<Turno2025Agg> turnoOriginalResult = await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId);
		if (turnoOriginalResult.IsError) return new Result<Turno2025Agg>.Error(turnoOriginalResult.UnwrapAsError());
		Turno2025Agg agggrgOriginal = turnoOriginalResult.UnwrapAsOk();








	}

}