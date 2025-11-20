using System;
using System.Linq;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.FunctionalProgramingTools;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.Tests.Escenarios;

public class DisponibilidadEscenariosTestConsole {
	//[Fact]
	public static void Escenario_Asignar_turnos_por_orden_de_solicitud() {
		Console.WriteLine("=== ESCENARIO: Asignación por orden de solicitud ===");

		// Arrange
		Console.WriteLine("\n--- Creando médicos ---");
		var medico1 = Common.CrearMedico("Ana", "Perez", "11111111", "Gastroenterólogo",
								  DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));
		var medico2 = Common.CrearMedico("Luis", "Gomez", "22222222", "Gastroenterólogo",
								  DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));
		var psicologo = Common.CrearMedico("Marta", "Lopez", "33333333", "Psicólogo",
									DayOfWeek.Tuesday, TimeOnly.Parse("10:00"), TimeOnly.Parse("14:00"));

		var repoMedicos = new FakeRepositorioMedicos([medico1, medico2, psicologo]);
		var repoTurnos = new FakeRepositorioTurnos();

		Console.WriteLine("Médicos cargados: Ana, Luis y Marta.\n");

		Console.WriteLine("--- Creando pacientes ---");
		var juan = Common.CrearPaciente("Juan", "Diaz", "44444444");
		var pedro = Common.CrearPaciente("Pedro", "Lopez", "55555555");
		var rosalia = Common.CrearPaciente("Rosalia", "Martinez", "66666666");
		Console.WriteLine("Pacientes cargados: Juan, Pedro y Rosalia.\n");

		var especialGastro = EspecialidadMedica2025.CrearPorTitulo("Gastroenterólogo")
							   .Match(ok => ok, err => throw new Exception(err));

		var especialPsico = EspecialidadMedica2025.CrearPorTitulo("Psicólogo")
							   .Match(ok => ok, err => throw new Exception(err));


		// ================================================================
		// ⭐ 1. Juan solicita gastroenterólogo
		// ================================================================
		Console.WriteLine("\n=== JUAN solicita Gastroenterología ===");

		var solicitudJuan = Entidades.Crear(
			new Result<Paciente2025>.Ok(juan),
			new Result<EspecialidadMedica2025>.Ok(especialGastro),
            DateTime.Now
		).Match(ok => ok, err => throw new Exception(err));

		var dispJuan = solicitudJuan.BuscarDisponibilidades(repoMedicos, repoTurnos)
									.Match(ok => ok, err => throw new Exception(err))
									.ToList();

		Console.WriteLine($"Disponibilidades encontradas para Juan ({dispJuan.Count}):");
		foreach (var d in dispJuan)
			Console.WriteLine($" → {d.Medico.NombreCompleto.Apellido}, {d.Medico.NombreCompleto.Nombre} a las {d.Inicio}");

		var primeraDispJuan = dispJuan.First();
		Console.WriteLine($"Primer turno asignable a Juan: {primeraDispJuan.Medico.NombreCompleto.Apellido} - {primeraDispJuan.Inicio}");

		var turnoJuanRes = Turno2025.Programar(
			new Result<Medico2025>.Ok(primeraDispJuan.Medico),
			new Result<Paciente2025>.Ok(juan),
			new Result<EspecialidadMedica2025>.Ok(especialGastro),
			primeraDispJuan.Inicio
		);

		var turnoJuan = ((Result<Turno2025>.Ok)turnoJuanRes).Valor;
		repoTurnos.Guardar(turnoJuan);

		Console.WriteLine($"✔ Turno de JUAN guardado: {primeraDispJuan.Medico.NombreCompleto.Apellido} {primeraDispJuan.Inicio}\n");


		// ================================================================
		// ⭐ 2. Pedro solicita gastroenterólogo
		// ================================================================
		Console.WriteLine("\n=== PEDRO solicita Gastroenterología ===");

		var solicitudPedro = Entidades.Crear(
			new Result<Paciente2025>.Ok(pedro),
			new Result<EspecialidadMedica2025>.Ok(especialGastro),
            DateTime.Now
		).Match(ok => ok, err => throw new Exception(err));

		var dispPedro = solicitudPedro.BuscarDisponibilidades(repoMedicos, repoTurnos)
									.Match(ok => ok, err => throw new Exception(err))
									.ToList();

		Console.WriteLine($"Disponibilidades encontradas para Pedro ({dispPedro.Count}):");
		foreach (var d in dispPedro)
			Console.WriteLine($" → {d.Medico.NombreCompleto.Apellido}, {d.Medico.NombreCompleto.Nombre} a las {d.Inicio}");

		var primeraDispPedro = dispPedro.First();
		Console.WriteLine($"Primer turno asignable a Pedro: {primeraDispPedro.Medico.NombreCompleto.Apellido} - {primeraDispPedro.Inicio}");

		var turnoPedroRes = Turno2025.Programar(
			new Result<Medico2025>.Ok(primeraDispPedro.Medico),
			new Result<Paciente2025>.Ok(pedro),
			new Result<EspecialidadMedica2025>.Ok(especialGastro),
			primeraDispPedro.Inicio
		);

		var turnoPedro = ((Result<Turno2025>.Ok)turnoPedroRes).Valor;
		repoTurnos.Guardar(turnoPedro);

		Console.WriteLine($"✔ Turno de PEDRO guardado: {primeraDispPedro.Medico.NombreCompleto.Apellido} {primeraDispPedro.Inicio}\n");


		// ================================================================
		// ⭐ 3. Rosalia solicita psicología para la próxima semana
		// ================================================================
		Console.WriteLine("\n=== ROSALIA solicita Psicología (semana próxima) ===");

		var solicitudRosalia = Entidades.Crear(
			new Result<Paciente2025>.Ok(rosalia),
			new Result<EspecialidadMedica2025>.Ok(especialPsico),
            DateTime.Now.AddDays(7)
		).Match(ok => ok, err => throw new Exception(err));

		var dispRosalia = solicitudRosalia.BuscarDisponibilidades(repoMedicos, repoTurnos)
										 .Match(ok => ok, err => throw new Exception(err))
										 .ToList();

		Console.WriteLine($"Disponibilidades encontradas para Rosalia ({dispRosalia.Count}):");
		foreach (var d in dispRosalia)
			Console.WriteLine($" → Medico {d.Medico.NombreCompleto.Apellido} {d.Medico.NombreCompleto.Nombre}, a las {d.Inicio}");

		var primeraDispRosalia = dispRosalia.First();
		Console.WriteLine($"Primer turno asignable a Rosalia: {primeraDispRosalia.Medico.NombreCompleto.Apellido} - {primeraDispRosalia.Inicio}");

		var turnoRosaliaRes = Turno2025.Programar(
			new Result<Medico2025>.Ok(primeraDispRosalia.Medico),
			new Result<Paciente2025>.Ok(rosalia),
			new Result<EspecialidadMedica2025>.Ok(especialPsico),
			primeraDispRosalia.Inicio
		);

		var turnoRosalia = ((Result<Turno2025>.Ok)turnoRosaliaRes).Valor;

		Console.WriteLine($"✔ Turno de ROSALIA asignado a {primeraDispRosalia.Medico.NombreCompleto.Apellido} el {primeraDispRosalia.Inicio}\n");

		Console.WriteLine("\n=== ESCENARIO COMPLETADO ===\n");
	}

}
