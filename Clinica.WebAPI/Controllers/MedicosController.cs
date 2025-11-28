using Clinica.Infrastructure.ServiciosAsync;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DtosEntidades.DtosEntidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Clinica.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicosController(
	ServiciosPublicosAsync servicio,
	ILogger<TurnosController> logger
) : ControllerBase {
	// GET: api/<MedicosController>
	[HttpGet]
	public async Task<ActionResult<IEnumerable<MedicoDto>>> Get() {
		try {
			IEnumerable<MedicoDto> instances = await servicio.baseDeDatos.SelectMedicos();
			return Ok(instances);
		} catch (Exception ex) {
			logger.LogError(ex, "Error al obtener listado de instances.");
			return StatusCode(500, "Error interno del servidor.");
		}
	}
	// GET api/<MedicosController>/5
	[HttpGet("{id}")]
	public string Get(int id) {
		return "value";
	}

	// POST api/<MedicosController>
	[HttpPost]
	public void Post([FromBody] string value) {
	}

	// PUT api/<MedicosController>/5
	[HttpPut("{id}")]
	public void Put(int id, [FromBody] string value) {
	}

	// DELETE api/<MedicosController>/5
	[HttpDelete("{id}")]
	public void Delete(int id) {
	}
}
