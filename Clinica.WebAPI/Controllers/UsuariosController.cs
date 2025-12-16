using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.IRepositorios;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsuariosController(
	IRepositorioUsuarios repositorio,
	ILogger<UsuariosController> logger
) : ControllerBase {
	[HttpGet]
	public Task<ActionResult<IEnumerable<UsuarioDbModel>>> GetUsuarios()
	=> this.SafeExecute(
		logger,
		AccionesDeUsuarioEnum.VerUsuarios,
		() => repositorio.SelectUsuarios()
	);



	[HttpGet("{id:int}")]
	public Task<ActionResult<UsuarioDbModel?>> GetUsuarioPorId(int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerUsuarios,
			() => repositorio.SelectUsuarioWhereId(UsuarioId.Crear(id)),
			notFoundMessage: $"No existe usuario con id {id}"
		);


	[HttpGet("por-nombre/{username}")]
	public Task<ActionResult<UsuarioDbModel>> GetUsuarioProfileByUsername(string username)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerUsuarios,
			() => repositorio.SelectUsuarioProfileWhereUsername(new UserName2025(username)),
			notFoundMessage: $"No existe usuario con nombre: {username}"
		);

	[HttpDelete("{id:int}")]
	public Task<ActionResult<Unit>> DeleteUsuario(int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.EliminarUsuarios,
			currentUserId => repositorio.DeleteUsuarioWhereId(UsuarioId.Crear(id)),
			precondicion: currentUserId => currentUserId.Valor != id,
			notFoundMessage: $"No existe usuario con id {id}"
		);




	[HttpPut("{id:int}")]
	public Task<ActionResult<UsuarioDbModel>> UpdateUsuario(int id, [FromBody] UsuarioEditarDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		AccionesDeUsuarioEnum.ModificarEntidades,
		dto,
		x => x.ToDomain(), ///here validation or insta return bad request + domain error. requires 
		usuario => repositorio.UpdateUsuarioWhereId(UsuarioId.Crear(id), usuario),
		notFoundMessage: $"No existe usuario con id {id}"
	);



	[HttpPost]
	public Task<ActionResult<UsuarioId>> CrearUsuario([FromBody] UsuarioCrearDto dto) {

		Console.WriteLine(dto.ToString());

		return this.SafeExecuteWithDomain(
				logger,
				AccionesDeUsuarioEnum.CrearUsuarios,
				dto,
				x => x.ToDomain(),
				usuario => repositorio.InsertUsuarioReturnId(usuario)
			);
	}


}