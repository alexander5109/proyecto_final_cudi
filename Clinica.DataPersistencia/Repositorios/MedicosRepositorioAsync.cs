using Dapper;
using Clinica.DataPersistencia.ModelDtos;
using System.ComponentModel;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.DataPersistencia.Repositorios;

public class MedicosRepositorioAsync(DbConnectionFactory connectionFactory) {
	private readonly DbConnectionFactory _connectionFactory = connectionFactory;

	// ------------------- CREATE -------------------
	public async Task<Result<string>> InsertAsync(Medico2025 medico) {
		throw new NotImplementedException();
	}

	// ------------------- READ -------------------
	public async Task<Result<Medico2025>> GetByIdAsync(string id) {
		throw new NotImplementedException();
	}

	// ------------------- UPDATE -------------------
	public async Task<Result> UpdateAsync(string id, Medico2025 medico) {
		throw new NotImplementedException();
	}

	// ------------------- DELETE -------------------
	public async Task<Result> DeleteAsync(string id) {
		throw new NotImplementedException();
	}
}
