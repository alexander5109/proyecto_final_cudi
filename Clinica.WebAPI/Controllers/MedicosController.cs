using Clinica.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.WebAPI.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MedicosController(IRepositorio repositorio, ILogger<AuthController> logger) : ControllerBase {
	// GET: api/<MedicosController>

	[HttpGet("{id}")]
	public async Task<ActionResult<MedicoDbModel>> GetMedicoPorId([FromRoute] MedicoId id) {
		if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return Unauthorized();

		if (!usuario.HasPermission(PermisoSistema.VerMedicos))
			return Forbid();

 		var medico = await repositorio.SelectMedicoWhereId(id);
		return medico is null ? NotFound() : Ok(medico);
	}




	//[HttpGet]
	//public async Task<ActionResult<IEnumerable<MedicoDto>>> GetTodos() {
	//	if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
	//		return Unauthorized("Token válido pero sin usuario asociado");

	//	Result<IEnumerable<Medico2025>> result =
	//		await ServiciosPublicos.SelectMedicos(usuario, repositorio);

	//	ActionResult<IEnumerable<MedicoDto>> respuesta = null!;
	//	result.Switch(
	//		ok => {
	//			respuesta = Ok(ok.Select(p => p.ToDto()));
	//		},
	//		error => {
	//			respuesta = Forbid(error);
	//		}
	//	);
	//	return respuesta;
	//}













	//[HttpGet("{id}")]
	//public async Task<ActionResult<MedicoDto>> GetMedicoPorId([FromRoute] MedicoId id) {
	//	if (HttpContext.Items["Usuario"] is not Usuario2025 usuario)
	//		return Unauthorized("Token válido pero sin usuario asociado");

	//	Result<Medico2025> result =
	//		await ServiciosPublicos.SelectMedicoWhereId(usuario, repositorio, id);

	//	ActionResult<MedicoDto> respuesta = null!;

	//	result.Switch(
	//		ok => {
	//			respuesta = Ok(ok.ToDto());
	//		},
	//		error => {
	//			respuesta = Problem(error);
	//		}
	//	);

	//	return respuesta;
	//}





}
