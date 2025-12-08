using System.Runtime.CompilerServices;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;


public record struct TurnoId(int Valor) {
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
}
public record Turno2025(
	//TurnoId Id,
	FechaRegistro2025 FechaDeCreacion,
	PacienteId PacienteId,
	MedicoId MedicoId,
	Especialidad2025 Especialidad,
	DateTime FechaHoraAsignadaDesdeValor,
	DateTime FechaHoraAsignadaHastaValor,
	TurnoOutcomeEstado2025 OutcomeEstado,
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
			$"  • OutcomeEstado: {OutcomeEstado.Nombre}\n";
	}
	public static Result<Turno2025> CrearResult(
		//Result<TurnoId> idResult,
		Result<FechaRegistro2025> fechaCreacionResult,
		Result<PacienteId> pacienteIdResult,
		Result<MedicoId> medicoIdResult,
		Result<Especialidad2025> especialidadResult,
		DateTime desde,
		DateTime hasta,
		Result<TurnoOutcomeEstado2025> outcomeEstadoResult,
		Option<DateTime> outcomeFecha,
		Option<string> outcomeComentario
	) {
		return
			//from id in idResult
			from fechaCreacion in fechaCreacionResult
			from pacienteId in pacienteIdResult
			from medicoId in medicoIdResult
			from especialidad in especialidadResult
			from estado in outcomeEstadoResult
			from _ in ValidarOutcome(estado, outcomeFecha, outcomeComentario)
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




	private static Result<Unit> ValidarOutcome(
		//TurnoId id,
		TurnoOutcomeEstado2025 estado,
		Option<DateTime> fecha,
		Option<string> comentario
	) {
		bool programado = estado == TurnoOutcomeEstado2025.Programado;
		bool tieneFecha = fecha.HasValor;
		bool tieneComentario = comentario.HasValor;

		// ------------------------
		// CASO 1 — Programado
		// ------------------------
		if (programado) {
			if (tieneFecha || tieneComentario) {
				return new Result<Unit>.Error(
					$"Turno está Programado y no debe tener Fecha ni Comentario. " +
					$"Recibido: Fecha={(tieneFecha ? fecha.Valor.ToString() : "—")}, " +
					$"Comentario={(tieneComentario ? comentario.Valor : "—")}."
				);
			}

			return new Result<Unit>.Ok(Unit.Valor);
		}

		// ------------------------
		// CASO 2 — NO programado
		// ------------------------
		// Estados 2, 3, 4, 5: requieren AMBOS fecha y comentario
		if (!tieneFecha || !tieneComentario) {
			string faltantes =
				(!tieneFecha && !tieneComentario) ? "Fecha y Comentario" :
				(!tieneFecha) ? "Fecha" :
				"Comentario";

			return new Result<Unit>.Error(
				$"Turno en estado '{estado}' debe tener {faltantes}. " +
				$"Recibido: Fecha={(tieneFecha ? fecha.Valor.ToString() : "—")}, " +
				$"Comentario={(tieneComentario ? comentario.Valor : "—")}."
			);
		}

		return new Result<Unit>.Ok(Unit.Valor);
	}






	public static Result<Turno2025> ProgramarNuevo(
		//this Turno2025 turnoOriginal,
		//TurnoId turnoId,
		PacienteId pacienteId,
		FechaRegistro2025 solicitadoEn,
		Disponibilidad2025 disp
	) {
		if (disp.FechaHoraDesde >= disp.FechaHoraHasta)
			return new Result<Turno2025>.Error(
				"La hora de inicio debe ser anterior a la hora de fin."
			);

		if (disp.FechaHoraDesde.Date != disp.FechaHoraHasta.Date)
			return new Result<Turno2025>.Error(
				"Un turno no puede extenderse entre dos días distintos."
			);

		return new Result<Turno2025>.Ok(new Turno2025(
			//Id: turnoId,
			FechaDeCreacion: solicitadoEn,
			PacienteId: pacienteId,
			MedicoId: disp.MedicoId,
			Especialidad: disp.Especialidad,
			FechaHoraAsignadaDesdeValor: disp.FechaHoraDesde,
			FechaHoraAsignadaHastaValor: disp.FechaHoraHasta,
			OutcomeEstado: TurnoOutcomeEstado2025.Programado,
			OutcomeFechaOption: Option<DateTime>.None,
			OutcomeComentarioOption: Option<string>.None
		));
	}


}






