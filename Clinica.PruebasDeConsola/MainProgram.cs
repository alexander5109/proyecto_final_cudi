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

		// 1. Crear fábrica de conexión
		IConfiguration config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.Development.json")
			.Build();

        SqlConnectionFactory factory = new(config.GetConnectionString("ClinicaMedica"));

        MedicoRepository medicoRepo = new(factory);
        TurnoRepository turnoRepo = new(factory);


        // 2. Crear el servicio de casos de uso
        ServicioTurnosManagerAsync servicio = new(medicoRepo, turnoRepo);

        // 3. Ejecutar casos de uso, NO cargar colecciones
        Result<Turno2025> turno = (await servicio.SolicitarTurnoEnLaPrimeraDisponibilidad(
			new PacienteId(1),
			EspecialidadMedica2025.ClinicoGeneral,
			DateTime.Now
		)).PrintAndContinue("Turno asignado:");

        Result<Turno2025> reprogramado = (await servicio.SolicitarReprogramacionALaPrimeraDisponibilidad(
			turno,
			DateTime.Now.AddDays(1),
			"Reprogramación"
		)).PrintAndContinue("Turno reprogramado:");

		Result<Turno2025> cancelado = (await servicio.SolicitarCancelacion(
			reprogramado,
			DateTime.Now.AddDays(2),
			"Cancelación definitiva"
		)).PrintAndContinue("Turno cancelado:");


	}

}
