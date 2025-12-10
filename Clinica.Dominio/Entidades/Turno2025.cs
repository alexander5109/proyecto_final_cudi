using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;


public record struct TurnoId(int Valor) {
	public static Result<TurnoId> CrearResult(TurnoId? id) =>
		(id is TurnoId idGood && idGood.Valor >= 0)
		? new Result<TurnoId>.Ok(idGood)
		: new Result<TurnoId>.Error("El id no puede ser nulo o negativo.");
	public static TurnoId Crear(int id) => new(id);
	public static Result<TurnoId> CrearResult(int? id) =>
		id is int idGood
		? new Result<TurnoId>.Ok(new TurnoId(idGood))
		: new Result<TurnoId>.Error("El id no puede ser nulo.");
	public static bool TryParse(string? s, out TurnoId id) {
		if (int.TryParse(s, out int value)) {
			id = new TurnoId(value);
			return true;
		}
		id = default;
		return false;
	}

	public override string ToString() {
		return Valor.ToString();
	}
}
public record Turno2025Agg(TurnoId Id, Turno2025 Turno) {
	public static Turno2025Agg Crear(TurnoId id, Turno2025 turno) => new(id, turno);
	public static Result<Turno2025Agg> CrearResult(Result<TurnoId> idResult,Result<Turno2025> instanceResult)
		=> from id in idResult
		   from instance in instanceResult
		   select new Turno2025Agg(
			   id,
			   instance
		   );

}
public record Turno2025(
	DateTime FechaDeCreacion,
	PacienteId PacienteId,
	MedicoId MedicoId,
	Especialidad2025 Especialidad,
	DateTime FechaHoraAsignadaDesdeValor,
	DateTime FechaHoraAsignadaHastaValor,
	TurnoEstadoCodigo OutcomeEstado,
	Option<DateTime> OutcomeFechaOption,
	Option<string> OutcomeComentarioOption
) : IComoTexto {

	public string ATexto() {
		string fecha = FechaHoraAsignadaDesdeValor.ToString("dddd dd/MM/yyyy");
		string desde = FechaHoraAsignadaDesdeValor.ToString("HH:mm");
		string hasta = FechaHoraAsignadaHastaValor.ToString("HH:mm");
		double duracion = (FechaHoraAsignadaHastaValor - FechaHoraAsignadaDesdeValor).TotalMinutes;
		return
			$"Turno de {Especialidad.ATexto()}\n" +
			$"  • PacienteId: {PacienteId}\n" +
			$"  • Médico asignado: {MedicoId}\n" +
			$"  • Fecha: {fecha}\n" +
			$"  • Horario: {desde}–{hasta} ({duracion} min)\n" +
			$"  • OutcomeEstado: {OutcomeEstado}\n";
	}

	public static Turno2025 Representar(
		DateTime fechaCreacion,
		PacienteId pacienteId,
		MedicoId medicoId,
		EspecialidadCodigo especialidadCododigo,
		DateTime desde,
		DateTime hasta,
		TurnoEstadoCodigo outcomeEstado,
		DateTime? outcomeFecha,
		string? outcomeComentario
	) {
		return new(
			FechaDeCreacion: fechaCreacion,
			PacienteId: pacienteId,
			MedicoId: medicoId,
			Especialidad: Especialidad2025.Representar(especialidadCododigo),
			FechaHoraAsignadaDesdeValor: desde,
			FechaHoraAsignadaHastaValor: hasta,
			OutcomeEstado: outcomeEstado,
			OutcomeFechaOption: outcomeFecha is DateTime fechagud ? Option<DateTime>.Some(fechagud) : Option<DateTime>.None,
			OutcomeComentarioOption: outcomeComentario is null ? Option<string>.None : Option<string>.Some(outcomeComentario)
		);
	}


	public static Result<Turno2025> CrearResult(
		//Result<TurnoId> idResult,
		DateTime fechaCreacion,
		Result<PacienteId> pacienteIdResult,
		Result<MedicoId> medicoIdResult,
		Result<Especialidad2025> especialidadResult,
		DateTime desde,
		DateTime hasta,
		Result<TurnoEstadoCodigo> outcomeEstadoResult,
		Option<DateTime> outcomeFecha,
		Option<string> outcomeComentario
	) {
		return
			//from id in idResult
			from pacienteId in pacienteIdResult
			from medicoId in medicoIdResult
			from especialidad in especialidadResult
			from estado in outcomeEstadoResult
			select new Turno2025(
				//id,
				fechaCreacion,
				pacienteId,
				medicoId,
				especialidad,
				desde,
				hasta,
				estado,
				outcomeFecha,
				outcomeComentario
			);
	}



	public static Result<Turno2025> Programar(
		PacienteId pacienteId,
		DateTime solicitadoEn,
		Disponibilidad2025 disp
	) {
		if (disp.FechaHoraDesde >= disp.FechaHoraHasta)
			return new Result<Turno2025>.Error(
				"La hora de inicio debe ser anterior a la hora de fin."
			);

		//we cant check this unless disponibildiad richness includes a whole doctor. Let's trust the disponibilidad maker
		//if (disp.Medico.EspecialidadUnica != disp.Especialidad)
		//	return new Result<Turno2025>.Error($"El medico {disp.Medico.NombreCompleto.ATextoDia()} tentativo no es especialidad en {disp.Especialidad.ATextoDia()}");

		if (disp.FechaHoraDesde.Date != disp.FechaHoraHasta.Date)
			return new Result<Turno2025>.Error(
				"Un turno no puede extenderse entre dos días distintos."
			);

		return new Result<Turno2025>.Ok(new Turno2025(
			//Id: turnoId,
			FechaDeCreacion: solicitadoEn,
			PacienteId: pacienteId,
			MedicoId: disp.MedicoId,
			Especialidad: Especialidad2025.Representar(disp.EspecialidadCodigo),
			FechaHoraAsignadaDesdeValor: disp.FechaHoraDesde,
			FechaHoraAsignadaHastaValor: disp.FechaHoraHasta,
			OutcomeEstado: TurnoEstadoCodigo.Programado,
			OutcomeFechaOption: Option<DateTime>.None,
			OutcomeComentarioOption: Option<string>.None
		));
	}





}






