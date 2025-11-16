using System;
using System.Linq;
using System.Collections.Generic;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Repositorios;
using Clinica.Dominio.Servicios;
using Xunit;
using FluentAssertions;

namespace Clinica.Dominio.Tests.Escenarios;



public class FakeRepositorioMedicos(List<Medico2025> medicos) : IRepositorioMedicos {
    public Result<IReadOnlyList<Medico2025>> ObtenerMedicosPorEspecialidad(string especialidadTitulo) => new Result<IReadOnlyList<Medico2025>>.Ok(medicos.Where(m => m.Especialidad.Titulo == especialidadTitulo).ToList());
	public Result<Medico2025> ObtenerPorDni(string dni) => new Result<Medico2025>.Ok(medicos.First(m => m.Dni.Valor == dni));
	public Result<Medico2025> Guardar(Medico2025 medico) => new Result<Medico2025>.Ok(medico);
	public Result<bool> EliminarPorDni(string dni) => new Result<bool>.Ok(true);
}

public class FakeRepositorioTurnos : IRepositorioTurnos {
	private readonly List<Turno2025> _turnos = new();

	public Result<IReadOnlyList<Turno2025>> ObtenerTurnosPorMedicoDni(string medicoDni, DateTime desde, DateTime hasta) => new Result<IReadOnlyList<Turno2025>>.Ok(_turnos.Where(t => t.MedicoAsignado is not null && t.MedicoAsignado.Value.Dni.Valor == medicoDni && t.FechaYHora >= desde && t.FechaYHora <= hasta).ToList());

	public Result<IReadOnlyList<Turno2025>> ObtenerTurnosPorEspecialidad(string especialidadTitulo, DateTime desde, DateTime hasta) => new Result<IReadOnlyList<Turno2025>>.Ok(_turnos.Where(t => t.Especialidad.Titulo == especialidadTitulo && t.FechaYHora >= desde && t.FechaYHora <= hasta).ToList());

	public Result<Turno2025> Guardar(Turno2025 turno) { _turnos.Add(turno); return new Result<Turno2025>.Ok(turno); }
	public Result<bool> Eliminar(Turno2025 turno) { _turnos.RemoveAll(t => t.FechaYHora == turno.FechaYHora && t.Paciente.Dni.Valor == turno.Paciente.Dni.Valor); return new Result<bool>.Ok(true); }
}

public class DisponibilidadEscenariosTests {



	[Fact]
	public void Escenario_Asignar_turnos_por_orden_de_solicitud_unit_test() {
		// Arrange
		var medico1 = CrearMedico("Ana", "Perez", "11111111", "Gastroenterólogo",
			DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));

