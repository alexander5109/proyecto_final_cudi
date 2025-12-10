using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;
using Clinica.Infrastructure.Repositorios;
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
		PermisosAccionesCodigo.VerTurnos,
		() => repositorio.SelectTurnos()
	);




	[HttpGet("{id:int}")]
	public Task<IActionResult> GetTurnoPorId(int id)
	=> this.SafeExecute(
		logger,
		PermisosAccionesCodigo.VerTurnos,
		() => repositorio.SelectTurnoWhereId(new TurnoId(id)),
		notFoundMessage: $"No existe turno con id {id}"
	);

	[HttpGet("medico/{id}")]
	public Task<IActionResult> GetTurnosPorMedico([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			PermisosAccionesCodigo.VerTurnos,
			() => repositorio.SelectTurnosWhereMedicoId(new MedicoId(id)),
			notFoundMessage: $"No existen turnos con medicoid {id}"
		);

	[HttpGet("paciente/{id}")]
	public Task<IActionResult> GetTurnosPorPaciente([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			PermisosAccionesCodigo.VerTurnos,
			() => repositorio.SelectTurnosWherePacienteId(new PacienteId(id)),
			notFoundMessage: $"No existen turnos con PacienteId {id}"
		);


	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeleteTurno(int id)
	=> this.SafeExecute(
		logger,
		PermisosAccionesCodigo.DeleteEntidades,
		() => repositorio.DeleteTurnoWhereId(new TurnoId(id)),
		notFoundMessage: $"No existe turno con id {id}"
	);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdateTurno(int id, [FromBody] TurnoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisosAccionesCodigo.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		turno => repositorio.UpdateTurnoWhereId(new TurnoId(id), turno),
		notFoundMessage: $"No existe turno con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearTurno([FromBody] TurnoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisosAccionesCodigo.CrearTurnos,
		dto,
		x => x.ToDomain(),
		turno => repositorio.InsertTurnoReturnId(turno)
	);








}
