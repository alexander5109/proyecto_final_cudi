using System.Runtime.CompilerServices;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct Disponibilidad2025( //este tipo de valor es la mejor utilizacion de un struct in c#
	EspecialidadCodigo EspecialidadCodigo,
	MedicoId MedicoId,
	DateTime FechaHoraDesde,
	DateTime FechaHoraHasta
) : IComoTexto {
	public string ATexto() {
        string fecha = FechaHoraDesde.ToString("dddd dd/MM/yyyy");
        string desde = FechaHoraDesde.ToString("HH:mm");
        string hasta = FechaHoraHasta.ToString("HH:mm");
		return
			$"Disponibilidad de {EspecialidadCodigo}\n" +
			$"  • Médico: {MedicoId}\n" +
			//$"  • Médico: {Medico.NombreCompleto.ATexto()}\n" +
			$"  • Fecha: {fecha}\n" +
			$"  • Horario: {desde}–{hasta}";
	}
}
//public static class DisponbiilidadConvertions {



	//public Especialidad2025 ToDomain(CallConvThiscall DayOfWeek)




//}