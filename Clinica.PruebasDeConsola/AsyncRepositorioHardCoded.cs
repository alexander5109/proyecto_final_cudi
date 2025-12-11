using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.PruebasDeConsola;

public static class AsyncRepositorioHardCoded {
	public static Task<List<Medico2025>> GetMedicos()
		=> Task.FromResult(new List<Medico2025> {
		Medico2025.CrearResult(
				//MedicoId.CrearResult(1),
				NombreCompleto2025.CrearResult("Carlos Alfredo", "Markier"),
				Especialidad2025.CrearResult(Especialidad2025.Gastroenterologo.Codigo),
				//ListaEspecialidadesMedicas2025.CrearResult([
				//	Especialidad2025.Gastroenterologo,
				//	Especialidad2025.ClinicoGeneral
				//]),
				DniArgentino2025.CrearResult("15350996"),
				DomicilioArgentino2025.CrearResult(
					LocalidadDeProvincia2025.CrearResult(
						"Morón",
						ProvinciaArgentina2025.CrearResult("Buenos Aires")
					),
					"Avenida Rivadavia 2323"
				),
				Telefono2025.CrearResult("1133832021"),
				Email2025.CrearResult("carlosmerkeir@gmail.com"),
				ListaHorarioMedicos2025.CrearResult([
					Horario2025.Crear(
						new MedicoId(2341456),
						DayOfWeek.Monday,
						new TimeOnly(09,00),
						new TimeOnly(15,00),
						new DateOnly(2024, 1, 1),
						new DateOnly(2026, 1, 1)
					),
					Horario2025.Crear(
						new MedicoId(2311056),
						DayOfWeek.Wednesday,
						new TimeOnly(09,00),
						new TimeOnly(15,00),
						new DateOnly(2024, 1, 1),
						new DateOnly(2026, 1, 1)
					)
				]),
				new DateTime(2014,12,12),
				false
			)
			.PrintAndContinue("Creando a clinico general-gastroenterologo Carlos Merkier: ")
			.GetOrRaise()
			,

			Medico2025.CrearResult(
				//MedicoId.CrearResult(2),
				NombreCompleto2025.CrearResult("Jorge", "Pereyra"),
				Especialidad2025.CrearResult(Especialidad2025.Ginecologo.Codigo),
				//ListaEspecialidadesMedicas2025.CrearResult([
				//	Especialidad2025.Pediatra,
				//	Especialidad2025.Ginecologo
				//]),
				DniArgentino2025.CrearResult("20350996"),
				DomicilioArgentino2025.CrearResult(
					LocalidadDeProvincia2025.CrearResult(
						"Gregorio de Laferrere",
						ProvinciaArgentina2025.CrearResult("Buenos Aires")
					),
					"Armonia 23231"
				),
				Telefono2025.CrearResult("1163632071"),
				Email2025.CrearResult("jorgepereiyra@gmail.com"),
				ListaHorarioMedicos2025.CrearResult([
					Horario2025.Crear(
						new MedicoId(231156),
						DayOfWeek.Tuesday,
						new TimeOnly(08, 0),
						new TimeOnly(18, 00),
						new DateOnly(2024, 1, 12),
						new DateOnly(2026, 1, 12)
					),
					Horario2025.Crear(
						new MedicoId(23156),
						DayOfWeek.Thursday,
						new TimeOnly(08,00),
						new TimeOnly(18,0),
						new DateOnly(2024, 1, 12),
						new DateOnly(2026, 1, 12)
					)
				]),
				new DateTime(2014, 12, 12),
				false
			)
			.PrintAndContinue("Creando a pediatra-ginecologo Jorge Pereyra: ")
			.GetOrRaise()
			,

			Medico2025.CrearResult(
				NombreCompleto2025.CrearResult("Marta", "Algerich"),
				Especialidad2025.CrearResult(Especialidad2025.Neurologo.Codigo),
				//ListaEspecialidadesMedicas2025.CrearResult([
					//new Result.Ok<Especialidad2025>(Especialidad2025.Neurologo),
					//Especialidad2025.Osteopata
				//]),
				DniArgentino2025.CrearResult("10350996"),
				DomicilioArgentino2025.CrearResult(
					LocalidadDeProvincia2025.CrearResult(
						"Palermo",
						ProvinciaArgentina2025.CrearResult("Buenos Aires")
					),
					"Entre Rios 123"
				),
				Telefono2025.CrearResult("1149920537"),
				Email2025.CrearResult("martaalgerich@gmail.com"),
				ListaHorarioMedicos2025.CrearResult([
					Horario2025.Crear(
						new MedicoId(156),
						DayOfWeek.Monday,
						new TimeOnly(08,00),
						new TimeOnly(12,00),
						new DateOnly(2022, 1, 12),
						new DateOnly(2024, 1, 12)
					),
					Horario2025.Crear(
						new MedicoId(1256),
						DayOfWeek.Wednesday,
						new TimeOnly(08,00),
						new TimeOnly(12,00),
						new DateOnly(2022, 1, 12),
						new DateOnly(2024, 1, 12)
					),
					Horario2025.Crear(
						new MedicoId(12756),
						DayOfWeek.Friday,
						new TimeOnly(08,00),
						new TimeOnly(12,00),
						new DateOnly(2022, 1, 12),
						new DateOnly(2024, 1, 12)
					)
				]),
				new DateTime(2013, 12,12),
				false
			)
			.PrintAndContinue("Creando a neurologa-osteopata Marta Algerich: ")
			.GetOrRaise()
		});



