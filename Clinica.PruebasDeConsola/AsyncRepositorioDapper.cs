using Clinica.DataPersistencia;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Microsoft.Extensions.Configuration;

namespace Clinica.PruebasDeConsola;

public class AsyncRepositorioDapper {
	public static async Task<List<Result<Paciente2025>>> GetPacientes() {
		IConfiguration config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.Development.json", optional: false)
			.Build();

		IDbConnectionFactory factory = new SqlConnectionFactory(config.GetConnectionString("ClinicaMedica"));

		PacienteRepository repo = new(factory);

		IEnumerable<PacienteDto> pacientes = await repo.GetAll();

		// Convertimos cada DTO a dominio
		return pacientes
			.Select(
			x => x.ToDomain()
			.PrintAndContinue("Paciente domainizado")
		).ToList();
	}
	public static async Task<List<Result<Medico2025>>> GetMedicos() {
		IConfiguration config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.Development.json", optional: false)
			.Build();

		IDbConnectionFactory factory = new SqlConnectionFactory(config.GetConnectionString("ClinicaMedica"));

		MedicoRepository repo = new(factory);

		IEnumerable<MedicoDto> medicos = await repo.GetAll();

		return medicos
			.Select(
			x => x.ToDomain()
			.PrintAndContinue("Medico domainizado")
			//.GetOrRaise()
		).ToList();
		//);
	}
	public static async Task<ListaTurnosHistorial2025> GetTurnos() {
		throw new NotImplementedException();

		//IConfiguration config = new ConfigurationBuilder()
		//    .SetBasePath(AppContext.BaseDirectory)
		//    .AddJsonFile("appsettings.Development.json", optional: false)
		//    .Build();

		//IDbConnectionFactory factory = new SqlConnectionFactory(config.GetConnectionString("ClinicaMedica"));

		//TurnoRepository repo = new(factory);

		//IEnumerable<TurnoDto> medicos = await repo.GetAllTurnos();

		//List<Result<Turno2025>> resultados = medicos.Select(x => x.ToDomain().PrintAndContinue("Turno domainizado")).ToList();
		//return resultados;
	}

}
