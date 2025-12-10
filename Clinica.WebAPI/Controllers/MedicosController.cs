using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.ApiDtos.MedicoDtos;
using static Clinica.Shared.ApiDtos.PacienteDtos;

namespace Clinica.WebAPI.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MedicosController(
	IRepositorioTurnos repositorioTurnos,
	IRepositorioMedicos repositorioMedicos,
	ILogger<AuthController> logger
) : ControllerBase {

	[HttpGet]
	public Task<IActionResult> GetMedicos() => this.SafeExecute(
		logger,
		PermisoSistema.VerMedicos,
		() => repositorioMedicos.SelectMedicos()
	);


	[HttpGet("por-especialidad/{code}")]
	public Task<IActionResult> GetMedicosWhereEspecialidadCodigo([FromRoute] EspecialidadCodigo code)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerMedicos,
			() => repositorioMedicos.SelectMedicosWhereEspecialidadCodigo(code)
		);



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetMedicoWhereId(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerMedicos,
			() => repositorioMedicos.SelectMedicoWhereId(new MedicoId(id)),
			notFoundMessage: $"No existe medico con id {id}"
		);


	[HttpGet("{id}/turnos")]
	public Task<IActionResult> GetTurnosPorMedico([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerTurnos,
			() => repositorioMedicos.SelectTurnosWhereMedicoId(new MedicoId(id)),
			notFoundMessage: $"No existen turnos con medicoid {id}"
		);



	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeleteMedico(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.DeleteEntidades,
			() => repositorioMedicos.DeleteMedicoWhereId(new MedicoId(id)),
			notFoundMessage: $"No existe medico con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		medico => repositorioMedicos.UpdateMedicoWhereId(new MedicoId(id), medico),
		notFoundMessage: $"No existe medico con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearMedico([FromBody] MedicoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.CrearMedicos,
		dto,
		x => x.ToDomain(),
		medico => repositorioMedicos.InsertMedicoReturnId(medico)
	);







}
