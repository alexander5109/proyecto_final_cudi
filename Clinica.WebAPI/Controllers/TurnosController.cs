using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.Dtos;
using Clinica.Infrastructure.ServiciosAsync;
using Clinica.WebAPI.DTOs;
using Clinica.WebAPI.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TurnosController : ControllerBase {
	private readonly ServiciosPublicosAsync _servicio;
	private readonly ILogger<TurnosController> _logger;

	public TurnosController(
		ServiciosPublicosAsync servicio,
		ILogger<TurnosController> logger) {
		_servicio = servicio;
		_logger = logger;
	}

	// --------------------------------------------------------
	// GET /turnos/{id}
	// --------------------------------------------------------
	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetPorId(int id) {
		Result<Turno2025> result = await _servicio.ObtenerTurnoPorIdAsync(id);

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDTO()),

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

		var result = await _servicio.AgendarTurnoAsync(
			dto.PacienteId,
			dto.MedicoId,
			especialidad,
			dto.Desde,
			dto.Hasta
		);

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDTO()),

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
		Result<Turno2025> result = await _servicio.ReprogramarTurnoAsync(
			id,
			dto.NuevaFechaDesde,
			dto.NuevaFechaHasta
		);

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDTO()),

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
		var result = await _servicio.CancelarTurnoAsync(id, comentario.ToOption());

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDTO()),

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
		var result = await _servicio.MarcarTurnoComoConcretadoAsync(id, comentario.ToOption());

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDTO()),

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
		var result = await _servicio.MarcarTurnoComoAusenteAsync(id, comentario.ToOption());

		return result switch {
			Result<Turno2025>.Ok ok =>
				Ok(ok.Valor.ToDTO()),

			Result<Turno2025>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500),
		};
	}
}
