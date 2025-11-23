using System.Data;
using Dapper;

namespace Clinica.Infrastructure.Persistencia;


public class PacienteRepository {
	private readonly IDbConnectionFactory _factory;

	public PacienteRepository(IDbConnectionFactory factory) {
		_factory = factory;
	}

	public async Task<IEnumerable<PacienteDto>> GetAll() {
		using IDbConnection conn = _factory.CreateConnection();

		return await conn.QueryAsync<PacienteDto>(
			"sp_ReadPacientesAll",
			commandType: CommandType.StoredProcedure
		);
	}
}
