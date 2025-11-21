
namespace Clinica.PruebasDeConsola;

internal class Program {
	static async Task Main() {
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		await ScenarioTestingDatabase.ProbarDataPersistenciaAsync();
		//ScenarioTestingHardCoded.TestDominio();

	}

}
