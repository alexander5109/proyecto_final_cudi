using System.Data;
using Microsoft.Data.SqlClient;


namespace Clinica.Infrastructure.Persistencia;

public class SqlConnectionFactory {
	private readonly string _connectionString;

	public SqlConnectionFactory(string connectionString) {
		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection() {
		return new SqlConnection(_connectionString);
	}
}
