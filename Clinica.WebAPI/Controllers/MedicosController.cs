using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.ApiDtos;
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
		logger,
		PermisoSistema.VerMedicos,
		() => repositorio.SelectMedicos()
	);


	[HttpGet("por-especialidad/{code}")]
	public Task<IActionResult> GetMedicosWhereEspecialidadCodigo([FromRoute] EspecialidadCodigo code)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerMedicos,
			() => repositorio.SelectMedicosWhereEspecialidadCodigo(code)
		);



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetMedicoWhereId(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerMedicos,
			() => repositorio.SelectMedicoWhereId(new MedicoId(id)),
			notFoundMessage: $"No existe medico con id {id}"
		);


	[HttpGet("{id}/turnos")]
	public Task<IActionResult> GetTurnosPorMedico([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerTurnos,
			() => repositorio.SelectTurnosWhereMedicoId(new MedicoId(id)),
			notFoundMessage: $"No existen turnos con medicoid {id}"
		);



	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeleteMedico(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.DeleteEntidades,
			() => repositorio.DeleteMedicoWhereId(new MedicoId(id)),
			notFoundMessage: $"No existe medico con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		medico => repositorio.UpdateMedicoWhereId(new MedicoId(id), medico),
		notFoundMessage: $"No existe medico con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearMedico([FromBody] MedicoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.CrearMedicos,
		dto,
		x => x.ToDomain(),
		medico => repositorio.InsertMedicoReturnId(medico)
	);







}
