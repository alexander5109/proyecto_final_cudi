using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.DbModels;

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
		PermisoSistema.VerUsuarios,
		() => repositorio.SelectUsuarios()
	);



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetUsuarioPorId(int id)
		=> this.SafeExecute(
			PermisoSistema.VerUsuarios,
			() => repositorio.SelectUsuarioWhereId(new UsuarioId(id)),
			notFoundMessage: $"No existe usuario con id {id}"
		);


	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeleteUsuario(int id)
		=> this.SafeExecute(
			PermisoSistema.DeleteEntidades,
			() => repositorio.DeleteUsuarioWhereId(new UsuarioId(id)),
			notFoundMessage: $"No existe usuario con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioDbModel dto)
	=> this.SafeExecuteWithDomain(
		PermisoSistema.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		usuario => repositorio.UpdateUsuarioWhereId(usuario),
		notFoundMessage: $"No existe usuario con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearUsuario([FromBody] UsuarioDbModel dto)
	=> this.SafeExecuteWithDomain(
		PermisoSistema.CrearUsuarios,
		dto,
		x => x.ToDomain(),
		usuario => repositorio.InsertUsuarioReturnId(usuario)
	);



}