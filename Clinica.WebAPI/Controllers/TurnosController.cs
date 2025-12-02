using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using static Clinica.Shared.Dtos.ApiDtos;
using static Clinica.Shared.Dtos.DomainDtos;


namespace Clinica.WebAPI.Controllers;




[Authorize]
[ApiController]
[Route("[controller]")]
public class TurnosController(RepositorioInterface repositorio, ILogger<TurnosController> logger) : ControllerBase {

	// GET: api/<MedicosController>
	// [HttpGet]
	// public async Task<ActionResult<IEnumerable<TurnoDbModel>>> Get() {
	// try {
	// IEnumerable<TurnoDbModel> instances = await repositorio.SelectTurnos();
	// return Ok(instances);
	// } catch (Exception ex) {
	// logger.LogError(ex, "Error al obtener listado de instances.");
	// return StatusCode(500, "Error interno del servidor.");
	// }
	// }

	// --------------------------------------------------------
	// GET /turnos/{id}
	// --------------------------------------------------------
	// [HttpGet("{id:TurnoId}")]
	// public async Task<IActionResult> GetPorId([FromRoute] TurnoId id) {
	// Result<Turno2025> result = await repositorio.SelectTurnoWhereId(id);

	// return result switch {
	// Result<Turno2025>.Ok ok =>
	// Ok(ok.Valor.ToDomain()),

	// Result<Turno2025>.Error err =>
	// NotFound(new { error = err.Mensaje }),

	// _ => StatusCode(500),
	// };
	// }

	// --------------------------------------------------------
	// POST /turnos   (crear / agendar)
	// --------------------------------------------------------
	// [HttpPost]
	// public async Task<IActionResult> Crear([FromBody] CrearTurnoRequestDto dto) {
	// Result<EspecialidadMedica2025> espResult = EspecialidadMedica2025.CrearPorCodigoInterno(dto.EspecialidadCodigo);

	// if (espResult.IsError)
	// return BadRequest($"Especialidad inválida: {dto.EspecialidadCodigo}");

	// EspecialidadMedica2025 especialidad = espResult.GetOrRaise();

	// Result<Turno2025> result = await repositorio.AgendarTurnoAsync(
	// dto.PacienteId,
	// dto.MedicoId,
	// especialidad,
	// dto.Desde,
	// dto.Hasta
	// );

	// return result switch {
	// Result<Turno2025>.Ok ok =>
	// Ok(ok.Valor.ToDomain()),

	// Result<Turno2025>.Error err =>
	// BadRequest(new { error = err.Mensaje }),

	// _ => StatusCode(500),
	// };
	// }





	// --------------------------------------------------------
	// PUT /turnos/{id}/reprogramar
	// --------------------------------------------------------
	// [HttpPut("{id:TurnoId}/reprogramar")]
	// public async Task<IActionResult> Reprogramar([FromRoute] TurnoId id, [FromBody] ReprogramarTurnoRequestDto dto) {
	// Result<Turno2025> result = await repositorio.ReprogramarTurnoAsync(
	// id,
	// dto.NuevaFechaDesde,
	// dto.NuevaFechaHasta
	// );

	// return result switch {
	// Result<Turno2025>.Ok ok => Ok(ok.Valor.ToDomain()),
	// Result<Turno2025>.Error err => BadRequest(new { error = err.Mensaje }),
	// _ => StatusCode(500)
	// };
	// }

	// --------------------------------------------------------
	// PUT /turnos/{id}/cancelar
	// --------------------------------------------------------
	// [HttpPut("{id:TurnoId}/cancelar")]
	// public async Task<IActionResult> Cancelar(
	// [FromRoute] TurnoId id,
	// [FromBody] string? comentario) {
	// Result<Turno2025> result =
	// await repositorio.CancelarTurnoAsync(id, comentario.ToOption());

	// return result switch {
	// Result<Turno2025>.Ok ok => Ok(ok.Valor.ToDomain()),
	// Result<Turno2025>.Error err => BadRequest(new { error = err.Mensaje }),
	// _ => StatusCode(500)
	// };
	// }

	// --------------------------------------------------------
	// PUT /turnos/{id}/concretar
	// --------------------------------------------------------
	// [HttpPut("{id:TurnoId}/concretar")]
	// public async Task<IActionResult> Concretar([FromRoute] TurnoId id, [FromBody] string? comentario) {
	// Result<Turno2025> result = await repositorio.MarcarTurnoComoConcretadoAsync(id, comentario.ToOption());

	// return result switch {
	// Result<Turno2025>.Ok ok =>
	// Ok(ok.Valor.ToDomain()),

	// Result<Turno2025>.Error err =>
	// BadRequest(new { error = err.Mensaje }),

	// _ => StatusCode(500),
	// };
	// }

	// --------------------------------------------------------
	// PUT /turnos/{id}/ausente
	// --------------------------------------------------------
	// [HttpPut("{id:TurnoId}/ausente")]
	// public async Task<IActionResult> Ausente([FromRoute] TurnoId id, [FromBody] string? comentario) {
	// Result<Turno2025> result = await repositorio.MarcarTurnoComoAusenteAsync(id, comentario.ToOption());

	// return result switch {
	// Result<Turno2025>.Ok ok =>
	// Ok(ok.Valor.ToDomain()),

	// Result<Turno2025>.Error err =>
	// BadRequest(new { error = err.Mensaje }),

	// _ => StatusCode(500),
	// };
	// }
}
