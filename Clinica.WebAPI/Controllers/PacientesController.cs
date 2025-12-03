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
public class PacientesController(IRepositorio repositorio, ILogger<PacientesController> logger) : ControllerBase {
	[HttpGet]
	public Task<IActionResult> GetPacientes()
	=> this.SafeExecute(
		PermisoSistema.VerPacientes,
	() => repositorio.SelectPacientes()
	);

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetPacientePorId(int id) {
		if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return Unauthorized();

		if (!usuario.HasPermission(PermisoSistema.VerPacientes))
			return Forbid();

		Result<PacienteDbModel?> resultado = await repositorio.SelectPacienteWhereId(new PacienteId(id));

		return resultado.Match<IActionResult>(
			ok => {
				if (ok is null)
					return NotFound(new { mensaje = $"No existe paciente con id {id}" });

				return Ok(ok);
			},
			error => Problem(error)
		);
	}


	[HttpDelete("{id:int}")]
	public async Task<IActionResult> EliminarPaciente(int id) {
		if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return Unauthorized();

		if (!usuario.HasPermission(PermisoSistema.EliminarEntidad))
			return Forbid();

		Result<Unit> deleteResult = await repositorio.DeletePacienteWhereId(new PacienteId(id));

		return deleteResult.Match<IActionResult>(
			_ => Ok(new { mensaje = $"Paciente {id} eliminado correctamente" }),
			error => Problem(error)
		);
	}

	[HttpGet("{id}/turnos")]
	public async Task<IActionResult> GetTurnosPorPaciente([FromRoute] int id) {
		if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return Unauthorized();

		if (!usuario.HasPermission(PermisoSistema.VerTurnos))
			return Forbid();

		var turnos = await repositorio.SelectTurnosWherePacienteId(new PacienteId(id));
		return Ok(turnos);
	}



	[HttpPut("{id:int}")]
	public async Task<IActionResult> ActualizarPaciente(int id, [FromBody] PacienteDbModel dto) {
		if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return Unauthorized();

		if (!usuario.HasPermission(PermisoSistema.EditarPacientes))
			return Forbid();

		// Validar dominio
		Result<Paciente2025> pacienteResult = dto.ToDomain();

		if (pacienteResult.IsError)
			return BadRequest(new { error = pacienteResult.UnwrapAsError() });

		Paciente2025 paciente = pacienteResult.UnwrapAsOk();

		// Este método recibe dominio ya validado
		Result<Unit> updateResult = await repositorio.UpdatePacienteWhereId(paciente);

		return updateResult.Match<IActionResult>(
			_ => Ok(new { mensaje = $"Paciente {id} actualizado correctamente" }),
			error => Problem(error)
		);
	}

	[HttpPost]
	public async Task<IActionResult> CrearPaciente([FromBody] PacienteDbModel dto) {
		if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return Unauthorized();

		if (!usuario.HasPermission(PermisoSistema.CrearPacientes))
			return Forbid();

		Result<Paciente2025> pacienteResult = dto.ToDomain();

		if (pacienteResult.IsError)
			return BadRequest(new { error = pacienteResult.UnwrapAsError() });

		Paciente2025 paciente = pacienteResult.UnwrapAsOk();

		Result<PacienteId> guardarResult =
			await repositorio.InsertPacienteReturnId(paciente);

		return guardarResult.Match<IActionResult>(
			pacienteAgregadoId => Ok(new {
				mensaje = $"Paciente creado exitosamente con id: {pacienteAgregadoId.Valor}"
			}),
			pacienteDBInsertError => Problem(pacienteDBInsertError)
		);
	}



}