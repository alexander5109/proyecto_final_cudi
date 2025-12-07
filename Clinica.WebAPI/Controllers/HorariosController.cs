using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.WebAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.DbModels;

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
		PermisoSistema.VerHorarios,
		() => repositorio.SelectHorarios()
	);



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetHorarioPorId(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerHorarios,
			() => repositorio.SelectHorarioWhereId(new HorarioMedicoId(id)),
			notFoundMessage: $"No existe horario con id {id}"
		);


	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeleteHorario(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.DeleteEntidades,
			() => repositorio.DeleteHorarioWhereId(new HorarioMedicoId(id)),
			notFoundMessage: $"No existe horario con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdateHorario(int id, [FromBody] HorarioDbModel dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		horario => repositorio.UpdateHorarioWhereId(horario),
		notFoundMessage: $"No existe horario con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearHorario([FromBody] HorarioDbModel dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.CrearHorarios,
		dto,
		x => x.ToDomain(),
		horario => repositorio.InsertHorarioReturnId(horario)
	);



}