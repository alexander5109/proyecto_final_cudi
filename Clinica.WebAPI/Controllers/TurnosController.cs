using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;
using Clinica.Shared.ApiDtos;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.ApiDtos.TurnoDtos;
using static Clinica.Shared.DbModels.DbModels;
namespace Clinica.WebAPI.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TurnosController(
	IRepositorioTurnos repositorio, 
	ILogger<TurnosController> logger
) : ControllerBase {




	[HttpGet]
	public Task<ActionResult<IEnumerable<TurnoDbModel>>> GetTurnos()
	=> this.SafeExecute(
		logger,
		AccionesDeUsuarioEnum.VerTurnos,
		() => repositorio.SelectTurnos()
	);




	[HttpGet("{id:int}")]
	public Task<ActionResult<TurnoDbModel?>> GetTurnoPorId(int id)
	=> this.SafeExecute(
		logger,
		AccionesDeUsuarioEnum.VerTurnos,
		() => repositorio.SelectTurnoWhereId(TurnoId2025.Crear(id)),
		notFoundMessage: $"No existe turno con id {id}"
	);

	[HttpGet("medico/{id}")]
	public Task<ActionResult<IEnumerable<TurnoDbModel>>> GetTurnosPorMedico([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerTurnos,
			() => repositorio.SelectTurnosWhereMedicoId(MedicoId2025.Crear(id)),
			notFoundMessage: $"No existen turnos con medicoid {id}"
		);

	[HttpGet("paciente/{id}")]
	public Task<ActionResult<IEnumerable<TurnoDbModel>>> GetTurnosPorPaciente([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerTurnos,
			() => repositorio.SelectTurnosWherePacienteId(PacienteId2025.Crear(id)),
			notFoundMessage: $"No existen turnos con PacienteId2025 {id}"
		);


	[HttpDelete("{id:int}")]
	public Task<ActionResult<Unit>> DeleteTurno(int id)
	=> this.SafeExecute(
		logger,
		AccionesDeUsuarioEnum.EliminarEntidades,
		() => repositorio.DeleteTurnoWhereId(TurnoId2025.Crear(id)),
		notFoundMessage: $"No existe turno con id {id}"
	);



	[HttpPut("{id:int}")]
	public Task<ActionResult<TurnoDbModel>> UpdateTurno(int id, [FromBody] TurnoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		AccionesDeUsuarioEnum.ModificarEntidades,
		dto,
		x => x.ToDomain(),
		turno => repositorio.UpdateTurnoWhereId(TurnoId2025.Crear(id), turno),
		notFoundMessage: $"No existe turno con id {id}"
	);



	[HttpPost]
	public Task<ActionResult<TurnoId2025>> CrearTurno([FromBody] TurnoDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		AccionesDeUsuarioEnum.CrearTurnos,
		dto,
		x => x.ToDomain(),
		turno => repositorio.InsertTurnoReturnId(turno)
	);








}
