using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.ApiDtos.HorarioDtos;

namespace Clinica.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HorariosController(
	IRepositorioHorarios repositorio,
	ILogger<HorariosController> logger
) : ControllerBase {
	[HttpGet]
	public Task<IActionResult> GetHorarios()
	=> this.SafeExecute(
		logger,
		PermisosAccionesCodigo.VerHorarios,
		() => repositorio.SelectHorarios()
	);



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetHorarioPorId(int id)
		=> this.SafeExecute(
			logger,
			PermisosAccionesCodigo.VerHorarios,
			() => repositorio.SelectHorarioWhereId(new HorarioId(id)),
			notFoundMessage: $"No existe horario con id {id}"
		);


	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeleteHorario(int id)
		=> this.SafeExecute(
			logger,
			PermisosAccionesCodigo.DeleteEntidades,
			() => repositorio.DeleteHorarioWhereId(new HorarioId(id)),
			notFoundMessage: $"No existe horario con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdateHorario(int id, [FromBody] HorarioDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisosAccionesCodigo.UpdateHorarios,
		dto,
		x => x.ToDomain(),
		horario => repositorio.UpdateHorarioWhereId(new HorarioId(id), horario),
		notFoundMessage: $"No existe horario con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearHorario([FromBody] HorarioDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisosAccionesCodigo.CrearHorarios,
		dto,
		x => x.ToDomain(),
		horario => repositorio.InsertHorarioReturnId(horario)
	);



}