public static class TurnoExtentions {

	public static Result<Turno2025> MarcarComoAusente(
		this Turno2025 turnoOriginal,
		DateTime fechaEvento,
		string comentario
	) {
		//if (comentario is null)
		//	return new Result<Turno2025>.Error("El comentario es obligatorio al marcar como ausente un turno.");

		if (turnoOriginal.OutcomeEstado == TurnoEstadoCodigo.Programado&& !turnoOriginal.OutcomeFechaOption.HasValor)
			return new Result<Turno2025>.Error("Solo puede marcarse como ausente un turno que esté programado.");

		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				OutcomeEstado = TurnoEstadoCodigo.Ausente,
				OutcomeFechaOption = Option<DateTime>.Some(fechaEvento),
				OutcomeComentarioOption = Option<string>.Some(comentario)
			}
		);
	}

	public static Result<Turno2025> MarcarComoConcretado(
		this Turno2025 turnoOriginal,
		DateTime fechaEvento,
		string? comentario
	) {
		//comentario totalmente opcional
		//if (comentario is null)
		//	return new Result<Turno2025>.Error("El comentario es obligatorio al concretar un turno.");

		if (turnoOriginal.OutcomeEstado == TurnoEstadoCodigo.Programado&& !turnoOriginal.OutcomeFechaOption.HasValor)
			return new Result<Turno2025>.Error("Solo puede concretarse un turno que esté programado.");

		if (fechaEvento.DayOfWeek < turnoOriginal.FechaHoraAsignadaDesdeValor.DayOfWeek)
			return new Result<Turno2025>.Error("No puede confirmarse un turno uno o mas dias antes de la fecha de la cita");

		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				OutcomeEstado = TurnoEstadoCodigo.Concretado,
				OutcomeFechaOption = Option<DateTime>.Some(fechaEvento),
				OutcomeComentarioOption = comentario is null ? Option<string>.None : Option<string>.Some(comentario)
			}
		);
	}
	public static Result<Turno2025> MarcarComoCancelado(
		this Turno2025 turnoOriginal,
		DateTime fechaEvento,
		string comentario
	) {
		//if (comentario is null) 
		//	return new Result<Turno2025>.Error("El comentario es obligatorio al cancelar un turno.");
		
		if (turnoOriginal.OutcomeEstado != TurnoEstadoCodigo.Programado || turnoOriginal.OutcomeFechaOption.HasValor) 
			return new Result<Turno2025>.Error("Solo puede cancelarse un turno que todavía esté programado.");
		
		if (fechaEvento < turnoOriginal.FechaDeCreacion) 
			return new Result<Turno2025>.Error("La fecha del evento no puede ser anterior a la fecha de creación del turno.");
		
		if (fechaEvento >= turnoOriginal.FechaHoraAsignadaHastaValor) 
			return new Result<Turno2025>.Error("No puede cancelarse un turno después de la cita.");
		
		if (fechaEvento.Date == turnoOriginal.FechaHoraAsignadaDesdeValor.Date)
			return new Result<Turno2025>.Error("No puede cancelarse el mismo dia de la cita. Por favor marcar como ausente por no avisar antes.");

		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				OutcomeEstado = TurnoEstadoCodigo.Cancelado,
				OutcomeFechaOption = Option<DateTime>.Some(fechaEvento),
				OutcomeComentarioOption = Option<string>.Some(comentario)
			}
		);
	}

	public static Result<Turno2025> MarcarComoReprogramado(
		this Turno2025 turnoOriginal,
		DateTime fechaEvento,
		string comentario
	) {
		//if (comentario is null)
		//	return new Result<Turno2025>.Error("El comentario es obligatorio al reporogranar un turno.");

		if (
			turnoOriginal.OutcomeEstado == TurnoEstadoCodigo.Programado
			&& !turnoOriginal.OutcomeFechaOption.HasValor
		)
			return new Result<Turno2025>.Error("Solo puede reprogramarse un turno que todavía esté programado.");

		if (fechaEvento > turnoOriginal.FechaHoraAsignadaHastaValor)
			return new Result<Turno2025>.Error("No puede reprogramarse un turno despues de la cita");

		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				OutcomeEstado = TurnoEstadoCodigo.Reprogramado,
				OutcomeFechaOption = Option<DateTime>.Some(fechaEvento),
				OutcomeComentarioOption = Option<string>.Some(comentario)
			}
		);
	}





}