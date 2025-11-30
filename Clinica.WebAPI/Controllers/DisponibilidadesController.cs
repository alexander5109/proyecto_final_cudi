using Microsoft.AspNetCore.Mvc;
using Clinica.Infrastructure.ServiciosAsync;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.Comun;
using static Clinica.Shared.Dtos.DomainDtos;
using static Clinica.Shared.Dtos.ApiDtos;


namespace Clinica.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DisponibilidadesController(ServiciosPublicosAsync servicio, ILogger<DisponibilidadesController> logger) : ControllerBase {


	[HttpGet(Name = "GetDisponibilidades")]
	public async Task<IActionResult> Get(
		[FromQuery] int especialidadCodigoInterno,
		[FromQuery] int cuantos = 10) {
		Result<EspecialidadMedica2025> especialidadResult = EspecialidadMedica2025.CrearPorCodigoInterno(especialidadCodigoInterno);
		if (especialidadResult.IsError) {
			return BadRequest($"Especialidad inválida: {especialidadResult}");
		}
		EspecialidadMedica2025 especialidad = especialidadResult.GetOrRaise();

		Result<IReadOnlyList<DisponibilidadEspecialidad2025>> result = await servicio.SolicitarDisponibilidadesPara(
			especialidad,
			DateTime.Now,
			cuantos
		);
		return result switch {
			Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok ok =>
				Ok(ok.Valor.Select(d => d.ToDto()).ToList()),

			Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500),
		};
	}
}
