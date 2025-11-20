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

		List<Turno2025> TURNOS = [];

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
		// ⭐ 1. Juan solicita TURNO
		// ================================================================

		Paciente2025 pacienteJuan = PACIENTES[0];
		SolicitudDeTurno solicitudJuan = new(
			pacienteJuan,
			EspecialidadMedica2025.ClinicoGeneral,
			//EspecialidadMedica2025.Gastroenterologo,
			DiaSemana2025.Lunes,
			new TardeOMañana(true),
			DateTime.Now
		);
		Console.WriteLine(solicitudJuan.ATexto());

		ListaDisponibilidades2025 disponibilidadesParaJuan =
			ListaDisponibilidades2025
				.Buscar(MEDICOS, solicitudJuan.SolicitudEn)
				.Bind(seq => seq
					.Where(s => s.Especialidad == solicitudJuan.Especialidad)
					.Where(s => s.FechaHoraDesde.DayOfWeek == solicitudJuan.DiaPreferido.Valor)
					.Where(s => solicitudJuan.PrefiereTardeOMañana.AplicaA(s.FechaHoraDesde))
					.Take(3)
					.ToListaDisponibilidades2025()
				).GetOrRaise();

		Console.WriteLine(disponibilidadesParaJuan.ATexto());

		DisponibilidadEspecialidad2025 primeraDispJuan = disponibilidadesParaJuan.Valores.First();

		Console.WriteLine($"Disponibilidad elegida: {primeraDispJuan.ATexto()}");

		Turno2025 turnoJuan = Turno2025.Programar(solicitudJuan, primeraDispJuan).GetOrRaise();
		TURNOS.Add(turnoJuan);
		Console.WriteLine($"Turno guardado: {turnoJuan.ATexto()}");


		//var turnoJuan = ((Result<Turno2025>.Ok)turnoJuanRes).Valor;
		//repoTurnos.Guardar(turnoJuan);



		// ================================================================
		// ⭐ 2. Pedro solicita gastroenterólogo
		// ================================================================
		//Console.WriteLine("\n=== PEDRO solicita Gastroenterología ===");

		//var solicitudPedro = SolicitudDeTurno2.Crear(
		//	new Result<Paciente2025>.Ok(pedro),
		//	new Result<EspecialidadMedica2025>.Ok(especialGastro),
		//	DateTime.Now
		//).Match(ok => ok, err => throw new Exception(err));

		//var dispPedro = solicitudPedro.BuscarDisponibilidades(repoMedicos, repoTurnos)
		//							.Match(ok => ok, err => throw new Exception(err))
		//							.ToList();

		//Console.WriteLine($"Disponibilidades encontradas para Pedro ({dispPedro.Count}):");
		//foreach (var d in dispPedro)
		//	Console.WriteLine($" → {d.Medico.NombreCompleto.Apellido}, {d.Medico.NombreCompleto.Nombre} a las {d.Inicio}");

		//var primeraDispPedro = dispPedro.First();
		//Console.WriteLine($"Primer turno asignable a Pedro: {primeraDispPedro.Medico.NombreCompleto.Apellido} - {primeraDispPedro.Inicio}");

		//var turnoPedroRes = Turno2025.Programar(
		//	new Result<Medico2025>.Ok(primeraDispPedro.Medico),
		//	new Result<Paciente2025>.Ok(pedro),
		//	new Result<EspecialidadMedica2025>.Ok(especialGastro),
		//	primeraDispPedro.Inicio
		//);

		//var turnoPedro = ((Result<Turno2025>.Ok)turnoPedroRes).Valor;
		//repoTurnos.Guardar(turnoPedro);

		//Console.WriteLine($"✔ Turno de PEDRO guardado: {primeraDispPedro.Medico.NombreCompleto.Apellido} {primeraDispPedro.Inicio}\n");


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
