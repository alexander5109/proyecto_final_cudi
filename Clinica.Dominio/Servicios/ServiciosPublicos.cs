using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.IInterfaces.QueryModels;


namespace Clinica.Dominio.Servicios;

public class ServiciosPublicos : IServiciosDeDominio {

	async Task<Result<IReadOnlyList<Disponibilidad2025>>>IServiciosDeDominio.SolicitarDisponibilidades(
		EspecialidadEnumCodigo especialidadCodigo,
		DateTime aPartirDeCuando,
		int cuantos,
		DayOfWeek? diaSemanaPreferido,
		IRepositorioDominioServices repo
	) {

		Console.WriteLine(especialidadCodigo.ToString());

		if (cuantos <= 0)
			return new Result<IReadOnlyList<Disponibilidad2025>>
				.Error("La cantidad solicitada debe ser mayor a cero.");

		if (cuantos > 80)
			return new Result<IReadOnlyList<Disponibilidad2025>>
				.Error("No vamos a producir tantas disponibilidades.");

		var espResult = Especialidad2025.CrearResult(especialidadCodigo);
		if (espResult.IsError)
			return new Result<IReadOnlyList<Disponibilidad2025>>.Error(espResult.UnwrapAsError());

		Especialidad2025 especialidad = espResult.UnwrapAsOk();

		DateTime hastaBusqueda = aPartirDeCuando.Date.AddDays(7 * 30);

		// 1️⃣ Médicos de la especialidad
		var medicosResult =
			await repo.SelectMedicosIdWhereEspecialidadCodigo(especialidadCodigo);

		if (medicosResult.IsError)
			return new Result<IReadOnlyList<Disponibilidad2025>>
				.Error(medicosResult.UnwrapAsError());

		var disponibilidades = new List<Disponibilidad2025>();

		foreach (MedicoId medicoId in medicosResult.UnwrapAsOk()) {

			// 2️⃣ Turnos existentes
			var turnosResult =
				await repo.SelectTurnosProgramadosBetweenFechasWhereMedicoId(
					medicoId, aPartirDeCuando, hastaBusqueda);

			if (turnosResult.IsError)
				return new Result<IReadOnlyList<Disponibilidad2025>>
					.Error(turnosResult.UnwrapAsError());

			var turnos = turnosResult.UnwrapAsOk().ToList();

			// 3️⃣ Horarios vigentes
			var horariosResult =
				await repo.SelectHorariosVigentesBetweenFechasWhereMedicoId(
					medicoId, aPartirDeCuando, hastaBusqueda);

			if (horariosResult.IsError)
				return new Result<IReadOnlyList<Disponibilidad2025>>
					.Error(horariosResult.UnwrapAsError());

			foreach (HorarioMedicoQM franja in horariosResult.UnwrapAsOk()) {

				if (diaSemanaPreferido is not null &&
					franja.DiaSemana != diaSemanaPreferido)
					continue;

				// buscar primer día coincidente
				DateTime fecha = aPartirDeCuando.Date;

				int diasASumar =
					((int)franja.DiaSemana - (int)fecha.DayOfWeek + 7) % 7;

				fecha = fecha.AddDays(diasASumar);

				for (int semana = 0; semana < 30; semana++, fecha = fecha.AddDays(7)) {

					DateTime desde = fecha + franja.HoraDesde;
					DateTime hasta = fecha + franja.HoraHasta;

					if (hasta <= aPartirDeCuando)
						continue;

					for (DateTime slot = desde;
						 slot.AddMinutes(especialidad.DuracionConsultaMinutos) <= hasta;
						 slot = slot.AddMinutes(especialidad.DuracionConsultaMinutos)) {

						if (slot < aPartirDeCuando)
							continue;

						DateTime slotHasta =
							slot.AddMinutes(especialidad.DuracionConsultaMinutos);

						bool solapa = turnos.Any(t =>
							t.EspecialidadCodigo == especialidad.Codigo &&
							t.OutcomeEstado == TurnoEstadoCodigo.Programado &&
							t.FechaHoraAsignadaDesde < slotHasta &&
							slot < t.FechaHoraAsignadaHasta
						);

						if (!solapa) {
							disponibilidades.Add(new Disponibilidad2025(
								especialidad.Codigo,
								medicoId,
								slot,
								slotHasta
							));
						}
					}
				}
			}
		}

		var resultado = disponibilidades
			.OrderBy(d => d.FechaHoraDesde)
			.Take(cuantos)
			.ToList();

		return resultado.Count > 0
			? new Result<IReadOnlyList<Disponibilidad2025>>.Ok(resultado)
			: new Result<IReadOnlyList<Disponibilidad2025>>.Error("No se encontraron disponibilidades.");
	}


















	async Task<Result<Turno2025>> IServiciosDeDominio.PersistirComoAusenteAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string? outcomeComentario,
		IRepositorioDominioServices repositorio
	) {
		return await (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			caseOk: turnoOriginal => turnoOriginal.MarcarComoAusente(outcomeFecha, outcomeComentario),
			prefixError: "Error de dominio: ")
		.BindWithPrefixAsync(
			caseOk: async turnoModificado => await repositorio.UpdateTurnoWhereIdAndReturnAsDomain(turnoOriginalId, turnoModificado),
			prefixError: "Error de escritura en Db: ")
		;
	}


	async Task<Result<Turno2025>> IServiciosDeDominio.PersistirComoConcretadoAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string? outcomeComentario,
		IRepositorioDominioServices repositorio
	) {
		return await (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			caseOk: turnoOriginal => turnoOriginal.MarcarComoConcretado(outcomeFecha, outcomeComentario),
			prefixError: "Error de dominio: ")
		.BindWithPrefixAsync(
			caseOk: async turnoModificado => await repositorio.UpdateTurnoWhereIdAndReturnAsDomain(turnoOriginalId, turnoModificado),
			prefixError: "Error de escritura en Db: ")
		;
	}



	async Task<Result<Turno2025>> IServiciosDeDominio.PersistirComoCanceladoAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string? outcomeComentario,
		IRepositorioDominioServices repositorio
	) {
		return await (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			caseOk: turnoOriginal => turnoOriginal.MarcarComoCancelado(outcomeFecha, outcomeComentario),
			prefixError: "Error de dominio: ")
		.BindWithPrefixAsync(
			caseOk: async turnoModificado => await repositorio.UpdateTurnoWhereIdAndReturnAsDomain(turnoOriginalId, turnoModificado),
			prefixError: "Error de escritura en Db: ")
		;
	}



	async Task<Result<Turno2025>> IServiciosDeDominio.PersistirComoReprogramado(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string? outcomeComentario,
		IRepositorioDominioServices repositorio
	) {
		return await (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
		.BindWithPrefix(
			caseOk: turnoOriginal => turnoOriginal.MarcarComoReprogramado(outcomeFecha, outcomeComentario),
			prefixError: "Error de dominio: ")
		.BindWithPrefixAsync(
			caseOk: async turnoModificado => await repositorio.UpdateTurnoWhereIdAndReturnAsDomain(turnoOriginalId, turnoModificado),
			prefixError: "Error de escritura en Db: ")
		;
	}



	async Task<Result<Turno2025Agg>> IServiciosDeDominio.PersistirProgramarTurnoAsync(
		PacienteId pacienteId,
		DateTime fechaSolicitud,
		Disponibilidad2025 disponibilidad,
		IRepositorioDominioServices repositorio
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