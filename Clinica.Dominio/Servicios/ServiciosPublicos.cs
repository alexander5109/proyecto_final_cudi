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
		return (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			doOnSuccess: aggOriginal => aggOriginal.Turno.MarcarComoAusente(outcomeFecha, outcomeComentario)
			.BindWithPrefix(
				doOnSuccess: turnoAusente => new Result<Turno2025Agg>.Ok(Turno2025Agg.Crear(turnoOriginalId, turnoAusente)),
				prefixError: "Error de dominio: "),
			prefixError: "Error de db: "
		);
	}


	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.PersistirComoConcretadoAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string? outcomeComentario,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		return (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			doOnSuccess: aggOriginal => aggOriginal.Turno.MarcarComoConcretado(outcomeFecha, outcomeComentario)
			.BindWithPrefix(
				doOnSuccess: turnoAusente => new Result<Turno2025Agg>.Ok(Turno2025Agg.Crear(turnoOriginalId, turnoAusente)),
				prefixError: "Error de Dominio: "),
			prefixError: "Error de db: "
		);
	}


	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.PersistirComoCanceladoAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string outcomeComentario,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		return (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			doOnSuccess: aggOriginal => aggOriginal.Turno.MarcarComoCancelado(outcomeFecha, outcomeComentario)
			.BindWithPrefix(
				doOnSuccess: turnoCancelado => new Result<Turno2025Agg>.Ok(Turno2025Agg.Crear(turnoOriginalId, turnoCancelado)),
				prefixError: "Error de dominio: "),
			prefixError: "Error de db: "
		);
	}


	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.PersistirProgramarTurnoAsync(
		PacienteId pacienteId,
		DateTime fechaSolicitud,
		Disponibilidad2025 disponibilidad,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		return await Turno2025.Programar(pacienteId, FechaRegistro2025.Representar(fechaSolicitud), disponibilidad)
		.BindWithPrefixAsync(
			doOnSuccess: async nuevoTurno => (await repositorio.InsertTurnoReturnId(nuevoTurno))
			.BindWithPrefix(
				doOnSuccess: turnoId => new Result<Turno2025Agg>.Ok(Turno2025Agg.Crear(turnoId, nuevoTurno)),
				prefixError: "Error de DB: "),
			prefixError: "Error de Dominio: "
		);
	}




	async Task<Result<Turno2025Agg>> PersistirComoReprogramadoYPersistirProgramarTurnoAsync(
		TurnoId turnoOriginalId,
		PacienteId pacienteId,
		DateTime outcomeFecha,
		string Comentario,
		Disponibilidad2025 disponibilidad,
		IRepositorioDomainServiciosPrivados repositorio
	) {

		throw new NotImplementedException();
	}

	async Task<Result<Turno2025Agg>> IServiciosGestionTurnos.PersistirComoReprogramadoYPersistirProgramarTurnoAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string outcomeComentario,
		IRepositorioDomainServiciosPrivados repositorio
	) {
		throw new NotImplementedException();
	}
}