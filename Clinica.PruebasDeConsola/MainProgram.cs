
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


		ServicioTurnosManager TURNOS_MANAGER = ServicioTurnosManager.Crear(
			(await AsyncRepositorioDapper.GetTurnos()),
			(await AsyncRepositorioDapper.GetMedicos()),
			(await AsyncRepositorioDapper.GetPacientes())
		).GetOrRaise();

        Result<Turno2025> turnoResult = TURNOS_MANAGER.SolicitarTurnoEnLaPrimeraDisponibilidad(
			new PacienteId(1), //este paciente
			EspecialidadMedica2025.ClinicoGeneral, //pide esta especialidad
			new DateTime(2025, 12, 25) //la solicitud ocurre este dia. generalmente se usaria Datetime.Now
		).PrintAndContinue("paciente1 pide turno.");
		Turno2025 turno = turnoResult.GetOrRaise();


		Result<Turno2025> turnoReprogramadoResult = TURNOS_MANAGER.SolicitarReprogramacionALaPrimeraDisponibilidad(
			turno, //este es el turno base
			turno.FechaHoraAsignadaHasta.AddDays(-2) //la solicitud ocurre dos dias antes de la cita.
		).PrintAndContinue("paciente1 pide turno. Se lo damos?: ");
		Turno2025 turnoReprogramado = turnoResult.GetOrRaise();




	}

}
