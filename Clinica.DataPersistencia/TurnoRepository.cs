using System.Data;
using Dapper;

namespace Clinica.DataPersistencia;

public class TurnoRepository {
	/*
	private readonly IDbConnectionFactory _factory;

	public TurnoRepository(IDbConnectionFactory factory) {
		_factory = factory;
	}

	public IEnumerable<TurnoDto> GetAll() {
		using var conn = _factory.CreateConnection();
		return conn.Query<TurnoDto>("sp_Turnos_GetAll",
			commandType: CommandType.StoredProcedure);
	}

	public int Insert(int pacienteId, int medicoId, DateTime fecha) {
		using var conn = _factory.CreateConnection();

		return conn.ExecuteScalar<int>("sp_Turnos_Insert",
			new { PacienteId = pacienteId, MedicoId = medicoId, Fecha = fecha },
			commandType: CommandType.StoredProcedure);
	}

	*/
}
