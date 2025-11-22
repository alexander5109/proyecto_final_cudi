
using System.Collections.Generic;
using System.Collections.Immutable;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.PruebasDeConsola;

public static class MainProgram {
	static async Task Main() {
		Console.OutputEncoding = System.Text.Encoding.UTF8;


		List<Result<Paciente2025>> PACIENTES_RESULT = await AsyncRepositorioDapper.GetPacientes();
		Console.WriteLine(PACIENTES_RESULT.Count);

		IReadOnlyList<Paciente2025> PACIENTES = [.. PACIENTES_RESULT
			.Select(r => r.PrintAndContinue("Paciente domainizado")
						  .GetOrRaise())];

		IReadOnlyList<Medico2025> MEDICOS = [.. (await AsyncRepositorioDapper.GetMedicos())
			.Select(r => r.PrintAndContinue("Medico domainizado")
						  .GetOrRaise())];

        List<Result<Turno2025>> TURNOS_RESULT = await AsyncRepositorioDapper.GetTurnos(); //hello?
		List<Turno2025> TURNOS = [.. TURNOS_RESULT
			.Select(r => r.PrintAndContinue("Turno domainizado")
						  .GetOrRaise())];

		ServicioTurnosManager TURNOS_MANAGER = new(TURNOS);

		Result<SolicitudDeTurno> solicitudPaciente1 = SolicitudDeTurno.Crear(
				PACIENTES_RESULT[0].GetOrRaise().Id,
				EspecialidadMedica2025.ClinicoGeneral,
				DateTime.Now
			)
			.PrintAndContinue("paciente1 intenta solicitar turno: ")
		;
		//IReadOnlyList<Medico2025> MEDICOS = await AsyncRepositorioHardCoded.GetMedicos();

		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1 = ServicioDisponibilidadesSearcher
			.Buscar(solicitudPaciente1, MEDICOS, TURNOS_MANAGER, 3)
			.PrintAndContinue("Buscando disponibilidades: ")
			//.AplicarFiltrosOpcionales(new(DiaSemana2025.Lunes, new TardeOMañana(false)))
			//.PrintAndContinue("AplicarFiltrosOpcionales: ")
			.TomarPrimera()
			.PrintAndContinue("Tomando la primera: ")
		;

		Result<Turno2025> turno = Turno2025.Crear(new TurnoId(1), solicitudPaciente1, disponibilidadParaPaciente1)
			.PrintAndContinue("Creando turno: ")
		;

		TURNOS_MANAGER.AgendarTurno(turno)
			.PrintAndContinue("Agendando turno: ")
		;


		TURNOS_MANAGER.CancelarTurno(
			turno,
			Option<DateTime>.Some(turno.GetOrRaise().FechaDeCreacion.AddDays(1)),
			Option<string>.Some("Paciente solicita reprogramacion. No dio explicaciones.")
			)
			.PrintAndContinue("paciente1 pide cancelar turno: ")
		;


		// Paciente quiere reprogramar.
		Result<SolicitudDeTurno> solicitudPaciente1_reprogramacion = SolicitudDeTurno.Crear(
				PACIENTES_RESULT[0].GetOrRaise().Id,
				EspecialidadMedica2025.ClinicoGeneral,
				DateTime.Now.AddDays(1)
			)
			.PrintAndContinue("paciente1 intenta solicitar un nuevo turno: ")
		;

		Result<DisponibilidadEspecialidad2025> disponibilidadParaPaciente1_reprogramado = ServicioDisponibilidadesSearcher
			.Buscar(solicitudPaciente1_reprogramacion, MEDICOS, TURNOS_MANAGER, 3)
			.PrintAndContinue("Buscando disponibilidades: ")
			//.AplicarFiltrosOpcionales(new(DiaSemana2025.Lunes, new TardeOMañana(false)))
			//.PrintAndContinue("AplicarFiltrosOpcionales: ")
			.TomarPrimera()
			.PrintAndContinue("Tomando la primera: ")
		;


		Result<Turno2025> nuevoTurno = Turno2025.Crear(new TurnoId(2), solicitudPaciente1_reprogramacion, disponibilidadParaPaciente1_reprogramado)
			.PrintAndContinue("Creando nuevo turno: ")
		;

		TURNOS_MANAGER.AgendarTurno(nuevoTurno)
			.PrintAndContinue("Agendando nuevo turno: ")
		;


	}

}
