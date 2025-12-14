using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.IRepositorios;
using Dapper;
using static Clinica.Shared.ApiDtos.TurnoDtos;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.Repositorios;

public class RepositorioUsuarios(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioUsuarios {








	Task<Result<UsuarioDbModel?>> IRepositorioUsuarios.SelectUsuarioWhereId(UsuarioId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});




	Task<Result<UsuarioDbModel>> IRepositorioUsuarios.UpdateUsuarioWhereId(
		UsuarioId id,
		Usuario2025 instance
	)
		=> TryAsync(async conn => {
			// 1) Convertimos solo UNA VEZ
			UsuarioDbModel dto = instance.ToModel(id);

			// 2) Ejecutamos el SP que devuelve @@ROWCOUNT
			int rowsAffected = await conn.ExecuteScalarAsync<int>(
				"sp_UpdateUsuarioWhereId",
				dto,
				commandType: CommandType.StoredProcedure
			);

			// 3) Validamos que realmente se haya actualizado
			if (rowsAffected == 0)
				throw new Exception($"No se actualizó ningún usuario con Id={id.Valor}");

			// 4) Devolvemos el dto actualizado
			return dto;
		});




	Task<Result<Unit>> IRepositorioUsuarios.DeleteUsuarioWhereId(UsuarioId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteUsuarioWhereId",
				new { Id = id.Valor, },
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<IEnumerable<UsuarioDbModel>>> IRepositorioUsuarios.SelectUsuarios()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<UsuarioDbModel>(
				"sp_SelectUsuarios",
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<UsuarioDbModel>> IRepositorioUsuarios.SelectUsuarioProfileWhereUsername(UserName nombre)
		=> TryAsync(async conn => await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereNombre",
				new { UserName = nombre.Valor },
				commandType: CommandType.StoredProcedure
			) ?? throw new Exception($"Usuario con UserName={nombre.Valor} no encontrado.")
		);





	Task<Result<UsuarioId>> IRepositorioUsuarios.InsertUsuarioReturnId(Usuario2025 instance)
		=> TryAsync(async conn => await conn.ExecuteScalarAsync<int>(
			"sp_InsertUsuarioReturnId",
			instance.ToDto(),
			commandType: CommandType.StoredProcedure
		)).MapAsync(newId => new UsuarioId(newId));










}
