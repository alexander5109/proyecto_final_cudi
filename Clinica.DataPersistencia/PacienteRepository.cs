using System.Data;
using Dapper;

namespace Clinica.DataPersistencia;


public class PacienteRepository {
	private readonly IDbConnectionFactory _factory;

	public PacienteRepository(IDbConnectionFactory factory) {
		_factory = factory;
	}

	public async Task<IEnumerable<PacienteDto>> GetAllPacientes() {
		using var conn = _factory.CreateConnection();

		return await conn.QueryAsync<PacienteDto>(
			"sp_ReadPacientesAll",
			commandType: CommandType.StoredProcedure
		);
	}

	public async Task<PacienteDto?> GetById(int id) {
		using var conn = _factory.CreateConnection();

		return await conn.QueryFirstOrDefaultAsync<PacienteDto>(
			"sp_ReadPacienteById",
			new { Id = id },
			commandType: CommandType.StoredProcedure
		);
	}
}
