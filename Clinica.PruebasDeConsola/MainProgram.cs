
using System.Collections.Generic;
using System.Collections.Immutable;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.PruebasDeConsola;

public static class MainProgram {
	static async Task Main() {
		Console.OutputEncoding = System.Text.Encoding.UTF8;


		ServicioTurnosManager TURNOS_MANAGER = ServicioTurnosManager.CrearServicio(
			(await AsyncRepositorioDapper.GetTurnos())
			,(await AsyncRepositorioDapper.GetMedicos())
			//,(await AsyncRepositorioDapper.GetPacientes()) // we don't really need pacientes loaded anymore :)
		).GetOrRaise();

		Result<IReadOnlyList<DisponibilidadEspecialidad2025>> disponibiliades = TURNOS_MANAGER.SolicitarDisponibilidadesPara(
			EspecialidadMedica2025.Psicologo
			,new DateTime(2025, 12, 20) //a partir de este dia
			,7 //howmanytoYield
		).PrintAndContinue("Solicitando disponibilidades para Psicologo.");


        Turno2025 turno1 = TURNOS_MANAGER.SolicitarTurnoEnLaPrimeraDisponibilidad(
			new PacienteId(1) //este paciente
			,EspecialidadMedica2025.ClinicoGeneral //pide esta especialidad
			,new DateTime(2025, 12, 25) //la solicitud ocurre este dia. generalmente se usaria Datetime.Now
		).PrintAndContinue("paciente1 pide turno.").GetOrRaise();


		Turno2025 turnoReprogramado = TURNOS_MANAGER.SolicitarReprogramacionALaPrimeraDisponibilidad(
			turno1 //este es el turno base
			,turno1.FechaHoraAsignadaHasta.AddDays(-2) //la reprogramacion ocurre justo dos dias antes de la cita.
			,"El paciente1 solicita reprogramacion y no dio explicaciones."
		).PrintAndContinue("paciente1 pide reprogramacion. Se lo damos?: ").GetOrRaise();


		Turno2025 turnoCancelado = TURNOS_MANAGER.SolicitarCancelacion(
			turnoReprogramado //este es el turno base
			, turnoReprogramado.FechaHoraAsignadaHasta.AddDays(-1) //la cancelacion ocurre justo 1 dias antes de la cita.
			, "El paciente1 solicita cancelacion porque si."
		).PrintAndContinue("paciente1 pide cancelacion. ").GetOrRaise();



	}

}
