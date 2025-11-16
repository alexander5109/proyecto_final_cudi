using System;
using System.Linq;
using System.Collections.Generic;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Repositorios;
using System.Net.Http.Headers;
using Clinica.Dominio.Servicios;

namespace Clinica.Dominio.Tests.Escenarios;

public class FakeRepositorioMedicos : IRepositorioMedicos {
	private readonly List<Medico2025> _medicos;
	public FakeRepositorioMedicos(List<Medico2025> medicos) => _medicos = medicos;
	public Result<IReadOnlyList<Medico2025>> ObtenerMedicosPorEspecialidad(string especialidadTitulo) => new Result<IReadOnlyList<Medico2025>>.Ok(_medicos.Where(m => m.Especialidad.Titulo == especialidadTitulo).ToList());
	public Result<Medico2025> ObtenerPorDni(string dni) => new Result<Medico2025>.Ok(_medicos.First(m => m.Dni.Valor == dni));
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
	private Medico2025 CrearMedico(string nombre, string apellido, string dni, string especialidadTitulo, DayOfWeek dia, TimeOnly desde, TimeOnly hasta) {
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

	private Paciente2025 CrearPaciente(string nombre, string apellido, string dni) {
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
	public void Escenario_Asignar_turnos_por_orden_de_solicitud() {
		// Arrange
		var medico1 = CrearMedico("Ana", "Perez", "11111111", "Gastroenterólogo", DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));
		var medico2 = CrearMedico("Luis", "Gomez", "22222222", "Gastroenterólogo", DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("12:00"));
		var psicologo = CrearMedico("Marta", "Lopez", "33333333", "Psicólogo", DayOfWeek.Tuesday, TimeOnly.Parse("10:00"), TimeOnly.Parse("14:00"));

		var repoMedicos = new FakeRepositorioMedicos(new List<Medico2025> { medico1, medico2, psicologo });
		var repoTurnos = new FakeRepositorioTurnos();

		var juan = CrearPaciente("Juan", "Diaz", "44444444");
		var pedro = CrearPaciente("Pedro", "Lopez", "55555555");
		var rosalia = CrearPaciente("Rosalia", "Martinez", "66666666");

		var especialGastro = MedicoEspecialidad2025.Crear("Gastroenterólogo", MedicoEspecialidad2025.RamasDisponibles.First()).Match(ok => ok, err => throw new Exception(err));
		var especialPsico = MedicoEspecialidad2025.Crear("Psicólogo", MedicoEspecialidad2025.RamasDisponibles.First()).Match(ok => ok, err => throw new Exception(err));

        // Juan solicita el turno mas pronto para gastro (sin preferencia horaria)
        SolicitudConsulta2025 solicitudJuan = SolicitudConsulta2025.Crear(new Result<Paciente2025>.Ok(juan), new Result<MedicoEspecialidad2025>.Ok(especialGastro), DateTime.Now).Match(ok => ok, err => throw new Exception(err));

        // Act: buscar primera disponibilidad y crear turno
        IEnumerable<EspecialidadDisponibilidadHoraria> solicitudJuanResults = solicitudJuan.BuscarDisponibilidades(repoMedicos, repoTurnos).Match(ok => ok, err => throw new Exception(err));

		EspecialidadDisponibilidadHoraria primeraDispJuan = solicitudJuanResults.FirstOrDefault(); ;

		//Assert.NotNull(primeraDispJuan);

		var turnoJuanRes = Turno2025.Crear(new Result<Medico2025>.Ok(primeraDispJuan.Medico), new Result<Paciente2025>.Ok(juan), new Result<MedicoEspecialidad2025>.Ok(especialGastro), primeraDispJuan.Inicio);
		//Assert.True(turnoJuanRes.IsOk);
		var turnoJuan = ((Result<Turno2025>.Ok)turnoJuanRes).Valor;
		var saved1 = repoTurnos.Guardar(turnoJuan);
		//Assert.True(saved1.IsOk);

        // Pedro solicita tambien gastro -> debe tomar siguiente slot disponible
        SolicitudConsulta2025 solicitudPedro = SolicitudConsulta2025.Crear(new Result<Paciente2025>.Ok(pedro), new Result<MedicoEspecialidad2025>.Ok(especialGastro), DateTime.Now).Match(ok => ok, err => throw new Exception(err));
		var primeraDispPedro = solicitudPedro.BuscarDisponibilidades(repoMedicos, repoTurnos).Match(ok => ok, err => throw new Exception(err)).FirstOrDefault();
		//Assert.NotNull(primeraDispPedro);
		var turnoPedroRes = Turno2025.Crear(new Result<Medico2025>.Ok(primeraDispPedro.Medico), new Result<Paciente2025>.Ok(pedro), new Result<MedicoEspecialidad2025>.Ok(especialGastro), primeraDispPedro.Inicio);
		//Assert.True(turnoPedroRes.IsOk);

		var turnoPedro = ((Result<Turno2025>.Ok)turnoPedroRes).Valor;
		var saved2 = repoTurnos.Guardar(turnoPedro);
		//Assert.True(saved2.IsOk);

		// Rosalia solicita psicologo, pero a partir de la semana que viene
		var solicitudRosalia = SolicitudConsulta2025.Crear(new Result<Paciente2025>.Ok(rosalia), new Result<MedicoEspecialidad2025>.Ok(especialPsico), DateTime.Now.AddDays(7)).Match(ok => ok, err => throw new Exception(err));
		var primeraDispRosalia = solicitudRosalia.BuscarDisponibilidades(repoMedicos, repoTurnos).Match(ok => ok, err => throw new Exception(err)).FirstOrDefault();
		//Assert.NotNull(primeraDispRosalia);
		var turnoRosaliaRes = Turno2025.Crear(new Result<Medico2025>.Ok(primeraDispRosalia.Medico), new Result<Paciente2025>.Ok(rosalia), new Result<MedicoEspecialidad2025>.Ok(especialPsico), primeraDispRosalia.Inicio);
		//Assert.True(turnoRosaliaRes.IsOk);
	}
}
