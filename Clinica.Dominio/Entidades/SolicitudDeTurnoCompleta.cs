using System.Security.Cryptography.X509Certificates;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;


public readonly record struct TardeOMañana(bool Tarde) : IComoTexto {
	public string ATexto() => Tarde ? "Tarde" : "Mañana";
	public bool AplicaA(DateTime fecha) => Tarde ? EsTarde(fecha) : EsMañana(fecha);
	private static bool EsTarde(DateTime dt) => dt.Hour >= 13;
	private static bool EsMañana(DateTime dt) => dt.Hour < 13;
}

public readonly record struct SolicitudDeTurno(
	Paciente2025 Paciente,
	EspecialidadMedica2025 Especialidad,
	DateTime Fecha
) : IComoTexto {

	public static Result<SolicitudDeTurno> Crear(
		Paciente2025 paciente,
		EspecialidadMedica2025 especialidad,
		DateTime fechaSolicitada
	) {
		if (fechaSolicitada < DateTime.Now)
			return new Result<SolicitudDeTurno>.Error("La fecha solicitada no puede ser en el pasado.");


		Console.WriteLine($"\n {paciente.NombreCompleto} solicita {especialidad.Titulo}");
		return new Result<SolicitudDeTurno>.Ok(
			new SolicitudDeTurno(paciente, especialidad, fechaSolicitada)
		);
	}

	public string ATexto() =>
		$"Solicitud básica:\n" +
		$"  • Paciente: {Paciente.NombreCompleto.ATexto()}\n" +
		$"  • Especialidad: {Especialidad.ATexto()}\n" +
		$"  • Solicitado en: {Fecha:G}";
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
//	public DateTime Fecha => Basica.Fecha;

//	public string ATexto() {
//		return
//			"Solicitud de turno:\n" +
//			Basica.ATexto() + "\n" +
//			Preferencias.ATexto();
//	}
//}