using System.Data;
using Dapper;

namespace Clinica.DataPersistencia;


public class PacienteRepository {
	private readonly IDbConnectionFactory _factory;

	public PacienteRepository(IDbConnectionFactory factory) {
		_factory = factory;
	}

	public async Task<IEnumerable<PacienteDto>> GetAll() {
		using var conn = _factory.CreateConnection();

		return await conn.QueryAsync<PacienteDto>(
			"sp_ReadPacientesAll",
			commandType: CommandType.StoredProcedure
		);
	}
}
