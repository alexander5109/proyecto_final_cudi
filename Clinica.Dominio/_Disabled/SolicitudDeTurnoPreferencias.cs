using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposExtensiones;

namespace Clinica.Dominio._Disabled;

public readonly record struct TardeOMañana(bool Tarde) : IComoTexto {
	public string ATexto() => Tarde ? "Tarde" : "Mañana";
	public bool AplicaA(DateTime fecha) => Tarde ? EsTarde(fecha) : EsMañana(fecha);
	private static bool EsTarde(DateTime dt) => dt.Hour >= 13;
	private static bool EsMañana(DateTime dt) => dt.Hour < 13;
}

public readonly record struct SolicitudDeTurnoPreferencias(
	DayOfWeek? DiaPreferido,
	TardeOMañana? MomentoPreferido
) : IComoTexto {
	public static readonly SolicitudDeTurnoPreferencias Ninguna = new(null, null);

	public bool TienePreferencias => DiaPreferido is not null || MomentoPreferido is not null;

	public string ATexto() {
		if (!TienePreferencias)
			return "Sin preferencias";

		return
			"Preferencias de turno:\n" +
			(DiaPreferido is DayOfWeek d ? $"  • Día preferido: {d.ATexto()}\n" : "") +
			(MomentoPreferido is TardeOMañana m ? $"  • Prefiere: {m.ATexto()}\n" : "");
	}
}

//public readonly record struct SolicitudDeTurnoCompleta(
//	SolicitudDeTurno Basica,
//	SolicitudDeTurnoPreferencias Preferencias
//) : IComoTexto {
//	public Paciente2025 Nivel4Medico => Basica.Nivel4Medico;
//	public Especialidad2025 Especialidad => Basica.Especialidad;
//	public DateTime FechaCreacion => Basica.FechaCreacion;

//	public string ATextoDia() {
//		return
//			"Solicitud de turno:\n" +
//			Basica.ATextoDia() + "\n" +
//			Preferencias.ATextoDia();
//	}
//}
//THIS IS JUST FOR UI'S
//NOT NEEDED SAVING



//public readonly record struct SolicitudDeTurno(
//	PacienteId PacienteId,
//	Especialidad2025 Especialidad,
//	DateTime FechaCreacion
//) : IComoTexto {

//	public static Result<SolicitudDeTurno> CrearFromStrings(
//		PacienteId pacienteId,
//		Especialidad2025 especialidad,
//		DateTime fechaSolicitada
//	) {
//		return new Result<SolicitudDeTurno>.Ok(new SolicitudDeTurno(pacienteId, especialidad, fechaSolicitada));
//	}

//	public string ATextoDia() =>
//		$"Solicitud básica:\n" +
//		$"  • Nivel4Medico: {PacienteId}\n" +
//		$"  • Especialidad: {Especialidad.ATextoDia()}\n" +
//		$"  • Solicitado en: {FechaCreacion:G}";
//}
