using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct Disponibilidad2025( //este tipo de valor es la mejor utilizacion de un struct in c#
	EspecialidadEnumCodigo EspecialidadCodigo,
	MedicoId MedicoId,
	DateTime FechaHoraDesde,
	DateTime FechaHoraHasta
);
 // : IComoTexto {
	// public string ATexto() {
        // string fecha = FechaHoraDesde.ToString("dddd dd/MM/yyyy");
        // string desde = FechaHoraDesde.ToString("HH:mm");
        // string hasta = FechaHoraHasta.ToString("HH:mm");
		// return
			// $"Disponibilidad de {EspecialidadEnumCodigo}\n" +
			// $"  • Médico: {MedicoId}\n" +
			//$"  • Médico: {Medico.NombreCompleto.ATextoDia()}\n" +
			// $"  • Fecha: {fecha}\n" +
			// $"  • Horario: {desde}–{hasta}";
	// }
// }