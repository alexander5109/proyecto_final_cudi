using System.Security.Cryptography.X509Certificates;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;

namespace Clinica.Dominio.TiposDeValor;

//THIS IS JUST FOR UI'S
//NOT NEEDED SAVING


public readonly record struct TardeOMañana(bool Tarde) : IComoTexto {
	public string ATexto() => Tarde ? "Tarde" : "Mañana";
	public bool AplicaA(DateTime fecha) => Tarde ? EsTarde(fecha) : EsMañana(fecha);
	private static bool EsTarde(DateTime dt) => dt.Hour >= 13;
	private static bool EsMañana(DateTime dt) => dt.Hour < 13;
}

public readonly record struct SolicitudDeTurno(
	PacienteId PacienteId,
	EspecialidadMedica2025 Especialidad,
	DateTime FechaCreacion
) : IComoTexto {

	public static Result<SolicitudDeTurno> Crear(
		PacienteId pacienteId,
		EspecialidadMedica2025 especialidad,
		DateTime fechaSolicitada
	) {
		return new Result<SolicitudDeTurno>.Ok(new SolicitudDeTurno(pacienteId, especialidad, fechaSolicitada));
	}

	public string ATexto() =>
		$"Solicitud básica:\n" +
		$"  • Paciente: {PacienteId}\n" +
		$"  • Especialidad: {Especialidad.ATexto()}\n" +
		$"  • Solicitado en: {FechaCreacion:G}";
}

public readonly record struct SolicitudDeTurnoPreferencias(
	DiaSemana2025? DiaPreferido,
	TardeOMañana? MomentoPreferido
) : IComoTexto {
	public static readonly SolicitudDeTurnoPreferencias Ninguna = new(null, null);

	public bool TienePreferencias => DiaPreferido is not null || MomentoPreferido is not null;

	public string ATexto() {
		if (!TienePreferencias)
			return "Sin preferencias";

		return
			"Preferencias de turno:\n" +
			(DiaPreferido is DiaSemana2025 d ? $"  • Día preferido: {d.ATexto()}\n" : "") +
			(MomentoPreferido is TardeOMañana m ? $"  • Prefiere: {m.ATexto()}\n" : "");
	}
}

//public readonly record struct SolicitudDeTurnoCompleta(
//	SolicitudDeTurno Basica,
//	SolicitudDeTurnoPreferencias Preferencias
//) : IComoTexto {
//	public Paciente2025 Paciente => Basica.Paciente;
//	public EspecialidadMedica2025 Especialidad => Basica.Especialidad;
//	public DateTime FechaCreacion => Basica.FechaCreacion;

//	public string ATexto() {
//		return
//			"Solicitud de turno:\n" +
//			Basica.ATexto() + "\n" +
//			Preferencias.ATexto();
//	}
//}