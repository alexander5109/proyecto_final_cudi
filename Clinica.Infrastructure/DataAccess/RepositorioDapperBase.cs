using System.Data;
using Clinica.Dominio.Comun;
using Microsoft.Data.SqlClient;

namespace Clinica.Infrastructure.DataAccess;

public class RepositorioDapperBase(SQLServerConnectionFactory factory) {
    protected async Task<Result<T>> TryAsync<T>(Func<IDbConnection, Task<T>> action) {
        try {
            using IDbConnection conn = factory.CrearConexion();
            T? result = await action(conn);
            return new Result<T>.Ok(result);
        } catch (SqlException ex) {
            return new Result<T>.Error($"Error SQL: {ex.Message}");
        } catch (Exception ex) {
            return new Result<T>.Error($"Error inesperado: {ex.Message}");
        }
    }

	protected async Task<Result<Unit>> TryAsyncVoid(Func<IDbConnection, Task> action) {
        try {
            using IDbConnection conn = factory.CrearConexion();
            await action(conn);
            return new Result<Unit>.Ok(Unit.Valor);
        } catch (SqlException ex) {
            return new Result<Unit>.Error($"Error SQL: {ex.Message}");
        } catch (Exception ex) {
            return new Result<Unit>.Error($"Error inesperado: {ex.Message}");
        }
    }
}