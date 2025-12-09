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
public class ServiciosPublicosController(IRepositorio repositorio, IServiciosPublicos servicios, ILogger<ServiciosPublicosController> logger) : ControllerBase {

	[HttpGet("Turnos/Disponibilidades")]
	public async Task<IActionResult> VerDisponibilidades(
		[FromQuery] EspecialidadCodigo EspecialidadCodigo,
		[FromQuery] int cuantos,
		[FromQuery] DateTime aPartirDeCuando
	) {
		Result<IReadOnlyList<Disponibilidad2025>> result = await servicios.SolicitarDisponibilidades(
			EspecialidadCodigo,
			aPartirDeCuando,
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





}
