
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.PruebasDeConsola;

public static class MainProgram {
	static async Task Main() {
		Console.OutputEncoding = System.Text.Encoding.UTF8;


		List<Result<Paciente2025>> PACIENTES = await AsyncRepositorioDapper.GetPacientes();
		//List<Result<Paciente2025>> PACIENTES = await AsyncRepositorioHardCoded.GetPacientes();
		List<Medico2025> MEDICOS = await AsyncRepositorioHardCoded.GetMedicos();
		ListaTurnosHistorial2025 TURNOS = await AsyncRepositorioHardCoded.GetTurnos();

		Result<Paciente2025> paciente1 = PACIENTES[0]
			.PrintAndContinue("Tratando de seleccionar paciente1")
		;
		Result<SolicitudDeTurno> solicitudPaciente1 = SolicitudDeTurno.Crear(
				paciente1,
				EspecialidadMedica2025.ClinicoGeneral,
				DateTime.Now
			)
			.PrintAndContinue("paciente1 intenta solicitar turno: ")
		;

		Result<ListaDisponibilidades2025> resultDisponibilidadesPaciente1 =
			ListaDisponibilidades2025.Buscar(solicitudPaciente1, MEDICOS, TURNOS, 3)
			.PrintAndContinue("Buscando disponibilidades: ")
			.AplicarFiltrosOpcionales(new(
				DiaSemana2025.Lunes,
				new TardeOMañana(false)
			))
			.PrintAndContinue("AplicarFiltrosOpcionales: ")
		;

		Result<DisponibilidadEspecialidad2025> primeraDispPaciente1 = resultDisponibilidadesPaciente1
			.TomarPrimera()
			.PrintAndContinue("Tomando la primera: ")
		;

		TURNOS.AgendarTurno(Turno2025
			.Programar(solicitudPaciente1, primeraDispPaciente1))
			.PrintAndContinue("Agendando turno: ")
		;


	}

}
