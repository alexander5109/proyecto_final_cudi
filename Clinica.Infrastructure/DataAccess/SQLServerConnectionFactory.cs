using System.Data;
using Clinica.Infrastructure.TypeHandlers;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Clinica.Infrastructure.DataAccess;

public class SQLServerConnectionFactory(string connectionString) {
	public IDbConnection CrearConexion() {

		SqlMapper.AddTypeHandler(new TurnoIdHandler());
		SqlMapper.AddTypeHandler(new PacienteIdHandler());
		SqlMapper.AddTypeHandler(new MedicoIdHandler());
		SqlMapper.AddTypeHandler(new UsuarioIdHandler());

		return new SqlConnection(connectionString);
	}
}
