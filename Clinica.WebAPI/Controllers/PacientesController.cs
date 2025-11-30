using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Infrastructure.ServiciosAsync;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.Dtos.ApiDtos;
using static Clinica.Shared.Dtos.DomainDtos;

namespace Clinica.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PacientesController(
	ServiciosPublicosAsync servicio,
	ILogger<PacientesController> logger
) : ControllerBase {

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
		var result = await servicio.baseDeDatos.SelectPacientes();
		return FromResult(result);
	}

	[HttpGet("list")]
	public async Task<ActionResult> GetPacientesList() {
		var result = await servicio.baseDeDatos.SelectPacientesList();
		return FromResult(result);
	}

	// ------------------ DELETE ------------------ //

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id) {
		var result = await servicio.baseDeDatos.DeletePaciente(id);
		return FromResult(result);
	}

	// ------------------ POST ------------------ //

	[HttpPost]
	public async Task<ActionResult> Create([FromBody] PacienteDto dto) {
		if (dto is null)
			return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

		var result = await servicio.baseDeDatos.CreatePaciente(dto);

		return result switch {
			Result<int>.Ok ok => CreatedAtAction(nameof(GetPacientes), new { id = ok.Valor }, ok.Valor),
			Result<int>.Error err => BadRequest(err.Mensaje),
			_ => StatusCode(500)
		};
	}

	// ------------------ PUT ------------------ //

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] PacienteDto dto) {
		if (dto is null)
			return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

		var result = await servicio.baseDeDatos.UpdatePaciente(new PacienteId(id), dto);
		return FromResult(result);
	}
}