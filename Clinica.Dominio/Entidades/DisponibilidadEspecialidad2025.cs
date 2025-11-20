using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;

public readonly record struct DisponibilidadEspecialidad2025(
	EspecialidadMedica2025 Especialidad,
	Medico2025 Medico,
	DateTime FechaHoraDesde,
	DateTime FechaHoraHasta
) : IComoTexto {
	public string ATexto() {
		var fecha = FechaHoraDesde.ToString("dddd dd/MM/yyyy");
		var desde = FechaHoraDesde.ToString("HH:mm");
		var hasta = FechaHoraHasta.ToString("HH:mm");
		return
			$"Disponibilidad de {Especialidad.ATexto()}\n" +
			$"  • Médico: {Medico.NombreCompleto.ATexto()}\n" +
			$"  • Fecha: {fecha}\n" +
			$"  • Horario: {desde}–{hasta}";
	}
}
