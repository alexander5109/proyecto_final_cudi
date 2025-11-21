using Clinica.DataPersistencia;
using Microsoft.Extensions.Configuration;

namespace Clinica.PruebasDeConsola;

public static class ScenarioTestingDatabase {
	//[Fact]
	public static async Task ProbarDataPersistenciaAsync() {
		IConfiguration config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.Development.json", optional: false)
			.Build();


		IDbConnectionFactory factory = new SqlConnectionFactory(config.GetConnectionString("ClinicaMedica"));

        PacienteRepository repo = new(factory);

        // ejemplo: leer pacientes
        IEnumerable<PacienteDto> pacientes = await repo.GetAllPacientes();

		foreach (PacienteDto paciente in pacientes) {
			Console.WriteLine(paciente);
		}

	}
}
