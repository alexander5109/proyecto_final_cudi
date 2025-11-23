using System.Data;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Dapper;

namespace Clinica.Infrastructure.Persistencia;

public class TurnoRepository(IDbConnectionFactory factory) {
	private readonly IDbConnectionFactory _factory = factory;

    public async Task<IEnumerable<TurnoDto>> GetAll() {
		using IDbConnection conn = _factory.CreateConnection();

		return await conn.QueryAsync<TurnoDto>(
			"sp_ReadTurnosAll",
			commandType: CommandType.StoredProcedure
		);
	}

	public async Task<IEnumerable<TurnoDto>> GetTurnosPorMedicos(IEnumerable<int> medicosIds) {
		using IDbConnection conn = _factory.CreateConnection();

		return await conn.QueryAsync<TurnoDto>(
			"sp_ReadTurnosAll",
			commandType: CommandType.StoredProcedure
		);
	}

    internal async Task Insert(Turno2025 turno2025) {
        throw new NotImplementedException();
    }
}
