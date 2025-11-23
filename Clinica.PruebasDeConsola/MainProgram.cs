
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

		Result<IReadOnlyList<DisponibilidadEspecialidad2025>> disponibiliades = ServicioTurnosManager.SolicitarDisponibilidadesPara(
			EspecialidadMedica2025.Psicologo
			,new DateTime(2025, 12, 20) //a partir de este dia
			,7 //howmanytoYield
			, (await AsyncRepositorioDapper.GetMedicos())
			, (await AsyncRepositorioDapper.GetTurnos())
		).PrintAndContinue("Solicitando disponibilidades para Psicologo."); //Ideal para GUI WPF


        Result<Turno2025> turno1Result = ServicioTurnosManager.SolicitarTurnoEnLaPrimeraDisponibilidad(
			new PacienteId(1) //este paciente
			,EspecialidadMedica2025.ClinicoGeneral //pide esta especialidad
			,new DateTime(2025, 12, 25) //la solicitud ocurre este dia. generalmente se usaria Datetime.Now
			, (await AsyncRepositorioDapper.GetMedicos())
			, (await AsyncRepositorioDapper.GetTurnos())
		).PrintAndContinue("paciente1 pide turno.");


		Result<Turno2025> turnoReprogramadoResult = ServicioTurnosManager.SolicitarReprogramacionALaPrimeraDisponibilidad(
			turno1Result //este es el turno base
			, new DateTime(2025, 12, 28) //el paciente llama al dia siguiente para cambiar la fecha
			, "El paciente1 solicita reprogramacion y no dio explicaciones."
			, (await AsyncRepositorioDapper.GetMedicos())
			, (await AsyncRepositorioDapper.GetTurnos())
		).PrintAndContinue("paciente1 pide reprogramacion. Se lo damos?: ");


		Result<Turno2025> turnoCanceladoResult = ServicioTurnosManager.SolicitarCancelacion(
			turnoReprogramadoResult //este es el turno base
			, new DateTime(2025, 12, 30) //el paciente vuelve a llamar pero para cancelacion definitiva
			, "El paciente1 solicita cancelacion porque si."
			, (await AsyncRepositorioDapper.GetTurnos())
		).PrintAndContinue("paciente1 pide cancelacion. ");


	}

}
