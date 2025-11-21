
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


        //IReadOnlyList<Paciente2025> PACIENTES = (await AsyncRepositorioHardCoded.GetPacientes())
        List<Result<Paciente2025>> PACIENTES_RESULT = await AsyncRepositorioDapper.GetPacientes();
		IReadOnlyList<Paciente2025> PACIENTES = PACIENTES_RESULT
			.Select(r => r.PrintAndContinue("Paciente domainizado")
						  .GetOrRaise())
			.ToList();

		IReadOnlyList<Medico2025> MEDICOS = (await AsyncRepositorioDapper.GetMedicos())
			.Select(r => r.PrintAndContinue("Medico domainizado")
						  .GetOrRaise())
			.ToList();

		ListaTurnosHistorial2025 TURNOS = await AsyncRepositorioHardCoded.GetTurnos();

		Result<SolicitudDeTurno> solicitudPaciente1 = SolicitudDeTurno.Crear(
				PACIENTES_RESULT[0],
				EspecialidadMedica2025.ClinicoGeneral,
				DateTime.Now
			)
			.PrintAndContinue("paciente1 intenta solicitar turno: ")
		;
		//IReadOnlyList<Medico2025> MEDICOS = await AsyncRepositorioHardCoded.GetMedicos();

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
