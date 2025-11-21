using System.Configuration;
using Clinica.DataPersistencia;

namespace Clinica.PruebasDeConsola;

public class ScenarioTestingDatabase {
	//[Fact]
	public static void ProbarDataPersistencia() {

		string connectionString =
		ConfigurationManager.ConnectionStrings["ClinicaDB"].ConnectionString;

		IDbConnectionFactory factory = new SqlConnectionFactory(connectionString);

        PacienteRepository repo = new PacienteRepository(factory);

		// ejemplo: leer pacientes
		var pacientes = await repo.GetAllPacientes();



	}
}
