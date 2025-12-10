using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.ApiDtos.TurnoDtos;
namespace Clinica.WebAPI.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
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
	public Task<IActionResult> UpdateTurno(int id, [FromBody] TurnoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		turno => repositorio.UpdateTurnoWhereId(new TurnoId(id), turno),
		notFoundMessage: $"No existe turno con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearTurno([FromBody] TurnoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.CrearTurnos,
		dto,
		x => x.ToDomain(),
		turno => repositorio.InsertTurnoReturnId(turno)
	);

	//[HttpGet]
	//public Task<IActionResult> GetTurnosWithPacienteMedicoNames()
	//=> this.SafeExecute(
	//	logger,
	//	PermisoSistema.VerTurnos,
	//	x => x.ToDto(),
	//	() => repositorio.SelectTurnos()
	//);








}
