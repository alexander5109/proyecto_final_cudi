using System.Collections.Generic;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.Persistencia;
using Clinica.Infrastructure.ServiciosAsync;
using Microsoft.Extensions.Configuration;

namespace Clinica.PruebasDeConsola;

public static class MainProgram {
	static async Task Main() {
		Console.OutputEncoding = System.Text.Encoding.UTF8;

		// 1. CrearFromStrings fábrica de conexión
		IConfiguration config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.Development.json")
			.Build();

		// 2. CrearFromStrings el servicio de casos de uso
		ServiciosPublicosAsync servicio = new(
			new BaseDeDatosRepositorio(
				new SqlConnectionFactory(config.GetConnectionString("ClinicaMedica")
				)
			)
		);


		// Caso de uso 1
		Result<IReadOnlyList<DisponibilidadEspecialidad2025>> disponibilidades = (await servicio.SolicitarDisponibilidadesPara(
			EspecialidadMedica2025.ClinicoGeneral,
			DateTime.Now,
			15
		)).PrintAndContinue("Disponbiildiades encontradas::");
		//var lista = disponibilidades.GetOrRaise();
		//foreach (var d in lista)
		//	Console.WriteLine(d.ATexto());

		// Caso de uso 2
		Result<Turno2025> turno = (await servicio.SolicitarTurnoEnLaPrimeraDisponibilidad(
			new PacienteId(1),
			EspecialidadMedica2025.ClinicoGeneral,
			new FechaRegistro2025(DateTime.Now)
		)).PrintAndContinue("Turno asignado:");

		// Caso de uso 3
		Result<Turno2025> reprogramado = (await servicio.SolicitarReprogramacionALaPrimeraDisponibilidad(
			turno,
			DateTime.Now.AddDays(1),
			"Reprogramación"
		)).PrintAndContinue("Turno reprogramado:");

		// Caso de uso 4
		Result<Turno2025> cancelado = (await servicio.SolicitarCancelacion(
			reprogramado,
			DateTime.Now.AddDays(2),
			"Cancelación definitiva"
		)).PrintAndContinue("Turno cancelado:");


	}

}
