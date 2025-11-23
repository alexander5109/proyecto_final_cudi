using System.Data;
using Clinica.Dominio.TiposDeValor;
using Dapper;

namespace Clinica.Infrastructure.Persistencia;


public class MedicoRepository {
	private readonly IDbConnectionFactory _factory;

	public MedicoRepository(IDbConnectionFactory factory) {
		_factory = factory;
	}

	public async Task<IEnumerable<MedicoDto>> ReadMedicosFull() {
		using IDbConnection conn = _factory.CreateConnection();

		return await conn.QueryAsync<MedicoDto>(
			"sp_ReadMedicosFull",
			commandType: CommandType.StoredProcedure
		);
	}

	public async Task<IEnumerable<MedicoDto>> ReadMedicosFullWhereEspecialidad(EspecialidadMedica2025 especialidad) {
		using IDbConnection conn = _factory.CreateConnection();

		return await conn.QueryAsync<MedicoDto>(
			"sp_ReadMedicosFullWhereEspecialidad",
			commandType: CommandType.StoredProcedure
		);
	}
}
