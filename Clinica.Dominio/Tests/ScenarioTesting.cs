using System;
using System.Linq;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Tests.Escenarios;


public class ScenarioTesting {
	//[Fact]
	public static void Escenario_Asignar_turnos_por_orden_de_solicitud() {
		// Arrange
		Console.WriteLine("\n--- Creando médicos ---");

		ListaTurnosHistorial2025 TURNOS = ListaTurnosHistorial2025.Crear();

		List<Medico2025> MEDICOS = [
			Medico2025.Crear(
				NombreCompleto2025.Crear("Carlos Alfredo", "Markier"),
				ListaEspecialidadesMedicas2025.Crear([
					EspecialidadMedica2025.Gastroenterologo,
					EspecialidadMedica2025.ClinicoGeneral
				]),
				DniArgentino2025.Crear("15350996"),
				DomicilioArgentino2025.Crear(
					LocalidadDeProvincia2025.Crear(
						"Morón",
						ProvinciaArgentina2025.Crear("Buenos Aires")
					),
					"Avenida Rivadavia 2323"
				),
				ContactoTelefono2025.Crear("1133832021"),
				ListaHorarioMedicos2025.Crear([
					HorarioMedico2025.Crear(
						DiaSemana2025.Lunes,
						new HorarioHora2025(TimeOnly.Parse("09:00")),
						new HorarioHora2025(TimeOnly.Parse("15:00"))
					),
					HorarioMedico2025.Crear(
						DiaSemana2025.Miercoles,
						new HorarioHora2025(TimeOnly.Parse("09:00")),
						new HorarioHora2025(TimeOnly.Parse("15:00"))
					)
				]),
				FechaIngreso2025.Crear(DateTime.Parse("2014/12/12")),
				MedicoSueldoMinimo2025.Crear(800_000),
				false
			).GetOrRaise(),

			Medico2025.Crear(
				NombreCompleto2025.Crear("Jorge", "Pereyra"),
				ListaEspecialidadesMedicas2025.Crear([
					EspecialidadMedica2025.Pediatra,
					EspecialidadMedica2025.Ginecologo
				]),
				DniArgentino2025.Crear("20350996"),
				DomicilioArgentino2025.Crear(
					LocalidadDeProvincia2025.Crear(
						"Gregorio de Laferrere",
						ProvinciaArgentina2025.Crear("Buenos Aires")
					),
					"Armonia 23231"
				),
				ContactoTelefono2025.Crear("1163632071"),
				ListaHorarioMedicos2025.Crear([
					HorarioMedico2025.Crear(
						DiaSemana2025.Martes,
						new HorarioHora2025(TimeOnly.Parse("08:00")),
						new HorarioHora2025(TimeOnly.Parse("18:00"))
					),
					HorarioMedico2025.Crear(
						DiaSemana2025.Jueves,
						new HorarioHora2025(TimeOnly.Parse("08:00")),
						new HorarioHora2025(TimeOnly.Parse("18:00"))
					)
				]),
				FechaIngreso2025.Crear(DateTime.Parse("2014/12/12")),
				MedicoSueldoMinimo2025.Crear(800_000),
				false
			).GetOrRaise(),
			Medico2025.Crear(
				NombreCompleto2025.Crear("Marta", "Algerich"),
				ListaEspecialidadesMedicas2025.Crear([
					EspecialidadMedica2025.Neurologo,
					EspecialidadMedica2025.Osteopata
				]),
				DniArgentino2025.Crear("10350996"),
				DomicilioArgentino2025.Crear(
					LocalidadDeProvincia2025.Crear(
						"Palermo",
						ProvinciaArgentina2025.Crear("Buenos Aires")
					),
					"Entre Rios 123"
				),
				ContactoTelefono2025.Crear("1149920537"),
				ListaHorarioMedicos2025.Crear([
					HorarioMedico2025.Crear(
						DiaSemana2025.Lunes,
						new HorarioHora2025(TimeOnly.Parse("08:00")),
						new HorarioHora2025(TimeOnly.Parse("12:00"))
					),
					HorarioMedico2025.Crear(
						DiaSemana2025.Miercoles,
						new HorarioHora2025(TimeOnly.Parse("08:00")),
						new HorarioHora2025(TimeOnly.Parse("12:00"))
					),
					HorarioMedico2025.Crear(
						DiaSemana2025.Viernes,
						new HorarioHora2025(TimeOnly.Parse("08:00")),
						new HorarioHora2025(TimeOnly.Parse("12:00"))
					)
				]),
				FechaIngreso2025.Crear(DateTime.Parse("2013/12/12")),
				MedicoSueldoMinimo2025.Crear(1300_000),
				false
			).GetOrRaise()
		];



		List<Paciente2025> PACIENTES = [
			Paciente2025.Crear(
				NombreCompleto2025.Crear("Juan", "Diaz"),
				DniArgentino2025.Crear("44444444"),
				Contacto2025.Crear(
					ContactoEmail2025.Crear("juandiaz@gmail.com"),
					ContactoTelefono2025.Crear("1155544433")
				),
				DomicilioArgentino2025.Crear(
					LocalidadDeProvincia2025.Crear(
						"Lanus",
						ProvinciaArgentina2025.Crear("Buenos Aires")
					),
					"Calle Falsa 123"
				),
				FechaDeNacimiento2025.Crear(DateTime.Parse("1990/05/15")),
				FechaIngreso2025.Crear(DateTime.Parse("2022/01/10"))
			).GetOrRaise(),

			Paciente2025.Crear(
				NombreCompleto2025.Crear("Pedro", "Fernandez"),
				DniArgentino2025.Crear("30350123"),
				Contacto2025.Crear(
					ContactoEmail2025.Crear("pedroFalopa@gmail.com"),
					ContactoTelefono2025.Crear("11655414253")
				),
				DomicilioArgentino2025.Crear(
					LocalidadDeProvincia2025.Crear(
						"Castillo",
						ProvinciaArgentina2025.Crear("Buenos Aires")
					),
					"Palmerita 12223"
				),
				FechaDeNacimiento2025.Crear(DateTime.Parse("1996/05/15")),
				FechaIngreso2025.Crear(DateTime.Parse("2024/01/10"))
			).GetOrRaise(),

			Paciente2025.Crear(
				NombreCompleto2025.Crear("Herminda", "Gutierrez Lopez"),
				DniArgentino2025.Crear("44444444"),
				Contacto2025.Crear(
					ContactoEmail2025.Crear("hermindalaturri201@gmail.com"),
					ContactoTelefono2025.Crear("11225411453")
				),
				DomicilioArgentino2025.Crear(
					LocalidadDeProvincia2025.Crear(
						"Virrey del Pinos",
						ProvinciaArgentina2025.Crear("Buenos Aires")
					),
					"Urien 2223"
				),
				FechaDeNacimiento2025.Crear(DateTime.Parse("1994/05/15")),
				FechaIngreso2025.Crear(DateTime.Parse("2023/01/10"))
			).GetOrRaise(),

		];


		// ================================================================
		// ⭐ 1. Juan solicita ClinicoGeneral
		// ================================================================


		Paciente2025 pacienteJuan = PACIENTES[0];
		Result<SolicitudDeTurno> solicitudJuan = SolicitudDeTurno.Crear(
			pacienteJuan,
			EspecialidadMedica2025.ClinicoGeneral,
			DateTime.Now
		).PrintAndContinue("Creando paciente: ");



		Result<ListaDisponibilidades2025> resultDisponibilidadesParaJuan =
			ListaDisponibilidades2025.Buscar(solicitudJuan, MEDICOS, TURNOS, 3)
			.AplicarFiltrosOpcionales(new(
				DiaSemana2025.Lunes,
				new TardeOMañana(true)
			));
		Result<DisponibilidadEspecialidad2025> primeraDispJuan = resultDisponibilidadesParaJuan.TomarPrimera();

		TURNOS.AgendarTurno(Turno2025.Programar(solicitudJuan, primeraDispJuan));


		// ================================================================
		// ⭐ 2. Pedro tambien solicita  ClinicoGeneral
		// ================================================================




		// ================================================================
		// ⭐ 3. Rosalia solicita psicología para la próxima semana
		// ================================================================
		//Console.WriteLine("\n=== ROSALIA solicita Psicología (semana próxima) ===");

		//var solicitudRosalia = SolicitudDeTurno2.Crear(
		//	new Result<Paciente2025>.Ok(rosalia),
		//	new Result<EspecialidadMedica2025>.Ok(especialPsico),
		//	DateTime.Now.AddDays(7)
		//).Match(ok => ok, err => throw new Exception(err));

		//var dispRosalia = solicitudRosalia.BuscarDisponibilidades(repoMedicos, repoTurnos)
		//								 .Match(ok => ok, err => throw new Exception(err))
		//								 .ToList();

		//Console.WriteLine($"Disponibilidades encontradas para Rosalia ({dispRosalia.Count}):");
		//foreach (var d in dispRosalia)
		//	Console.WriteLine($" → Medico {d.Medico.NombreCompleto.Apellido} {d.Medico.NombreCompleto.Nombre}, a las {d.Inicio}");

		//var primeraDispRosalia = dispRosalia.First();
		//Console.WriteLine($"Primer turno asignable a Rosalia: {primeraDispRosalia.Medico.NombreCompleto.Apellido} - {primeraDispRosalia.Inicio}");

		//var turnoRosaliaRes = Turno2025.Programar(
		//	new Result<Medico2025>.Ok(primeraDispRosalia.Medico),
		//	new Result<Paciente2025>.Ok(rosalia),
		//	new Result<EspecialidadMedica2025>.Ok(especialPsico),
		//	primeraDispRosalia.Inicio
		//);

		//var turnoRosalia = ((Result<Turno2025>.Ok)turnoRosaliaRes).Valor;

		//Console.WriteLine($"✔ Turno de ROSALIA asignado a {primeraDispRosalia.Medico.NombreCompleto.Apellido} el {primeraDispRosalia.Inicio}\n");

		//Console.WriteLine("\n=== ESCENARIO COMPLETADO ===\n");
	}

}