	public static Task<List<Result<Paciente2025Agg>>> GetPacientes()
		=> Task.FromResult(new List<Result<Paciente2025Agg>> {
		Paciente2025Agg.CrearResult(
			PacienteId.CrearResult(1),
			Paciente2025.CrearResult(
				NombreCompleto2025.CrearResult("Juan", "Diaz"),
				DniArgentino2025.CrearResult("44444444"),
				Telefono2025.CrearResult("1155544433"),
				Email2025.CrearResult("juandiaz@gmail.com"),
				DomicilioArgentino2025.CrearResult(
					LocalidadDeProvincia2025.CrearResult(
						"Lanus",
						ProvinciaArgentina2025.CrearResult("Buenos Aires")
					),
					"Calle Falsa 123"
				),
				FechaDeNacimiento2025.CrearResult(DateTime.Parse("1990/05/15")),
				new DateTime(2022,11,10)
			).PrintAndContinue("Creando a Juan: ")),

		Paciente2025Agg.CrearResult(
			PacienteId.CrearResult(1),
			Paciente2025.CrearResult(
				NombreCompleto2025.CrearResult("Pedro", "Fernandez"),
				DniArgentino2025.CrearResult("30350123"),
					Telefono2025.CrearResult("11655414253"),
					Email2025.CrearResult("pedroFalopa@gmail.com"),
				DomicilioArgentino2025.CrearResult(
					LocalidadDeProvincia2025.CrearResult(
						"Castillo",
						ProvinciaArgentina2025.CrearResult("Buenos Aires")
					),
					"Palmerita 12223"
				),
				FechaDeNacimiento2025.CrearResult(DateTime.Parse("1996/05/15")),
				new DateTime(2024, 1, 10)
			).PrintAndContinue("Creando a Pedro: ")),

			Paciente2025Agg.CrearResult(
				PacienteId.CrearResult(1),
				Paciente2025.CrearResult(
					NombreCompleto2025.CrearResult("Herminda", "Gutierrez Lopez"),
					DniArgentino2025.CrearResult("44444444"),
						Telefono2025.CrearResult("11225411453"),
						Email2025.CrearResult("hermindalaturri201@gmail.com"),
					DomicilioArgentino2025.CrearResult(
						LocalidadDeProvincia2025.CrearResult(
							"Virrey del Pinos",
							ProvinciaArgentina2025.CrearResult("Buenos Aires")
						),
						"Urien 2223"
					),
					FechaDeNacimiento2025.CrearResult(DateTime.Parse("1994/05/15")),
					new DateTime(2023,1,10))
				).PrintAndContinue("Creando a Herminda: ")

		});


	//public static Task<ServiciosPublicos> GetTurnos() {
	//return Task.FromResult(ServiciosPublicos._ValidarRepositorios());
	//}
}