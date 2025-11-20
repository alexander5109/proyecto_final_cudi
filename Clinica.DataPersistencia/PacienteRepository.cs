using System.Data;
using Dapper;


public class PacienteRepository {
	private readonly IDbConnectionFactory _factory;

	public PacienteRepository(IDbConnectionFactory factory) {
		_factory = factory;
	}

	public IEnumerable<PacienteDto> GetAll() {
		using var conn = _factory.CreateConnection();
		return conn.Query<PacienteDto>("sp_Pacientes_GetAll",
			commandType: CommandType.StoredProcedure);
	}
}
