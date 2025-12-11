using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.Servicios;

public class ServiciosPublicos : IServiciosDeDominio {

	async Task<Result<IReadOnlyList<Disponibilidad2025>>> IServiciosDeDominio.SolicitarDisponibilidades(EspecialidadCodigo solicitudEspecialidadCodigo, DateTime aPartirDeCuando, int cuantos, IRepositorioDominioServices repositorio) {

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


















	async Task<Result<Turno2025>> IServiciosDeDominio.PersistirComoAusenteAsync(
		TurnoId turnoOriginalId,
		DateTime outcomeFecha,
		string? outcomeComentario,
		IRepositorioDominioServices repositorio
	) {
		return await  (await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId))
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