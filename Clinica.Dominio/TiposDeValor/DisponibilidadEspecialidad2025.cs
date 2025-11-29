using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct DisponibilidadEspecialidad2025(
	EspecialidadMedica2025 Especialidad,
	MedicoId MedicoId,
	DateTime FechaHoraDesde,
	DateTime FechaHoraHasta
) : IComoTexto {
	public string ATexto() {
        string fecha = FechaHoraDesde.ToString("dddd dd/MM/yyyy");
        string desde = FechaHoraDesde.ToString("HH:mm");
        string hasta = FechaHoraHasta.ToString("HH:mm");
		return
			$"Disponibilidad de {Especialidad.ATexto()}\n" +
			$"  • Médico: {MedicoId}\n" +
			$"  • Fecha: {fecha}\n" +
			$"  • Horario: {desde}–{hasta}";
	}
}
