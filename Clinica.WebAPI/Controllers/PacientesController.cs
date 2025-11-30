using Clinica.Infrastructure.ServiciosAsync;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DtosEntidades.DtosEntidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Clinica.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PacientesController(
	ServiciosPublicosAsync servicio,
	ILogger<PacientesController> logger
) : ControllerBase {



	// GET: api/<ValuesController>
	[HttpGet]
	public async Task<ActionResult<IEnumerable<PacienteDto>>> Get() {
		try {
			IEnumerable<PacienteDto> pacientes = await servicio.baseDeDatos.SelectPacientes();
			return Ok(pacientes);
		} catch (Exception ex) {
			logger.LogError(ex, "Error al obtener listado de pacientes.");
			return StatusCode(500, "Error interno del servidor.");
		}
	}



	// GET api/<ValuesController>/5
	[HttpGet("{id}")]
	public string Get(int id) {
		return "value";
	}

	// POST api/<ValuesController>
	[HttpPost]
	public void Post([FromBody] string value) {
	}

	// PUT api/<ValuesController>/5
	[HttpPut("{id}")]
	public void Put(int id, [FromBody] string value) {
	}

	// DELETE api/<ValuesController>/5
	[HttpDelete("{id}")]
	public void Delete(int id) {
	}
}
