using Clinica.Dominio.Entidades;
using Clinica.WebAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.DbModels;
namespace Clinica.WebAPI.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class TurnosController(
	IRepositorioTurnos repositorio, 
	ILogger<TurnosController> logger
) : ControllerBase {




	[HttpGet]
	public Task<IActionResult> GetTurnos()
	=> this.SafeExecute(
		logger,
		PermisoSistema.VerTurnos,
		() => repositorio.SelectTurnos()
	);



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetTurnoPorId(int id)
	=> this.SafeExecute(
		logger,
		PermisoSistema.VerTurnos,
		() => repositorio.SelectTurnoWhereId(new TurnoId(id)),
		notFoundMessage: $"No existe turno con id {id}"
	);



	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeleteTurno(int id)
	=> this.SafeExecute(
		logger,
		PermisoSistema.DeleteEntidades,
		() => repositorio.DeleteTurnoWhereId(new TurnoId(id)),
		notFoundMessage: $"No existe turno con id {id}"
	);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdateTurno(int id, [FromBody] TurnoDbModel dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		turno => repositorio.UpdateTurnoWhereId(turno),
		notFoundMessage: $"No existe turno con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearTurno([FromBody] TurnoDbModel dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.CrearTurnos,
		dto,
		x => x.ToDomain(),
		turno => repositorio.InsertTurnoReturnId(turno)
	);








}
