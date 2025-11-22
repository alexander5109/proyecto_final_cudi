using System.Data;
using Dapper;

namespace Clinica.DataPersistencia;

public class TurnoRepository {
	private readonly IDbConnectionFactory _factory;

	public TurnoRepository(IDbConnectionFactory factory) {
		_factory = factory;
	}

	public async Task<IEnumerable<TurnoDto>> GetAll() {
		using var conn = _factory.CreateConnection();

		return await conn.QueryAsync<TurnoDto>(
			"sp_ReadTurnosAll",
			commandType: CommandType.StoredProcedure
		);
	}

}
