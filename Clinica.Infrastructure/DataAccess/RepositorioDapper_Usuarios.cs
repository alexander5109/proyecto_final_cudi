using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Dapper;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.Infrastructure.DataAccess;



public partial class RepositorioDapper {


	public Task<Result<Usuario2025>> SelectUsuarioWhereId(UsuarioId id)
		=> TryAsync(async conn => {
			UsuarioDbModel? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);

			if (dto is null)
				throw new Exception($"Usuario con Id={id.Valor} no encontrado.");

			Result<Usuario2025> map = dto.ToDomain();
			if (map.IsError)
				throw new Exception($"Error creando Usuario2025 desde DTO (Id={id.Valor}): {map.UnwrapAsError()}");

			return map.UnwrapAsOk();
		});



	public async Task<Result<UsuarioId>> InsertUsuarioReturnId(Usuario2025 usuario)
		=> await TryAsync(async conn => {
			DynamicParameters parameters = new();
			parameters.Add("@NombreUsuario", usuario.UserName.Valor);
			parameters.Add("@PasswordHash", usuario.UserPassword.Valor);
			parameters.Add("@EnumRole", usuario.EnumRole);
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
			await conn.ExecuteAsync(
				"sp_InsertUsuarioReturnId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
			int newId = parameters.Get<int>("@NewId");
			return new UsuarioId(newId);   // ← solo devolvés el valor
		});
	public Task<Result<Unit>> DeleteUsuarioWhereId(UsuarioId id)
		=> TryAsyncVoid(async conn => {
			var parameters = new {
				Id = id.Valor,
			};
			await conn.ExecuteAsync(
				"sp_DeleteUsuarioWhereId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
		});


}