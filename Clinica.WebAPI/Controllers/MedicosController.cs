using Clinica.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.WebAPI.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MedicosController(
	IRepositorioMedicos repositorio, 
	ILogger<AuthController> logger
) : ControllerBase {

	[HttpGet]
	public Task<IActionResult> GetMedicos()
	=> this.SafeExecute(
		PermisoSistema.VerMedicos,
		() => repositorio.SelectMedicos()
	);



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetMedicoPorId(int id)
		=> this.SafeExecute(
			PermisoSistema.VerMedicos,
			() => repositorio.SelectMedicoWhereId(new MedicoId(id)),
			notFoundMessage: $"No existe medico con id {id}"
		);



	[HttpGet("{id}/turnos")]
	public Task<IActionResult> GetTurnosPorMedico([FromRoute] int id)
		=> this.SafeExecute(
			PermisoSistema.VerTurnos,
			() => repositorio.SelectTurnosWhereMedicoId(new MedicoId(id)),
			notFoundMessage: $"No existen turnos con medicoid {id}"
		);



	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeleteMedico(int id)
		=> this.SafeExecute(
			PermisoSistema.DeleteEntidades,
			() => repositorio.DeleteMedicoWhereId(new MedicoId(id)),
			notFoundMessage: $"No existe medico con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoDbModel dto)
	=> this.SafeExecuteWithDomain(
		PermisoSistema.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		medico => repositorio.UpdateMedicoWhereId(medico),
		notFoundMessage: $"No existe medico con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearMedico([FromBody] MedicoDbModel dto)
	=> this.SafeExecuteWithDomain(
		PermisoSistema.CrearMedicos,
		dto,
		x => x.ToDomain(),
		medico => repositorio.InsertMedicoReturnId(medico)
	);







}
