using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.Dtos.ApiDtos;
using static Clinica.Shared.Dtos.DomainDtos;

namespace Clinica.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PacientesController(RepositorioInterface repositorio, ILogger<PacientesController> logger) : ControllerBase {

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

	[HttpGet]
	public async Task<ActionResult<IEnumerable<PacienteDto>>> GetPacientes() {

		UsuarioBase2025? usuario = HttpContext.Items["Usuario"] as UsuarioBase2025;

		if (usuario is null)
			return Unauthorized("Token válido pero sin usuario asociado");


		//UsuarioBase2025 usuario = HttpContext.Items["Usuario"] as UsuarioBase2025
			//?? throw new Exception("Usuario no autenticado");

		//return Ok("Hola mundo todo bien?");

		Result<IEnumerable<Paciente2025>> result = await ServiciosPublicos.SelectPacientes(usuario, repositorio);

		return result switch {
			Result<IEnumerable<Paciente2025>>.Ok ok =>
				Ok(ok.Valor.Select(p => p.ToDto())),

			Result<IEnumerable<Paciente2025>>.Error err =>
				Forbid(err.Mensaje),

			_ => StatusCode(500)
		};
	}

	[HttpGet("list")]
	public async Task<ActionResult> GetPacientesList() {
		var result = await repositorio.SelectPacientes();
		return FromResult(result);
	}



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

}