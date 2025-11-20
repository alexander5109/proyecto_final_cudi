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
	DiaSemana2025 DiaPreferido,
	TardeOMañana PrefiereTardeOMañana,
	DateTime SolicitudEn
) : IComoTexto {
	public string ATexto() {
		return
			$"Solicitud de turno:\n" +
			$"  • Paciente: {Paciente.NombreCompleto.ATexto()}\n" +
			$"  • Especialidad: {Especialidad.ATexto()}\n" +
			$"  • Día preferido: {DiaPreferido.ATexto()}\n" +
			$"  • Prefiere: {PrefiereTardeOMañana.ATexto()}\n" +
			$"  • Solicitado en: {SolicitudEn:G}";
	}
}