public static class TurnoExtentions {







	public static Result<Turno2025> SetOutcome(
		this Turno2025 turnoOriginal,
		TurnoOutcomeEstado2025 outcomeEstado,
		DateTime outcomeFecha,
		string outcomeComentario
	) {
		if (turnoOriginal.OutcomeEstado != TurnoOutcomeEstado2025.Programado)
			return new Result<Turno2025>.Error("El turno ya tiene un estado final. No puede modificarse.");

		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				OutcomeEstado = outcomeEstado,
				OutcomeComentarioOption = Option<string>.Some(outcomeComentario),
				OutcomeFechaOption = Option<DateTime>.Some(outcomeFecha)
			}
		);
	}

	public static Result<Turno2025> Reprogramar(
		this Turno2025 turnoOriginal,
		Disponibilidad2025 nuevaDisp
	) {
		if (turnoOriginal.OutcomeEstado == TurnoOutcomeEstado2025.Programado)
			return new Result<Turno2025>.Error("No puede reprogramarse un turno que todavía está programado.");

		if (!turnoOriginal.OutcomeFechaOption.HasValor)
			return new Result<Turno2025>.Error("No se puede reprogramar un turno sin fecha de finalización del estado anterior.");

		if (nuevaDisp.FechaHoraDesde >= nuevaDisp.FechaHoraHasta)
			return new Result<Turno2025>.Error("La disponibilidad nueva es inválida: fechaDesde >= fechaHasta.");

		return new Result<Turno2025>.Ok(
			turnoOriginal with {
				FechaDeCreacion = new FechaRegistro2025(turnoOriginal.OutcomeFechaOption.Valor),
				FechaHoraAsignadaDesdeValor = nuevaDisp.FechaHoraDesde,
				FechaHoraAsignadaHastaValor = nuevaDisp.FechaHoraHasta,
				OutcomeEstado = TurnoOutcomeEstado2025.Programado,
				OutcomeFechaOption = Option<DateTime>.None,
				OutcomeComentarioOption = Option<string>.None
			}
		);
	}
	//public static Result<Turno2025Agg> Reprogramar(
	//	this Turno2025 turnoOriginal,
	//	Disponibilidad2025 nuevaDisp,
	//	TurnoId nuevoId
	//) {
	//	if (turnoOriginal.Turno.OutcomeEstado == TurnoOutcomeEstado2025.Programado)
	//		return new Result<Turno2025Agg>.Error("No puede reprogramarse un turno que todavía está programado.");

	//	if (!turnoOriginal.Turno.OutcomeFechaOption.HasValor)
	//		return new Result<Turno2025Agg>.Error("No se puede reprogramar un turno sin fecha de finalización del estado anterior.");

	//	if (nuevaDisp.FechaHoraDesde >= nuevaDisp.FechaHoraHasta)
	//		return new Result<Turno2025Agg>.Error("La disponibilidad nueva es inválida: fechaDesde >= fechaHasta.");

	//	return new Result<Turno2025Agg>.Ok(new Turno2025Agg(
	//		Id: nuevoId,
	//		Turno: new Turno2025(
	//			FechaDeCreacion: new FechaRegistro2025(turnoOriginal.Turno.OutcomeFechaOption.Valor),
	//			PacienteId: turnoOriginal.Turno.PacienteId,
	//			MedicoId: turnoOriginal.Turno.MedicoId,
	//			Especialidad: turnoOriginal.Turno.Especialidad,
	//			FechaHoraAsignadaDesdeValor: nuevaDisp.FechaHoraDesde,
	//			FechaHoraAsignadaHastaValor: nuevaDisp.FechaHoraHasta,
	//			OutcomeEstado: TurnoOutcomeEstado2025.Programado,
	//			OutcomeFechaOption: Option<DateTime>.None,
	//			OutcomeComentarioOption: Option<string>.None
	//	)));
	//}






}