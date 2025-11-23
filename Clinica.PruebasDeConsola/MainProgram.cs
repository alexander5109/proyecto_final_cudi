
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
			//,(await AsyncRepositorioDapper.GetPacientes()) // we don't really need pacientes loaded anymore :) //actually, we might need it if we want domain to assert the paciente ID exists in the database or has no turnos pendientes de la misma especialidad, etc.
		).GetOrRaise(); //los await devuelven Result<> y este servicio revisa que todos esten ok, sino salta acá.

		Result<IReadOnlyList<DisponibilidadEspecialidad2025>> disponibiliades = TURNOS_MANAGER.SolicitarDisponibilidadesPara(
			EspecialidadMedica2025.Psicologo
			,new DateTime(2025, 12, 20) //a partir de este dia
			,7 //howmanytoYield
		).PrintAndContinue("Solicitando disponibilidades para Psicologo."); //Ideal para GUI WPF


        Result<Turno2025> turno1Result = TURNOS_MANAGER.SolicitarTurnoEnLaPrimeraDisponibilidad(
			new PacienteId(1) //este paciente
			,EspecialidadMedica2025.ClinicoGeneral //pide esta especialidad
			,new DateTime(2025, 12, 25) //la solicitud ocurre este dia. generalmente se usaria Datetime.Now
		).PrintAndContinue("paciente1 pide turno.");


		Result<Turno2025> turnoReprogramadoResult = TURNOS_MANAGER.SolicitarReprogramacionALaPrimeraDisponibilidad(
			turno1Result //este es el turno base
			, new DateTime(2025, 12, 28) //el paciente llama al dia siguiente para cambiar la fecha
			, "El paciente1 solicita reprogramacion y no dio explicaciones."
		).PrintAndContinue("paciente1 pide reprogramacion. Se lo damos?: ");


		Result<Turno2025> turnoCanceladoResult = TURNOS_MANAGER.SolicitarCancelacion(
			turnoReprogramadoResult //este es el turno base
			, new DateTime(2025, 12, 30) //el paciente vuelve a llamar pero para cancelacion definitiva
			, "El paciente1 solicita cancelacion porque si."
		).PrintAndContinue("paciente1 pide cancelacion. ");


	}

}
