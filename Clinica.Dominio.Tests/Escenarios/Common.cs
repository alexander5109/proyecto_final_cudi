using System;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Tests.Escenarios;

public class Common {
	public static Medico2025 CrearMedico(string nombre, string apellido, string dni, string especialidadTitulo, DayOfWeek dia, TimeOnly desde, TimeOnly hasta) {
		var nombreRes = NombreCompleto2025.Crear(nombre, apellido);
		var espRes = EspecialidadMedica2025.Crear(especialidadTitulo);
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

	public static Paciente2025 CrearPaciente(string nombre, string apellido, string dni) {
		var nom = NombreCompleto2025.Crear(nombre, apellido);
		var dniRes = DniArgentino2025.Crear(dni);
		var contacto = Contacto2025.Crear(ContactoEmail2025.Crear($"{nombre.ToLower()}@mail.test"), ContactoTelefono2025.Crear("+5491123456789"));
		var dom = DomicilioArgentino2025.Crear(LocalidadDeProvincia2025.Crear("Localidad", ProvinciaArgentina2025.Crear("Buenos Aires")), "Calle 1");
		var fechaNac = FechaDeNacimiento2025.Crear(DateTime.Today.AddYears(-30));
		var fechaIng = FechaIngreso2025.Crear(DateTime.Today);

		var pac = Paciente2025.Crear(nom, dniRes, contacto, dom, fechaNac, fechaIng);
		return pac.Match(p => p, e => throw new Exception(e));
	}



}
