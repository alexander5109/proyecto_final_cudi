using Microsoft.AspNetCore.Mvc;
using Clinica.Infrastructure.ServiciosAsync;
using Clinica.WebAPI.Mapping;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.Comun;

namespace Clinica.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DisponibilidadesController : ControllerBase {
	private readonly ServiciosPublicosAsync _servicio;
	private readonly ILogger<DisponibilidadesController> _logger;

	public DisponibilidadesController(
		ServiciosPublicosAsync servicio,
		ILogger<DisponibilidadesController> logger) {
		_servicio = servicio;
		_logger = logger;
	}

	[HttpGet(Name = "GetDisponibilidades")]
	public async Task<IActionResult> Get(
		[FromQuery] int especialidadCodigoInterno,
		[FromQuery] int cuantos = 10) {
		Result<EspecialidadMedica2025> especialidadResult = EspecialidadMedica2025.CrearPorCodigoInterno(especialidadCodigoInterno);
		if (especialidadResult.IsError) { 
			return BadRequest($"Especialidad inválida: {especialidadResult}"); 
		}
		EspecialidadMedica2025 especialidad = especialidadResult.GetOrRaise();

        Result<IReadOnlyList<DisponibilidadEspecialidad2025>> result = await _servicio.SolicitarDisponibilidadesPara(
			especialidad,
			DateTime.Now,
			cuantos
		);

		return result switch {
			Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok ok =>
				Ok(ok.Valor.Select(d => d.ToDTO()).ToList()),

			Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500),
		};
	}
}