		var medico2 = CrearMedico("Luis", "Gomez", "22222222", "Gastroenterólogo",
			DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));

		var psicologo = CrearMedico("Marta", "Lopez", "33333333", "Psicólogo",
			DayOfWeek.Tuesday, TimeOnly.Parse("10:00"), TimeOnly.Parse("14:00"));

		var repoMedicos = new FakeRepositorioMedicos(new List<Medico2025> { medico1, medico2, psicologo });
		var repoTurnos = new FakeRepositorioTurnos();

		var juan = CrearPaciente("Juan", "Diaz", "44444444");
		var pedro = CrearPaciente("Pedro", "Lopez", "55555555");
		var rosalia = CrearPaciente("Rosalia", "Martinez", "66666666");

		var especialGastro = MedicoEspecialidad2025
			.Crear("Gastroenterólogo", MedicoEspecialidad2025.RamasDisponibles.First())
			.Match(ok => ok, err => throw new Exception(err));

		var especialPsico = MedicoEspecialidad2025
			.Crear("Psicólogo", MedicoEspecialidad2025.RamasDisponibles.First())
			.Match(ok => ok, err => throw new Exception(err));

		// Juan solicita gastro
		var solicitudJuan = SolicitudConsulta2025
			.Crear(new Result<Paciente2025>.Ok(juan), new Result<MedicoEspecialidad2025>.Ok(especialGastro), DateTime.Now)
			.Match(ok => ok, err => throw new Exception(err));

		// Act
		var dispJuan = solicitudJuan.BuscarDisponibilidades(repoMedicos, repoTurnos)
			.Match(ok => ok, err => throw new Exception(err))
			.First();

		var turnoJuan = Turno2025
			.Crear(new Result<Medico2025>.Ok(dispJuan.Medico),
					new Result<Paciente2025>.Ok(juan),
					new Result<MedicoEspecialidad2025>.Ok(especialGastro),
					dispJuan.Inicio)
			.Match(ok => ok, err => throw new Exception(err));

		repoTurnos.Guardar(turnoJuan);

		// Pedro solicita gastro -> toma siguiente turno disponible
		var solicitudPedro = SolicitudConsulta2025
			.Crear(new Result<Paciente2025>.Ok(pedro), new Result<MedicoEspecialidad2025>.Ok(especialGastro), DateTime.Now)
			.Match(ok => ok, err => throw new Exception(err));

		var dispPedro = solicitudPedro.BuscarDisponibilidades(repoMedicos, repoTurnos)
			.Match(ok => ok, err => throw new Exception(err))
			.First();

		var turnoPedro = Turno2025
			.Crear(new Result<Medico2025>.Ok(dispPedro.Medico),
					new Result<Paciente2025>.Ok(pedro),
					new Result<MedicoEspecialidad2025>.Ok(especialGastro),
					dispPedro.Inicio)
			.Match(ok => ok, err => throw new Exception(err));

		repoTurnos.Guardar(turnoPedro);

		// Rosalia solicita psicólogo
		var solicitudRosalia = SolicitudConsulta2025
			.Crear(new Result<Paciente2025>.Ok(rosalia), new Result<MedicoEspecialidad2025>.Ok(especialPsico), DateTime.Now.AddDays(7))
			.Match(ok => ok, err => throw new Exception(err));

		var dispRosalia = solicitudRosalia.BuscarDisponibilidades(repoMedicos, repoTurnos)
			.Match(ok => ok, err => throw new Exception(err))
			.First();

		var turnoRosalia = Turno2025
			.Crear(new Result<Medico2025>.Ok(dispRosalia.Medico),
					new Result<Paciente2025>.Ok(rosalia),
					new Result<MedicoEspecialidad2025>.Ok(especialPsico),
					dispRosalia.Inicio)
			.Match(ok => ok, err => throw new Exception(err));

		// Assert

		turnoJuan.Paciente.NombreCompleto.Apellido.Should().Be("Diaz");
		//turnoJuan.Paciente.NombreCompleto.Apellido.Should().Be("Diaz_ERROR");
		turnoPedro.Paciente.NombreCompleto.Apellido.Should().Be("Lopez");
		turnoRosalia.Paciente.NombreCompleto.Apellido.Should().Be("Martinez");

		turnoJuan.Especialidad.Should().Be(especialGastro);
		turnoPedro.Especialidad.Should().Be(especialGastro);
		turnoRosalia.Especialidad.Should().Be(especialPsico);

		turnoPedro.FechaYHora.Should().BeAfter(turnoJuan.FechaYHora);

		turnoJuan.MedicoAsignado.Should().BeOneOf(medico1, medico2);
		turnoPedro.MedicoAsignado.Should().BeOneOf(medico1, medico2);

		turnoRosalia.MedicoAsignado.Should().Be(psicologo);


	}



	private static Medico2025 CrearMedico(string nombre, string apellido, string dni, string especialidadTitulo, DayOfWeek dia, TimeOnly desde, TimeOnly hasta) {
		var nombreRes = NombreCompleto2025.Crear(nombre, apellido);
		var espRes = MedicoEspecialidad2025.Crear(especialidadTitulo, MedicoEspecialidad2025.RamasDisponibles.First());
		var dniRes = DniArgentino2025.Crear(dni);
		var domRes = DomicilioArgentino2025.Crear(LocalidadDeProvincia2025.Crear("Localidad", ProvinciaArgentina2025.Crear("Buenos Aires")), "Calle 1");
		var telRes = ContactoTelefono2025.Crear("+5491123456789");
		var horariosRes = ListaHorarioMedicos2025.Crear([ 
				HorarioMedico2025.Crear(DiaSemana2025.Crear(dia), 
				HorarioHora2025.Crear(desde), 
				HorarioHora2025.Crear(hasta))
				.Match(ok => ok, err => throw new Exception(err)) ]
		);
		var fechaIng = FechaIngreso2025.Crear(DateTime.Today);
		var sueldo = MedicoSueldoMinimo2025.Crear(250000m);

		var medRes = Medico2025.Crear(nombreRes, espRes, dniRes, domRes, telRes, horariosRes, fechaIng, sueldo, false);
		return medRes.Match(m => m, e => throw new Exception(e));
	}

	private static Paciente2025 CrearPaciente(string nombre, string apellido, string dni) {
		var nom = NombreCompleto2025.Crear(nombre, apellido);
		var dniRes = DniArgentino2025.Crear(dni);
		var contacto = Contacto2025.Crear(ContactoEmail2025.Crear($"{nombre.ToLower()}@mail.test"), ContactoTelefono2025.Crear("+5491123456789"));
		var dom = DomicilioArgentino2025.Crear(LocalidadDeProvincia2025.Crear("Localidad", ProvinciaArgentina2025.Crear("Buenos Aires")), "Calle 1");
		var fechaNac = FechaDeNacimiento2025.Crear(DateTime.Today.AddYears(-30));
		var fechaIng = FechaIngreso2025.Crear(DateTime.Today);

		var pac = Paciente2025.Crear(nom, dniRes, contacto, dom, fechaNac, fechaIng);
		return pac.Match(p => p, e => throw new Exception(e));
	}

	//[Fact]
	public static void Escenario_Asignar_turnos_por_orden_de_solicitud() {
		Console.WriteLine("=== ESCENARIO: Asignación por orden de solicitud ===");

		// Arrange
		Console.WriteLine("\n--- Creando médicos ---");
		var medico1 = CrearMedico("Ana", "Perez", "11111111", "Gastroenterólogo",
								  DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));
		var medico2 = CrearMedico("Luis", "Gomez", "22222222", "Gastroenterólogo",
								  DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));
		var psicologo = CrearMedico("Marta", "Lopez", "33333333", "Psicólogo",
									DayOfWeek.Tuesday, TimeOnly.Parse("10:00"), TimeOnly.Parse("14:00"));

		var repoMedicos = new FakeRepositorioMedicos([medico1, medico2, psicologo]);
		var repoTurnos = new FakeRepositorioTurnos();

		Console.WriteLine("Médicos cargados: Ana, Luis y Marta.\n");

		Console.WriteLine("--- Creando pacientes ---");
		var juan = CrearPaciente("Juan", "Diaz", "44444444");
		var pedro = CrearPaciente("Pedro", "Lopez", "55555555");
		var rosalia = CrearPaciente("Rosalia", "Martinez", "66666666");
		Console.WriteLine("Pacientes cargados: Juan, Pedro y Rosalia.\n");

		var especialGastro = MedicoEspecialidad2025.Crear("Gastroenterólogo", MedicoEspecialidad2025.RamasDisponibles.First())
							   .Match(ok => ok, err => throw new Exception(err));

		var especialPsico = MedicoEspecialidad2025.Crear("Psicólogo", MedicoEspecialidad2025.RamasDisponibles.First())
							   .Match(ok => ok, err => throw new Exception(err));


		// ================================================================
		// ⭐ 1. Juan solicita gastroenterólogo
		// ================================================================
		Console.WriteLine("\n=== JUAN solicita Gastroenterología ===");

		var solicitudJuan = SolicitudConsulta2025.Crear(
			new Result<Paciente2025>.Ok(juan),
			new Result<MedicoEspecialidad2025>.Ok(especialGastro),
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

		var turnoJuanRes = Turno2025.Crear(
			new Result<Medico2025>.Ok(primeraDispJuan.Medico),
			new Result<Paciente2025>.Ok(juan),
			new Result<MedicoEspecialidad2025>.Ok(especialGastro),
			primeraDispJuan.Inicio
		);

		var turnoJuan = ((Result<Turno2025>.Ok)turnoJuanRes).Valor;
		repoTurnos.Guardar(turnoJuan);

		Console.WriteLine($"✔ Turno de JUAN guardado: {primeraDispJuan.Medico.NombreCompleto.Apellido} {primeraDispJuan.Inicio}\n");


		// ================================================================
		// ⭐ 2. Pedro solicita gastroenterólogo
		// ================================================================
		Console.WriteLine("\n=== PEDRO solicita Gastroenterología ===");

		var solicitudPedro = SolicitudConsulta2025.Crear(
			new Result<Paciente2025>.Ok(pedro),
			new Result<MedicoEspecialidad2025>.Ok(especialGastro),
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

		var turnoPedroRes = Turno2025.Crear(
			new Result<Medico2025>.Ok(primeraDispPedro.Medico),
			new Result<Paciente2025>.Ok(pedro),
			new Result<MedicoEspecialidad2025>.Ok(especialGastro),
			primeraDispPedro.Inicio
		);

		var turnoPedro = ((Result<Turno2025>.Ok)turnoPedroRes).Valor;
		repoTurnos.Guardar(turnoPedro);

		Console.WriteLine($"✔ Turno de PEDRO guardado: {primeraDispPedro.Medico.NombreCompleto.Apellido} {primeraDispPedro.Inicio}\n");


		// ================================================================
		// ⭐ 3. Rosalia solicita psicología para la próxima semana
		// ================================================================
		Console.WriteLine("\n=== ROSALIA solicita Psicología (semana próxima) ===");

		var solicitudRosalia = SolicitudConsulta2025.Crear(
			new Result<Paciente2025>.Ok(rosalia),
			new Result<MedicoEspecialidad2025>.Ok(especialPsico),
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

		var turnoRosaliaRes = Turno2025.Crear(
			new Result<Medico2025>.Ok(primeraDispRosalia.Medico),
			new Result<Paciente2025>.Ok(rosalia),
			new Result<MedicoEspecialidad2025>.Ok(especialPsico),
			primeraDispRosalia.Inicio
		);

		var turnoRosalia = ((Result<Turno2025>.Ok)turnoRosaliaRes).Valor;

		Console.WriteLine($"✔ Turno de ROSALIA asignado a {primeraDispRosalia.Medico.NombreCompleto.Apellido} el {primeraDispRosalia.Inicio}\n");

		Console.WriteLine("\n=== ESCENARIO COMPLETADO ===\n");
	}










}
