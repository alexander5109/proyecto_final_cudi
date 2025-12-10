using Clinica.Dominio.TiposDeValor;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
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
	public Task<IActionResult> GetUsuarios()
	=> this.SafeExecute(
		logger,
		PermisoSistema.VerUsuarios,
		() => repositorio.SelectUsuarios()
	);



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetUsuarioPorId(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerUsuarios,
			() => repositorio.SelectUsuarioWhereId(new UsuarioId(id)),
			notFoundMessage: $"No existe usuario con id {id}"
		);


	[HttpGet("{id:string}")]
	public Task<IActionResult> GetUsuarioProfileByUsername(string username)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerUsuarios,
			() => repositorio.SelectUsuarioProfileWhereUsername(new UserName(username)),
			notFoundMessage: $"No existe usuario con nombre: {username}"
		);


	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeleteUsuario(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.DeleteEntidades,
			() => repositorio.DeleteUsuarioWhereId(new UsuarioId(id)),
			notFoundMessage: $"No existe usuario con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioDbModel dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		usuario => repositorio.UpdateUsuarioWhereId(new UsuarioId(id), usuario),
		notFoundMessage: $"No existe usuario con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearUsuario([FromBody] UsuarioDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.CrearUsuarios,
		dto,
		x => x.ToDomain(),
		usuario => repositorio.InsertUsuarioReturnId(usuario)
	);



}