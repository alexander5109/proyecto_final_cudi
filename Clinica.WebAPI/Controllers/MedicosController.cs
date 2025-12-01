using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.Dtos.ApiDtos;
using static Clinica.Shared.Dtos.DomainDtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Clinica.WebAPI.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MedicosController(RepositorioInterface repositorio, ILogger<TurnosController> logger) : ControllerBase {
	// GET: api/<MedicosController>





	[HttpGet]
	public async Task<ActionResult<IEnumerable<MedicoDto>>> GetTodos() {
		if (HttpContext.Items["Usuario"] is not UsuarioBase2025 usuario)
			return Unauthorized("Token válido pero sin usuario asociado");

		Result<IEnumerable<Medico2025>> result =
			await ServiciosPublicos.SelectMedicos(usuario, repositorio);

		ActionResult<IEnumerable<MedicoDto>> respuesta = null!;
		result.Switch(
			ok => {
				respuesta = Ok(ok.Select(p => p.ToDto()));
			},
			error => {
				respuesta = Forbid(error);
			}
		);
		return respuesta;
	}













	[HttpGet("{id}")]
	public async Task<ActionResult<MedicoDto>> GetMedicoPorId([FromRoute] MedicoId id) {
		if (HttpContext.Items["Usuario"] is not UsuarioBase2025 usuario)
			return Unauthorized("Token válido pero sin usuario asociado");

		Result<Medico2025> result =
			await ServiciosPublicos.SelectMedicoWhereId(usuario, repositorio, id);

		ActionResult<MedicoDto> respuesta = null!;

		result.Switch(
			ok => {
				respuesta = Ok(ok.ToDto());
			},
			error => {
				respuesta = Problem(error);
			}
		);

		return respuesta;
	}





}
