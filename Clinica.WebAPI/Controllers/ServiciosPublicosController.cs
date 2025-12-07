using System.ComponentModel;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeValor;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.ApiDtos;
namespace Clinica.WebAPI.Controllers;







[ApiController]
[Route("api/[controller]")]
public class ServiciosPublicosController(IRepositorio repositorio, ILogger<ServiciosPublicosController> logger) : ControllerBase {



	[HttpGet("Turnos/Disponibilidades")]
	public async Task<IActionResult> VerDisponibilidades(
		[FromQuery] EspecialidadCodigo EspecialidadCodigo,
		[FromQuery] int cuantos,
		[FromQuery, DefaultValue("2025-12-03T00:00:00")] DateTime? aPartirDeCuando
	) {
		DateTime desde = aPartirDeCuando ?? DateTime.Now; // default real acá

        Result<IReadOnlyList<Disponibilidad2025>> result = await ServiciosPublicos.SolicitarDisponibilidadesPara(
			EspecialidadCodigo,
			desde,
			cuantos,
			repositorio
		);

		return result switch {
			Result<IReadOnlyList<Disponibilidad2025>>.Ok ok =>
				Ok(ok.Valor),

			Result<IReadOnlyList<Disponibilidad2025>>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500)
		};
	}






	[HttpPost("Turnos/cancelar")]
	public async Task<IActionResult> SolicitarCancelacion(
		[FromBody] SolicitarCancelacionDto dto
	) {
		if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return Unauthorized();

		Result<Turno2025> result = await ServiciosPublicos.SolicitarCancelacion(
			dto.TurnoId,
			dto.OutcomeFecha,
			dto.OutcomeComentario,
			repositorio
		);
		return result.Match<IActionResult>(
			ok => Ok(ok),
			err => Problem(err)
		);
	}





	[HttpPost("turnos/reprogramar")]
	public async Task<IActionResult> SolicitarReprogramacion(
		[FromBody] SolicitarReprogramacionDto dto
	) {
		if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return Unauthorized();

		Result<Turno2025> result = await ServiciosPublicos.SolicitarReprogramacionALaPrimeraDisponibilidad(
			dto.TurnoId,
			dto.OutcomeFecha,
			dto.OutcomeComentario,
			repositorio
		);

		return result.Match<IActionResult>(
			ok => Ok(ok),
			err => Problem(err)
		);
	}






	//[HttpPut("{id:int}")] // ejemplo
	//public Task<IActionResult> UpdateTurno(int id, [FromBody] TurnoDbModel dto)
	//=> this.SafeExecuteWithDomain(
	//	logger,
	//	PermisoSistema.UpdateEntidades,
	//	dto,
	//	x => x.ToDomain(),
	//	turno => repositorio.UpdateTurnoWhereId(turno),
	//	notFoundMessage: $"No existe turno con id {id}"
	//);





	//[HttpPost("turnos/solicitar")]
	//public async Task<IActionResult> SolicitarTurnoPrimeraDisponibilidad([FromBody] SolicitarTurnoPrimeraDispDto dto) {
	//	return await this.SafeExecuteWithDomain<SolicitarTurnoPrimeraDispDto,PacienteId,Turno2025>(
	//		logger,
	//		PermisoSistema.SolicitarTurno,
	//		dto,
	//		d => Result.Ok(d.PacienteId), // mapping simple
	//		async pacienteId => await ServiciosPublicos.SolicitarTurnoEnLaPrimeraDisponibilidad(
	//			pacienteId,
	//			dto.Especialidad,
	//			new FechaRegistro2025(dto.FechaCreacion),
	//			repositorio
	//		)
	//	);
	//}







	// GET: api/<MedicosController>
	// [HttpGet]
	// public async Task<ActionResult<IEnumerable<TurnoDbModel>>> VerDisponibilidades() {
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
	// public async Task<IActionResult> CrearResult([FromBody] CrearTurnoRequestDto dto) {
	// Result<Especialidad2025> espResult = Especialidad2025.CrearResultPorCodigoInterno(dto.EspecialidadCodigo);

	// if (espResult.IsError)
	// return BadRequest($"Especialidad inválida: {dto.EspecialidadCodigo}");

	// Especialidad2025 especialidad = espResult.GetOrRaise();

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
