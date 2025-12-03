using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;

namespace Clinica.PruebasDeConsola;

public static class MainProgram {




    private static void CifrarContraseña() {
		string? rawPassword = null;
		while (string.IsNullOrWhiteSpace(rawPassword)) {
			Console.Write("Ingrese contraseña directamente: ");
			rawPassword = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(rawPassword)) Console.WriteLine("La contraseña no puede estar vacía.\n");
		}
		Console.WriteLine($"\nHashed:");
		Console.WriteLine($"Raw: {rawPassword}\nHashed: {ContraseñaHasheada.CrearFromRaw(rawPassword).Valor}");
		Console.WriteLine("\nPresione ENTER para continuar...");
		Console.ReadLine();
	}



    private static async Task Main() {

		//CifrarContraseña();

		Console.OutputEncoding = System.Text.Encoding.UTF8;



		IConfiguration config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.Development.json")
			.Build();

		//RepositorioDapper repositorio = new(new SQLServerConnectionFactory(config.GetConnectionString("ClinicaMedica")!));
		var repodonomio = new RepositorioDapper(new SQLServerConnectionFactory(config.GetConnectionString("ClinicaMedica")!))

		//var response = await http.GetAsync($"/disponibilidades?especialidadCodigoInterno=3&cuantos=10");

		NombreUsuario nombreUsuario = new ("admin1");
		var usuarioFakeResult = await repositorio.SelectUsuarioWhereNombre(nombreUsuario);
		if (usuarioFakeResult.IsError) {
			Console.WriteLine($"No se encontro el usuario {nombreUsuario}");
			return;
		}
        Usuario2025 usuarioFake = usuarioFakeResult.UnwrapAsOk();


		//CRUD TESTS
		//PacienteId pacienteId = new(1);
  //      Result<IEnumerable<Result<Turno2025>>> responseResult = await ServiciosPublicos.SelectTurnosWherePacienteId(usuarioFake, repositorio, pacienteId);
		//if (responseResult.IsError) {
		//	Console.WriteLine($"No se encontraron turnos para pacienteid {pacienteId}");
		//	return;
		//}
		//foreach (var turno2025 in responseResult.UnwrapAsOk()) {
		//	Console.Write(turno2025.UnwrapAsOk().ATexto());
		//	break;
		//}


		// Caso de uso 1
		Result<IReadOnlyList<DisponibilidadEspecialidad2025>> disponibilidades = (await ServiciosPublicos.SolicitarDisponibilidadesPara(
			EspecialidadMedica2025.ClinicoGeneral,
			DateTime.Now,
			4,
			repositorio
		));
		//disponibilidades.PrintAndContinue("Disponbiildiades encontradas::");
		IReadOnlyList<DisponibilidadEspecialidad2025> lista = disponibilidades.GetOrRaise();
		foreach (DisponibilidadEspecialidad2025 d in lista)
			Console.WriteLine(d.ATexto());

		// Caso de uso 2
		Result<Turno2025> turno = (await ServiciosPublicos.SolicitarTurnoEnLaPrimeraDisponibilidad(
			new PacienteId(1),
			EspecialidadMedica2025.ClinicoGeneral,
			new FechaRegistro2025(DateTime.Now),
			repositorio
		));
		turno.PrintAndContinue("Turno asignado:");

		// Caso de uso 3
		Result<Turno2025> reprogramado = (await ServiciosPublicos.SolicitarReprogramacionALaPrimeraDisponibilidad(
			turno,
			DateTime.Now.AddDays(1),
			"Reprogramación",
			repositorio
		));
		reprogramado.PrintAndContinue("Turno reprogramado:");

		// Caso de uso 4
		Result<Turno2025> cancelado = (await ServiciosPublicos.SolicitarCancelacion(
			reprogramado,
			DateTime.Now.AddDays(2),
			"Cancelación definitiva",
			repositorio
		));
		cancelado.PrintAndContinue("Turno cancelado:");

	}

}
