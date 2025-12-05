using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.PruebasDeConsola;

public static class AsyncRepositorioHardCoded  {
	public static Task<List<Medico2025>> GetMedicos()
		=> Task.FromResult(new List<Medico2025> {
		Medico2025.Crear(
				MedicoId.Crear(1),
				NombreCompleto2025.Crear("Carlos Alfredo", "Markier"),
				Especialidad2025.CrearPorCodigoInterno(Especialidad2025.Gastroenterologo.Codigo),
				//ListaEspecialidadesMedicas2025.CrearResult([
				//	Especialidad2025.Gastroenterologo,
				//	Especialidad2025.ClinicoGeneral
				//]),
				DniArgentino2025.Crear("15350996"),
				DomicilioArgentino2025.Crear(
					LocalidadDeProvincia2025.Crear(
						"Morón",
						ProvinciaArgentina2025.Crear("Buenos Aires")
					),
					"Avenida Rivadavia 2323"
				),
				ContactoTelefono2025.Crear("1133832021"),
				ContactoEmail2025.Crear("carlosmerkeir@gmail.com"),
				ListaHorarioMedicos2025.Crear([
					Horario2025.Crear(
						DiaSemana2025.Lunes,
						new HorarioHora2025(TimeOnly.Parse("09:00")),
						new HorarioHora2025(TimeOnly.Parse("15:00")),
						new VigenciaHorario2025(new DateOnly(2024, 1, 1)),
						new VigenciaHorario2025(new DateOnly(2026, 1, 1))
					),
					Horario2025.Crear(
						DiaSemana2025.Miercoles,
						new HorarioHora2025(TimeOnly.Parse("09:00")),
						new HorarioHora2025(TimeOnly.Parse("15:00")),
						new VigenciaHorario2025(new DateOnly(2024, 1, 1)),
						new VigenciaHorario2025(new DateOnly(2026, 1, 1))
					)
				]),
				FechaRegistro2025.Crear(DateTime.Parse("2014/12/12")),
				false
			)
			.PrintAndContinue("Creando a clinico general-gastroenterologo Carlos Merkier: ")
			.GetOrRaise()
			,

			Medico2025.Crear(
				MedicoId.Crear(2),
				NombreCompleto2025.Crear("Jorge", "Pereyra"),
				Especialidad2025.CrearPorCodigoInterno(Especialidad2025.Ginecologo.Codigo),
				//ListaEspecialidadesMedicas2025.CrearResult([
				//	Especialidad2025.Pediatra,
				//	Especialidad2025.Ginecologo
				//]),
				DniArgentino2025.Crear("20350996"),
				DomicilioArgentino2025.Crear(
					LocalidadDeProvincia2025.Crear(
						"Gregorio de Laferrere",
						ProvinciaArgentina2025.Crear("Buenos Aires")
					),
					"Armonia 23231"
				),
				ContactoTelefono2025.Crear("1163632071"),
				ContactoEmail2025.Crear("jorgepereiyra@gmail.com"),
				ListaHorarioMedicos2025.Crear([
					Horario2025.Crear(
						DiaSemana2025.Martes,
						new HorarioHora2025(new TimeOnly(08, 0)),
						new HorarioHora2025(new TimeOnly(18, 00)),
						new VigenciaHorario2025(new DateOnly(2024, 1, 12)),
						new VigenciaHorario2025(new DateOnly(2026, 1, 12))
					),
					Horario2025.Crear(
						DiaSemana2025.Jueves,
						new HorarioHora2025(new TimeOnly(08,00)),
						new HorarioHora2025(new TimeOnly(18,0)),
						new VigenciaHorario2025(new DateOnly(2024, 1, 12)),
						new VigenciaHorario2025(new DateOnly(2026, 1, 12))
					)
				]),
				FechaRegistro2025.Crear(new DateTime(2014, 12, 12)),
				false
			)
			.PrintAndContinue("Creando a pediatra-ginecologo Jorge Pereyra: ")
			.GetOrRaise()
			,

			Medico2025.Crear(
				MedicoId.Crear(3),
				NombreCompleto2025.Crear("Marta", "Algerich"),
				Especialidad2025.CrearPorCodigoInterno(Especialidad2025.Neurologo.Codigo),
				//ListaEspecialidadesMedicas2025.CrearResult([
					//new Result.Ok<Especialidad2025>(Especialidad2025.Neurologo),
					//Especialidad2025.Osteopata
				//]),
				DniArgentino2025.Crear("10350996"),
				DomicilioArgentino2025.Crear(
					LocalidadDeProvincia2025.Crear(
						"Palermo",
						ProvinciaArgentina2025.Crear("Buenos Aires")
					),
					"Entre Rios 123"
				),
				ContactoTelefono2025.Crear("1149920537"),
				ContactoEmail2025.Crear("martaalgerich@gmail.com"),
				ListaHorarioMedicos2025.Crear([
					Horario2025.Crear(
						DiaSemana2025.Lunes,
						new HorarioHora2025(TimeOnly.Parse("08:00")),
						new HorarioHora2025(TimeOnly.Parse("12:00")),
						new VigenciaHorario2025(new DateOnly(2022, 1, 12)),
						new VigenciaHorario2025(new DateOnly(2024, 1, 12))
					),
					Horario2025.Crear(
						DiaSemana2025.Miercoles,
						new HorarioHora2025(TimeOnly.Parse("08:00")),
						new HorarioHora2025(TimeOnly.Parse("12:00")),
						new VigenciaHorario2025(new DateOnly(2022, 1, 12)),
						new VigenciaHorario2025(new DateOnly(2024, 1, 12))
					),
					Horario2025.Crear(
						DiaSemana2025.Viernes,
						new HorarioHora2025(TimeOnly.Parse("08:00")),
						new HorarioHora2025(TimeOnly.Parse("12:00")),
						new VigenciaHorario2025(new DateOnly(2022, 1, 12)),
						new VigenciaHorario2025(new DateOnly(2024, 1, 12))
					)
				]),
				FechaRegistro2025.Crear(DateTime.Parse("2013/12/12")),
				false
			)
			.PrintAndContinue("Creando a neurologa-osteopata Marta Algerich: ")
			.GetOrRaise()
		});



	public static Task<List<Result<Paciente2025>>> GetPacientes()
		=> Task.FromResult(new List<Result<Paciente2025>> {
		Paciente2025.Crear(
				PacienteId.Crear(1),
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
				FechaRegistro2025.Crear(DateTime.Parse("2022/01/10"))
			).PrintAndContinue("Creando a Juan: "),

			Paciente2025.Crear(
				PacienteId.Crear(2),
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
				FechaRegistro2025.Crear(DateTime.Parse("2024/01/10"))
			).PrintAndContinue("Creando a Pedro: "),

			Paciente2025.Crear(
				PacienteId.Crear(3),
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
				FechaRegistro2025.Crear(DateTime.Parse("2023/01/10"))
			).PrintAndContinue("Creando a Herminda: "),

		});


	//public static Task<ServiciosPublicos> GetTurnos() {
		//return Task.FromResult(ServiciosPublicos._ValidarRepositorios());
	//}
}