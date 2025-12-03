using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PacientesController(
	IRepositorio repositorio, 
	ILogger<PacientesController> logger
) : ControllerBase {


	[HttpGet]
	public Task<IActionResult> GetPacientes()
	=> this.SafeExecute(
		PermisoSistema.VerPacientes,
		() => repositorio.SelectPacientes()
	);



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetPacientePorId(int id)
		=> this.SafeExecute(
			PermisoSistema.VerPacientes,
			() => repositorio.SelectPacienteWhereId(new PacienteId(id)),
			notFoundMessage: $"No existe paciente con id {id}"
		);



	[HttpGet("{id}/turnos")]
	public Task<IActionResult> GetTurnosPorPaciente([FromRoute] int id)
		=> this.SafeExecute(
			PermisoSistema.VerTurnos,
			() => repositorio.SelectTurnosWherePacienteId(new PacienteId(id)),
			notFoundMessage: $"No existen turnos con pacienteid {id}"
		);



	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeletePaciente(int id)
		=> this.SafeExecute(
			PermisoSistema.EliminarEntidad,
			() => repositorio.DeletePacienteWhereId(new PacienteId(id)),
			notFoundMessage: $"No existe paciente con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteDbModel dto)
	=> this.SafeExecuteWithDomain(
		PermisoSistema.EditarPacientes,
		dto,
		x => x.ToDomain(),
		paciente => repositorio.UpdatePacienteWhereId(paciente),
		notFoundMessage: $"No existe paciente con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearPaciente([FromBody] PacienteDbModel dto)
	=> this.SafeExecuteWithDomain(
		PermisoSistema.CrearPacientes,
		dto,
		x => x.ToDomain(),
		paciente => repositorio.InsertPacienteReturnId(paciente)
	);



}