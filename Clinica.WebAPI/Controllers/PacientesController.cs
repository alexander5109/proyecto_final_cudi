using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Dominio.Dtos.ApiDtos;
using static Clinica.Dominio.Dtos.DomainDtos;

namespace Clinica.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PacientesController(RepositorioInterface repositorio, ILogger<PacientesController> logger) : ControllerBase {

	// ------------------ HELPERS ------------------ //

	private ActionResult FromResult<T>(Result<T> result) {
		return result switch {
			Result<T>.Ok ok => Ok(ok.Valor),
			Result<T>.Error err => BadRequest(err.Mensaje),
			_ => StatusCode(500)
		};
	}

	private ActionResult FromResult(Result<Unit> result) {
		return result switch {
			Result<Unit>.Ok => NoContent(),
			Result<Unit>.Error e => BadRequest(e.Mensaje),
			_ => StatusCode(500)
		};
	}


	// ------------------ GET ------------------ //

	[HttpGet]
	public async Task<ActionResult> GetPacientes() {
		var result = await repositorio.SelectPacientes();
		return FromResult(result);
	}

	[HttpGet("list")]
	public async Task<ActionResult> GetPacientesList() {
		var result = await repositorio.SelectPacientes();
		return FromResult(result);
	}






	//[HttpGet("{pacienteId:PacienteId}/turnos")]
	//public async Task<ActionResult<IEnumerable<TurnoListDto>>> GetTurnosPorPaciente([FromRoute] PacienteId pacienteId) {
	//	//listo

	//	try {
	//		IEnumerable<TurnoListDto> instances = await repositorio.SelectTurnosWherePacienteId(pacienteId);
	//		return Ok(instances);
	//	} catch (Exception ex) {
	//		logger.LogError(ex, "Error al obtener listado de instances.");
	//		return StatusCode(500, "Error interno del servidor.");
	//	}

	//}



	// ------------------ DELETE ------------------ //

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete([FromRoute] PacienteId id) {
		Result<Unit> result = await repositorio.DeletePacienteWhereId(id);
		return FromResult(result);
	}

	// ------------------ POST ------------------ //

	[HttpPost]
	public async Task<ActionResult> Create([FromBody] PacienteDto dto) {
		if (dto is null)
			return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

		Result<Paciente2025> pacienteResult = dto.ToDomain();
		if (pacienteResult.IsError) return BadRequest(pacienteResult.UnwrapAsError());

		Result<PacienteId> result2 = await repositorio.InsertPacienteReturnId(pacienteResult.UnwrapAsOk());

		return result2 switch {
			Result<PacienteId>.Ok ok => CreatedAtAction(nameof(GetPacientes), new { id = ok.Valor }, ok.Valor),
			Result<PacienteId>.Error err => BadRequest(err.Mensaje),
			_ => StatusCode(500)
		};
	}

	// ------------------ PUT ------------------ //

	//[HttpPut("{id}")]
	//public async Task<ActionResult> Update([FromRoute] PacienteId id, [FromBody] PacienteDto dto) {
	//	if (dto is null)
	//		return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

	//	Result<Unit> result = await repositorio.UpdatePaciente(id, dto);
	//	return FromResult(result);
	//}
}