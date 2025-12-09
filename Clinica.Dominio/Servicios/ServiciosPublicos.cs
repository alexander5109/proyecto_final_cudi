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

		Result<Especialidad2025> solicitudEspecialidadResult = Especialidad2025.CrearResult(solicitudEspecialidadCodigo);
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


















	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.PersistirComoAusenteAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string outcomeComentario,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		return await  (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			caseOk: aggOriginal => aggOriginal.Turno.MarcarComoAusente(outcomeFecha, outcomeComentario),
			prefixError: "Error de dominio: ")
		.BindWithPrefixAsync(
			caseOk: async turnoModificado => await repositorio.UpdateTurnoWhereId(turnoOriginalId, turnoModificado),
			prefixError: "Error de escritura en Db: ")
		;
	}


	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.PersistirComoConcretadoAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string? outcomeComentario,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		return await (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			caseOk: aggOriginal => aggOriginal.Turno.MarcarComoConcretado(outcomeFecha, outcomeComentario),
			prefixError: "Error de dominio: ")
		.BindWithPrefixAsync(
			caseOk: async turnoModificado => await repositorio.UpdateTurnoWhereId(turnoOriginalId, turnoModificado),
			prefixError: "Error de escritura en Db: ")
		;
	}



	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.PersistirComoCanceladoAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string outcomeComentario,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		return await (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			caseOk: aggOriginal => aggOriginal.Turno.MarcarComoCancelado(outcomeFecha, outcomeComentario),
			prefixError: "Error de dominio: ")
		.BindWithPrefixAsync(
			caseOk: async turnoModificado => await repositorio.UpdateTurnoWhereId(turnoOriginalId, turnoModificado),
			prefixError: "Error de escritura en Db: ")
		;
	}



	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.PersistirComoReprogramado(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string outcomeComentario,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		return await (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			caseOk: aggOriginal => aggOriginal.Turno.MarcarComoReprogramado(outcomeFecha, outcomeComentario),
			prefixError: "Error de dominio: ")
		.BindWithPrefixAsync(
			caseOk: async turnoModificado => await repositorio.UpdateTurnoWhereId(turnoOriginalId, turnoModificado),
			prefixError: "Error de escritura en Db: ")
		;
	}



	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.PersistirProgramarTurnoAsync(
		PacienteId pacienteId,
		DateTime fechaSolicitud,
		Disponibilidad2025 disponibilidad,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		return await Turno2025.Programar(pacienteId, fechaSolicitud, disponibilidad)
			.BindWithPrefixAsync(
				prefixError: "Error de Dominio: ",
				caseOk: async nuevoTurno => (await repositorio.InsertTurnoReturnId(nuevoTurno))
			.BindWithPrefix(
				prefixError: "Error de escritura en DB: ",
				caseOk: turnoId => new Result<Turno2025Agg>.Ok(Turno2025Agg.Crear(turnoId, nuevoTurno))
			)
		);
	}






}