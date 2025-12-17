using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct Disponibilidad2025( //este tipo de valor es la mejor utilizacion de un struct in c#
	EspecialidadEnum EspecialidadCodigo,
	MedicoId2025 MedicoId,
	DateTime FechaHoraDesde,
	DateTime FechaHoraHasta
);
 // : IComoTexto {
	// public string ATexto() {
        // string fecha = FechaHoraDesde.ToString("dddd dd/MM/yyyy");
        // string desde = FechaHoraDesde.ToString("HH:mm");
        // string hasta = FechaHoraHasta.ToString("HH:mm");
		// return
			// $"Disponibilidad de {EspecialidadEnum}\n" +
			// $"  • Médico: {MedicoId2025}\n" +
			//$"  • Médico: {Medico.NombreCompleto.ATextoDia()}\n" +
			// $"  • Fecha: {fecha}\n" +
			// $"  • Horario: {desde}–{hasta}";
	// }
// }