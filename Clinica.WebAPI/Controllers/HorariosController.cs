using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.IRepositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.ApiDtos.HorarioDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HorariosController(
	IRepositorioHorarios repositorio,
	ILogger<HorariosController> logger
) : ControllerBase {



	[HttpPut("{medicoId:int}")]
	public Task<ActionResult<Unit>> UpsertHorarios(
		int medicoId,
		[FromBody] IReadOnlyCollection<HorarioFranja2025> franjasDto
	) => this.SafeExecuteWithDomain(
			logger,
			PermisosAccionesEnum.UpdateHorarios,
			franjasDto,
			x => HorariosMedicos2025Agg.CrearResult(new MedicoId(medicoId), franjasDto),
			agg => repositorio.UpsertHorariosWhereMedicoId(agg)
		);



	[HttpGet]
	public Task<ActionResult<IEnumerable<HorarioDbModel>>> GetHorarios()
	=> this.SafeExecute(
		logger,
		PermisosAccionesEnum.VerHorarios,
		() => repositorio.SelectHorarios()
	);



	//[HttpGet("{id:int}")]
	//public Task<ActionResult<HorarioDbModel?>> GetHorarioPorId(int id)
	//	=> this.SafeExecute(
	//		logger,
	//		PermisosAccionesEnum.VerHorarios,
	//		() => repositorio.SelectHorarioWhereId(new HorarioId(id)),
	//		notFoundMessage: $"No existe horario con id {id}"
	//	);


	//[HttpDelete("{id:int}")]
	//public Task<ActionResult<Unit>> DeleteHorario(int id)
	//	=> this.SafeExecute(
	//		logger,
	//		PermisosAccionesEnum.DeleteEntidades,
	//		() => repositorio.DeleteHorarioWhereId(new HorarioId(id)),
	//		notFoundMessage: $"No existe horario con id {id}"
	//	);



	//[HttpPut("{id:int}")]
	//public Task<ActionResult<HorarioDbModel>> UpdateHorario(int id, [FromBody] HorarioDto dto)
	//=> this.SafeExecuteWithDomain(
	//	logger,
	//	PermisosAccionesEnum.UpdateHorarios,
	//	dto,
	//	x => x.ToDomain(),
	//	horario => repositorio.UpdateHorarioWhereId(new HorarioId(id), horario),
	//	notFoundMessage: $"No existe horario con id {id}"
	//);



	//[HttpPost]
	//public Task<ActionResult<HorarioId>> CrearHorario([FromBody] HorarioDto dto)
	//=> this.SafeExecuteWithDomain(
	//	logger,
	//	PermisosAccionesEnum.CrearHorarios,
	//	dto,
	//	x => x.ToDomain(),
	//	horario => repositorio.InsertHorarioReturnId(horario)
	//);






	//--------------------------------------------------MERGING-------------------------------------------------//

	//[HttpGet]
	//public Task<ActionResult<IEnumerable<HorarioFranja2025WithMedicoId>>> GetHorarios()
	//	=> this.SafeExecute(
	//		logger,
	//		PermisosAccionesEnum.VerHorarios,
	//		() => repositorio.SelectHorarios()
	//	);


	//[HttpGet("/medicos/{medicoId:int}/horarios")]
	//public Task<ActionResult<IEnumerable<HorarioFranja2025WithMedicoId>>> GetHorariosPorMedico(int medicoId)
	//	=> this.SafeExecute(
	//		logger,
	//		PermisosAccionesEnum.VerHorarios,
	//		() => repositorio.SelectHorariosWhereMedicoId(new MedicoId(medicoId)),
	//		notFoundMessage: $"No existen horarios para el médico {medicoId}"
	//	);



	//[HttpDelete("/medicos/{medicoId:int}/horarios")]
	//public Task<ActionResult<Unit>> DeleteHorariosPorMedico(int medicoId)
	//	=> this.SafeExecute(
	//		logger,
	//		PermisosAccionesEnum.DeleteEntidades,
	//		() => repositorio.DeleteHorariosWhereMedicoId(new MedicoId(medicoId)),
	//		notFoundMessage: $"No existen horarios para el médico {medicoId}"
	//	);

}