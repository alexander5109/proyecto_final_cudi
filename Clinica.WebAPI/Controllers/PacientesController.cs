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



	[Authorize]
	[HttpGet("{id}/turnos")]
	public async Task<ActionResult<IEnumerable<TurnoDto>>> GetTurnosPorPaciente([FromRoute] PacienteId id) {
		if (HttpContext.Items["Usuario"] is not UsuarioBase2025 usuario)
			return Unauthorized("Token válido pero sin usuario asociado");

		Result<IEnumerable<Turno2025>> result =
			await ServiciosPublicos.SelectTurnosWherePacienteId(usuario, repositorio, id);

		ActionResult<IEnumerable<TurnoDto>> respuesta = null!;

		result.Switch(
			ok => {
				respuesta = Ok(ok.Select(t => t.ToDto()));
			},
			error => {
				respuesta = Forbid(error);
			}
		);

		return respuesta;
	}


	[HttpGet]
	public async Task<ActionResult<IEnumerable<PacienteDto>>> GetPacientes() {
		if (HttpContext.Items["Usuario"] is not UsuarioBase2025 usuario)
			return Unauthorized("Token válido pero sin usuario asociado");
		Result<IEnumerable<Paciente2025>> result =
			await ServiciosPublicos.SelectPacientes(usuario, repositorio);
		ActionResult<IEnumerable<PacienteDto>> respuesta = null!;

		result.Switch(
			ok => {
				respuesta = Ok(ok.Select(p => p.ToDto()));
			},
			error => {
				respuesta = Forbid(error);
			}
		);
		return respuesta;
	}




	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete([FromRoute] PacienteId id) {
		if (HttpContext.Items["Usuario"] is not UsuarioBase2025 usuario)
			return Unauthorized("Token válido pero sin usuario asociado");

		Result<Unit> result = await ServiciosPublicos.DeletePacienteWhereId(usuario, repositorio, id);
		return FromResult(result);
	}

	[HttpPost]
	public async Task<ActionResult> Create([FromBody] PacienteDto dto) {
		if (dto is null) return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
		Result<Paciente2025> pacienteResult = dto.ToDomain();
		if (pacienteResult.IsError) return BadRequest(pacienteResult.UnwrapAsError());

		if (HttpContext.Items["Usuario"] is not UsuarioBase2025 usuario)
			return Unauthorized("Token válido pero sin usuario asociado");

		Result<PacienteId> result2 = await ServiciosPublicos.InsertPaciente(usuario, repositorio, pacienteResult.UnwrapAsOk());

		return result2 switch {
			Result<PacienteId>.Ok ok => CreatedAtAction(nameof(GetPacientes), new { id = ok.Valor }, ok.Valor),
			Result<PacienteId>.Error err => BadRequest(err.Mensaje),
			_ => StatusCode(500)
		};
	}
}