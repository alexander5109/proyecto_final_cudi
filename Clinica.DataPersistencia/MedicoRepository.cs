using System.Data;
using Dapper;

namespace Clinica.DataPersistencia;


public class MedicoRepository {
	private readonly IDbConnectionFactory _factory;

	public MedicoRepository(IDbConnectionFactory factory) {
		_factory = factory;
	}

	public async Task<IEnumerable<MedicoDto>> GetAll() {
		using var conn = _factory.CreateConnection();

		return await conn.QueryAsync<MedicoDto>(
			"sp_ReadMedicosAllWithHorarios",
			commandType: CommandType.StoredProcedure
		);
	}
}
