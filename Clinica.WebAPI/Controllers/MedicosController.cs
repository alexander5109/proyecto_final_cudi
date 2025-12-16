using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;
using Clinica.Shared.ApiDtos;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.ApiDtos.MedicoDtos;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.WebAPI.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MedicosController(
	IRepositorioMedicos repositorio,
	ILogger<AuthController> logger
) : ControllerBase {

	[HttpGet]
	public async Task<ActionResult<IEnumerable<MedicoDbModel>>> GetMedicos() {
        ActionResult<IEnumerable<MedicoDbModel>> response = await this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerMedicos,
			() => repositorio.SelectMedicos()
		);
        ActionResult? test = response.Result;
		return response;
	}

	//[HttpGet("con-horarios/")]
	//public Task<ActionResult<IEnumerable<MedicoDbModel>>> GetMedicosWithHorarios() => this.SafeExecute(
	//	logger,
	//	AccionesDeUsuarioEnum.VerMedicos,
	//	() => repositorio.SelectMedicosWithHorarios()
	//);


	[HttpGet("por-especialidad/{code}")]
	public Task<ActionResult<IEnumerable<MedicoDbModel>>> GetMedicosWhereEspecialidadCodigo([FromRoute] EspecialidadEnum code)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerMedicos,
			() => repositorio.SelectMedicosWhereEspecialidadCodigo(code)
		);



	[HttpGet("{id:int}")]
	public Task<ActionResult<MedicoDbModel?>> GetMedicoWhereId(int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerMedicos,
			() => repositorio.SelectMedicoWhereId(new MedicoId(id)),
			notFoundMessage: $"No existe medico con id {id}"
		);


	[HttpGet("{id}/turnos")]
	public Task<ActionResult<IEnumerable<TurnoDbModel>>> GetTurnosPorMedico([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerTurnos,
			() => repositorio.SelectTurnosWhereMedicoId(new MedicoId(id)),
			notFoundMessage: $"No existen turnos con medicoid {id}"
		);



	[HttpDelete("{id:int}")]
	public Task<ActionResult<Unit>> DeleteMedico(int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.DeleteEntidades,
			() => repositorio.DeleteMedicoWhereId(new MedicoId(id)),
			notFoundMessage: $"No existe medico con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<ActionResult<MedicoDbModel>> UpdateMedico(int id, [FromBody] MedicoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		AccionesDeUsuarioEnum.UpdateEntidades,
		dto,
		x => x.ToDomain(),
		medico => repositorio.UpdateMedicoWhereId(new MedicoId(id), medico),
		notFoundMessage: $"No existe medico con id {id}"
	);



	[HttpPost]
	public Task<ActionResult<MedicoId>> CrearMedico([FromBody] MedicoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		AccionesDeUsuarioEnum.CrearMedicos,
		dto,
		x => x.ToDomain(),
		medico => repositorio.InsertMedicoReturnId(medico)
	);







}
