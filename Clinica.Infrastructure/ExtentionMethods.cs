using System.Data;
using Clinica.Dominio.Comun;
using Clinica.Infrastructure.DataAccess;
using Microsoft.Data.SqlClient;

namespace Clinica.Infrastructure;

public static class MappingExtensions {
	/// <summary>
	/// Convierte una secuencia de DB models en Result&lt;TDomain&gt; fila por fila.
	/// </summary>
	public static IEnumerable<Result<TDomain>> ToDomainList<TDbModel, TDomain>(
		this IEnumerable<TDbModel> source,
		Func<TDbModel, Result<TDomain>> map
	) {
		foreach (TDbModel? row in source)
			yield return map(row);
	}
}

public static class RepoTryExtensions {
	public static async Task<Result<T>> TryAsync<T>(
		this SQLServerConnectionFactory factory,
		Func<IDbConnection, Task<T>> action
	) {
		try {
			using IDbConnection conn = factory.CrearConexion();
			return new Result<T>.Ok(await action(conn));
		} catch (SqlException ex) {
			return new Result<T>.Error($"Error SQL: {ex.Message}");
		} catch (InvalidOperationException ex) {
			return new Result<T>.Error($"Error de conexión: {ex.Message}");
		} catch (Exception ex) {
			return new Result<T>.Error($"Error inesperado en infraestructura: {ex.Message}");
		}
	}
}