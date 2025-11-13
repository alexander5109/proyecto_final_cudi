using System.Data;
using Microsoft.Data.SqlClient;

namespace Clinica.DataPersistencia;

public class DbConnectionFactory {
	private readonly string _connectionString;

	public DbConnectionFactory(string connectionString) {
		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection() {
		var connection = new SqlConnection(_connectionString);
		connection.Open();
		return connection;
	}
}
