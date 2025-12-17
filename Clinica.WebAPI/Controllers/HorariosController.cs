using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		[FromBody] IReadOnlyCollection<HorarioFranja2026> franjasDto
	) => this.SafeExecuteWithDomain(
			logger,
			AccionesDeUsuarioEnum.ModificarHorarios,
			franjasDto,
			x => HorariosMedicos2026Agg.CrearResult(MedicoId2025.Crear(medicoId), franjasDto),
			agg => repositorio.UpsertHorariosWhereMedicoId(agg)
		);



	[HttpGet]
	public Task<ActionResult<IEnumerable<HorarioDbModel>>> GetHorarios()
	=> this.SafeExecute(
		logger,
		AccionesDeUsuarioEnum.VerHorarios,
		() => repositorio.SelectHorarios()
	);



	//[HttpGet("{id:int}")]
	//public Task<ActionResult<HorarioDbModel?>> GetHorarioPorId(int id)
	//	=> this.SafeExecute(
	//		logger,
	//		AccionesDeUsuarioEnum.VerHorarios,
	//		() => repositorio.SelectHorarioWhereId(new HorarioId2025(id)),
	//		notFoundMessage: $"No existe horario con id {id}"
	//	);


	//[HttpDelete("{id:int}")]
	//public Task<ActionResult<Unit>> DeleteHorario(int id)
	//	=> this.SafeExecute(
	//		logger,
	//		AccionesDeUsuarioEnum.EliminarEntidades,
	//		() => repositorio.DeleteHorarioWhereId(new HorarioId2025(id)),
	//		notFoundMessage: $"No existe horario con id {id}"
	//	);



	//[HttpPut("{id:int}")]
	//public Task<ActionResult<HorarioDbModel>> UpdateHorario(int id, [FromBody] HorarioDto dto)
	//=> this.SafeExecuteWithDomain(
	//	logger,
	//	AccionesDeUsuarioEnum.ModificarHorarios,
	//	dto,
	//	x => x.ToDomain(),
	//	horario => repositorio.UpdateHorarioWhereId(new HorarioId2025(id), horario),
	//	notFoundMessage: $"No existe horario con id {id}"
	//);



	//[HttpPost]
	//public Task<ActionResult<HorarioId2025>> CrearHorario([FromBody] HorarioDto dto)
	//=> this.SafeExecuteWithDomain(
	//	logger,
	//	AccionesDeUsuarioEnum.CrearHorarios,
	//	dto,
	//	x => x.ToDomain(),
	//	horario => repositorio.InsertHorarioReturnId(horario)
	//);






	//--------------------------------------------------MERGING-------------------------------------------------//

	//[HttpGet]
	//public Task<ActionResult<IEnumerable<HorarioFranja2025WithMedicoId>>> GetHorarios()
	//	=> this.SafeExecute(
	//		logger,
	//		AccionesDeUsuarioEnum.VerHorarios,
	//		() => repositorio.SelectHorarios()
	//	);


	//[HttpGet("/medicos/{medicoId:int}/horarios")]
	//public Task<ActionResult<IEnumerable<HorarioFranja2025WithMedicoId>>> GetHorariosPorMedico(int medicoId)
	//	=> this.SafeExecute(
	//		logger,
	//		AccionesDeUsuarioEnum.VerHorarios,
	//		() => repositorio.SelectHorariosWhereMedicoId(MedicoId2025.Crear(medicoId)),
	//		notFoundMessage: $"No existen horarios para el médico {medicoId}"
	//	);



	//[HttpDelete("/medicos/{medicoId:int}/horarios")]
	//public Task<ActionResult<Unit>> DeleteHorariosPorMedico(int medicoId)
	//	=> this.SafeExecute(
	//		logger,
	//		AccionesDeUsuarioEnum.EliminarEntidades,
	//		() => repositorio.DeleteHorariosWhereMedicoId(MedicoId2025.Crear(medicoId)),
	//		notFoundMessage: $"No existen horarios para el médico {medicoId}"
	//	);

}