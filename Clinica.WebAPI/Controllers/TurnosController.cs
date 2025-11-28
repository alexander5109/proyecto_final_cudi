using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.DtosEntidades;
using Clinica.Infrastructure.ServiciosAsync;
using Clinica.WebAPI.DtosWebAPI;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DtosEntidades.DtosEntidades;
using static Clinica.WebAPI.DtosWebAPI.DtosWebAPI;


namespace Clinica.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TurnosController(
	ServiciosPublicosAsync servicio,
	ILogger<TurnosController> logger
) : ControllerBase {



	// GET: api/<MedicosController>
	[HttpGet]
	public async Task<ActionResult<IEnumerable<TurnoDto>>> Get() {
		try {
			IEnumerable<TurnoDto> instances = await servicio.baseDeDatos.SelectTurnos();
			return Ok(instances);
		} catch (Exception ex) {
			logger.LogError(ex, "Error al obtener listado de instances.");
			return StatusCode(500, "Error interno del servidor.");
		}
	}

	// --------------------------------------------------------
	// GET /turnos/{id}
	// --------------------------------------------------------
	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetPorId(int id) {
		Result<Turno2025> result = await servicio.ObtenerTurnoPorIdAsync(id);

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDto()),

			Result<Turno2025>.Error err =>
				NotFound(new { error = err.Mensaje }),

			_ => StatusCode(500),
		};
	}

	// --------------------------------------------------------
	// POST /turnos   (crear / agendar)
	// --------------------------------------------------------
	[HttpPost]
	public async Task<IActionResult> Crear([FromBody] CrearTurnoRequestDto dto) {
		// Validar especialidad
		var espResult = EspecialidadMedica2025.CrearPorCodigoInterno(dto.EspecialidadCodigo);

		if (espResult.IsError)
			return BadRequest($"Especialidad inválida: {dto.EspecialidadCodigo}");

		EspecialidadMedica2025 especialidad = espResult.GetOrRaise();

		Result<Turno2025> result = await servicio.AgendarTurnoAsync(
			dto.PacienteId,
			dto.MedicoId,
			especialidad,
			dto.Desde,
			dto.Hasta
		);

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDto()),

			Result<Turno2025>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500),
		};
	}

	// --------------------------------------------------------
	// PUT /turnos/{id}/reprogramar
	// --------------------------------------------------------
	[HttpPut("{id:int}/reprogramar")]
	public async Task<IActionResult> Reprogramar(int id, [FromBody] ReprogramarTurnoRequestDto dto) {
		Result<Turno2025> result = await servicio.ReprogramarTurnoAsync(
			id,
			dto.NuevaFechaDesde,
			dto.NuevaFechaHasta
		);

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDto()),

			Result<Turno2025>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500),
		};
	}

	// --------------------------------------------------------
	// PUT /turnos/{id}/cancelar
	// --------------------------------------------------------
	[HttpPut("{id:int}/cancelar")]
	public async Task<IActionResult> Cancelar(int id, [FromBody] string? comentario) {
		Result<Turno2025> result = await servicio.CancelarTurnoAsync(id, comentario.ToOption());

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDto()),

			Result<Turno2025>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500),
		};
	}

	// --------------------------------------------------------
	// PUT /turnos/{id}/concretar
	// --------------------------------------------------------
	[HttpPut("{id:int}/concretar")]
	public async Task<IActionResult> Concretar(int id, [FromBody] string? comentario) {
		Result<Turno2025> result = await servicio.MarcarTurnoComoConcretadoAsync(id, comentario.ToOption());

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDto()),

			Result<Turno2025>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500),
		};
	}

	// --------------------------------------------------------
	// PUT /turnos/{id}/ausente
	// --------------------------------------------------------
	[HttpPut("{id:int}/ausente")]
	public async Task<IActionResult> Ausente(int id, [FromBody] string? comentario) {
		Result<Turno2025> result = await servicio.MarcarTurnoComoAusenteAsync(id, comentario.ToOption());

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDto()),

			Result<Turno2025>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500),
		};
	}
}
