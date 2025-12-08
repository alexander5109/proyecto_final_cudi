using System.Runtime.CompilerServices;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.TiposDeValor;

public record Disponibilidad2025(
	Especialidad2025 Especialidad,
	PacienteId Paciente,
	MedicoId MedicoId,
	DateTime FechaHoraDesde,
	DateTime FechaHoraHasta
	//DiaSemana2025 DiaSemana
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
//public static class DisponbiilidadConvertions {



	//public Especialidad2025 ToDomain(CallConvThiscall DayOfWeek)




//}