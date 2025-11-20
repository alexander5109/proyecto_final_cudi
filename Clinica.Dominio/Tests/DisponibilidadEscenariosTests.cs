using System;
using System.Linq;
using System.Collections.Generic;
using Clinica.Dominio.Servicios;
using Xunit;
using FluentAssertions;
using Clinica.Dominio.FunctionalProgramingTools;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.Tests.Escenarios;


public class DisponibilidadEscenariosTests {
	[Fact]
	public void Escenario_Asignar_turnos_por_orden_de_solicitud_unit_test() {
		// Arrange
		var medico1 = Common.CrearMedico("Ana", "Perez", "11111111", "Gastroenterólogo",
			DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));

		var medico2 = Common.CrearMedico("Luis", "Gomez", "22222222", "Gastroenterólogo",
			DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));

		var psicologo = Common.CrearMedico("Marta", "Lopez", "33333333", "Psicólogo",
			DayOfWeek.Tuesday, TimeOnly.Parse("10:00"), TimeOnly.Parse("14:00"));

		var repoMedicos = new FakeRepositorioMedicos(new List<Medico2025> { medico1, medico2, psicologo });
		var repoTurnos = new FakeRepositorioTurnos();

		var juan = Common.CrearPaciente("Juan", "Diaz", "44444444");
		var pedro = Common.CrearPaciente("Pedro", "Lopez", "55555555");
		var rosalia = Common.CrearPaciente("Rosalia", "Martinez", "66666666");

		var especialGastro = EspecialidadMedica2025
			.CrearPorTitulo("Gastroenterólogo")
			.Match(ok => ok, err => throw new Exception(err));

		var especialPsico = EspecialidadMedica2025
			.CrearPorTitulo("Psicólogo")
			.Match(ok => ok, err => throw new Exception(err));

		// Juan solicita gastro
		var solicitudJuan = Entidades
			.Crear(new Result<Paciente2025>.Ok(juan), new Result<EspecialidadMedica2025>.Ok(especialGastro), DateTime.Now)
			.Match(ok => ok, err => throw new Exception(err));

		// Act
		var dispJuan = solicitudJuan.BuscarDisponibilidades(repoMedicos, repoTurnos)
			.Match(ok => ok, err => throw new Exception(err))
			.First();

		var turnoJuan = Turno2025
			.Programar(new Result<Medico2025>.Ok(dispJuan.Medico),
					new Result<Paciente2025>.Ok(juan),
					new Result<EspecialidadMedica2025>.Ok(especialGastro),
					dispJuan.Inicio)
			.Match(ok => ok, err => throw new Exception(err));

		repoTurnos.Guardar(turnoJuan);

		// Pedro solicita gastro -> toma siguiente turno disponible
		var solicitudPedro = Entidades
			.Crear(new Result<Paciente2025>.Ok(pedro), new Result<EspecialidadMedica2025>.Ok(especialGastro), DateTime.Now)
			.Match(ok => ok, err => throw new Exception(err));

		var dispPedro = solicitudPedro.BuscarDisponibilidades(repoMedicos, repoTurnos)
			.Match(ok => ok, err => throw new Exception(err))
			.First();

		var turnoPedro = Turno2025
			.Programar(new Result<Medico2025>.Ok(dispPedro.Medico),
					new Result<Paciente2025>.Ok(pedro),
					new Result<EspecialidadMedica2025>.Ok(especialGastro),
					dispPedro.Inicio)
			.Match(ok => ok, err => throw new Exception(err));

		repoTurnos.Guardar(turnoPedro);

		// Rosalia solicita psicólogo
		var solicitudRosalia = Entidades
			.Crear(new Result<Paciente2025>.Ok(rosalia), new Result<EspecialidadMedica2025>.Ok(especialPsico), DateTime.Now.AddDays(7))
			.Match(ok => ok, err => throw new Exception(err));

		var dispRosalia = solicitudRosalia.BuscarDisponibilidades(repoMedicos, repoTurnos)
			.Match(ok => ok, err => throw new Exception(err))
			.First();

		var turnoRosalia = Turno2025
			.Programar(new Result<Medico2025>.Ok(dispRosalia.Medico),
					new Result<Paciente2025>.Ok(rosalia),
					new Result<EspecialidadMedica2025>.Ok(especialPsico),
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









}
