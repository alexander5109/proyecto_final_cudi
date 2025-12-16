using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.Servicios;
using Microsoft.Extensions.Configuration;
using Clinica.Dominio.IInterfaces;
using Clinica.Infrastructure.Repositorios;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;

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
		Console.WriteLine($"Raw: {rawPassword}\nHashed: {ContraseñaHasheada2025.CrearFromRaw(rawPassword).Valor}");
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
		IRepositorioDominioServices repositorio = new RepositorioDominioServices(new SQLServerConnectionFactory(config.GetConnectionString("ClinicaMedica")!));

		//var response = await http.GetAsync($"/disponibilidades?EspecialidadEnum=3&cuantos=10");

		UserName2025 UserName = new ("admin1");
		//var usuarioFakeResult = await repositorio.SelectUsuarioWhereNombre(UserName2025);
		//if (usuarioFakeResult.IsError) {
		//	Console.WriteLine($"No se encontro el usuario {UserName2025}");
		//	return;
		//}
        //Usuario2025 usuarioFake = usuarioFakeResult.UnwrapAsOk();


		//CRUD TESTS
		//PacienteId pacienteId = new(1);
  //      Result<IEnumerable<Result<Turno2025>>> responseResult = await ServiciosPublicos.SelectTurnosWherePacienteId(usuarioFake, repositorio, pacienteId);
		//if (responseResult.IsError) {
		//	Console.WriteLine($"No se encontraron turnos para pacienteid {pacienteId}");
		//	return;
		//}
		//foreach (var turno2025 in responseResult.UnwrapAsOk()) {
		//	Console.Write(turno2025.UnwrapAsOk().ATextoDia());
		//	break;
		//}

		IServiciosDeDominio servicios = new ServiciosPublicos();
		// Caso de uso 1
		Result<IReadOnlyList<Disponibilidad2025>> disponibilidades = (await servicios.SolicitarDisponibilidades(
			Especialidad2025.ClinicoGeneral.Codigo,
			DateTime.Now,
			4,
			DayOfWeek.Sunday,
			new MedicoId(1),
			repositorio
		));
		//disponibilidades.PrintAndContinue("Disponbiildiades encontradas::");
		IReadOnlyList<Disponibilidad2025> lista = disponibilidades.GetOrRaise();
		foreach (Disponibilidad2025 d in lista)
			Console.WriteLine(d.ToString());
			//Console.WriteLine(d.ATexto());

		// Caso de uso 2
		//Result<Turno2025> turno = (await servicios.SolicitarTurnoEnLaPrimeraDisponibilidad(
		//	new PacienteId(1),
		//	Especialidad2025.ClinicoGeneral.Codigo,
		//	DateTime.Now,
		//	repositorio
		//));
		//turno.PrintAndContinue("Turno asignado:");

		// Caso de uso 3
		//Result<Turno2025> reprogramado = (await servicios.SolicitarReprogramacionALaPrimeraDisponibilidad(
		//	turno.UnwrapAsOk().Id,
		//	DateTime.Now.AddDays(1),
		//	"Reprogramación",
		//	repositorio
		//));
		//reprogramado.PrintAndContinue("Turno reprogramado:");

		// Caso de uso 4
		//Result<Turno2025> cancelado = (await servicios.SolicitarCancelacion(
		//	reprogramado.UnwrapAsOk().Id,
		//	DateTime.Now.AddDays(2),
		//	"Cancelación definitiva",
		//	repositorio
		//));
		//cancelado.PrintAndContinue("Turno cancelado:");

	}

}
