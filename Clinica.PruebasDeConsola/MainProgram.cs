using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.DataAccess;
using Clinica.Infrastructure.Servicios;
using Clinica.Infrastructure.ServiciosAsync;
using Microsoft.Extensions.Configuration;

namespace Clinica.PruebasDeConsola;

public static class MainProgram {




	static void CifrarContraseña() {
		string? rawPassword = null;
		while (string.IsNullOrWhiteSpace(rawPassword)) {
			Console.Write("Ingrese contraseña directamente: ");
			rawPassword = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(rawPassword)) Console.WriteLine("La contraseña no puede estar vacía.\n");
		}
		Console.WriteLine($"\nHashed:");
		Console.WriteLine(AuthService.ComputeSha256(rawPassword));

		Console.WriteLine("\nPresione ENTER para continuar...");
		Console.ReadLine();
	}



	static async Task Main() {

		CifrarContraseña();

		Console.OutputEncoding = System.Text.Encoding.UTF8;

		IConfiguration config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.Development.json")
			.Build();

		ServiciosPublicosAsync servicio = new(
			new BaseDeDatosRepositorio(
				new SQLServerConnectionFactory(config.GetConnectionString("ClinicaMedica")
				)
			)
		);

		//var response = await http.GetAsync($"/disponibilidades?especialidadCodigoInterno=3&cuantos=10");


		// Caso de uso 1
		Result<IReadOnlyList<DisponibilidadEspecialidad2025>> disponibilidades = (await servicio.SolicitarDisponibilidadesPara(
			EspecialidadMedica2025.ClinicoGeneral,
			DateTime.Now,
			4
		)).PrintAndContinue("Disponbiildiades encontradas::");
		var lista = disponibilidades.GetOrRaise();
		foreach (var d in lista)
			Console.WriteLine(d.ATexto());

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
