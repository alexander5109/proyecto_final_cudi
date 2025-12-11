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
		PermisosAccionesCodigo.VerUsuarios,
		() => repositorio.SelectUsuarios()
	);



	[HttpGet("{id:int}")]
	public Task<ActionResult<UsuarioDbModel?>> GetUsuarioPorId(int id)
		=> this.SafeExecute(
			logger,
			PermisosAccionesCodigo.VerUsuarios,
			() => repositorio.SelectUsuarioWhereId(new UsuarioId(id)),
			notFoundMessage: $"No existe usuario con id {id}"
		);


	[HttpGet("por-nombre/{username}")]
	public Task<ActionResult<UsuarioDbModel>> GetUsuarioProfileByUsername(string username)
		=> this.SafeExecute(
			logger,
			PermisosAccionesCodigo.VerUsuarios,
			() => repositorio.SelectUsuarioProfileWhereUsername(new UserName(username)),
			notFoundMessage: $"No existe usuario con nombre: {username}"
		);


	[HttpDelete("{id:int}")]
	public Task<ActionResult<Unit>> DeleteUsuario(int id)
		=> this.SafeExecute(
			logger,
			PermisosAccionesCodigo.DeleteEntidades,
			() => repositorio.DeleteUsuarioWhereId(new UsuarioId(id)),
			notFoundMessage: $"No existe usuario con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<ActionResult<UsuarioDbModel>> UpdateUsuario(int id, [FromBody] UsuarioDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisosAccionesCodigo.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		usuario => repositorio.UpdateUsuarioWhereId(new UsuarioId(id), usuario),
		notFoundMessage: $"No existe usuario con id {id}"
	);



	[HttpPost]
	public Task<ActionResult<UsuarioId>> CrearUsuario([FromBody] UsuarioDto dto)
		=> this.SafeExecuteWithDomain(
			logger,
			PermisosAccionesCodigo.CrearUsuarios,
			dto,
			x => x.ToDomain(),
			usuario => repositorio.InsertUsuarioReturnId(usuario)
		);



}