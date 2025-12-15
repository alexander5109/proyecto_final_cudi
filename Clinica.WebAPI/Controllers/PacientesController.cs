using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;
using Clinica.Shared.ApiDtos;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PacientesController(
	IRepositorioPacientes repositorio,
	ILogger<PacientesController> logger
) : ControllerBase {


	[HttpGet]
	public Task<ActionResult<IEnumerable<PacienteDbModel>>> GetPacientes()
	=> this.SafeExecute(
		logger,
		PermisosAccionesEnum.VerPacientes,
		() => repositorio.SelectPacientes()
	);


	//tonto.
	//[HttpGet("por-turnos/{id:int}")]
	//public Task<ActionResult<Carajo>> GetPacientePorTurnoId(int id)
	//	=> this.SafeExecute(
	//		logger,
	//		PermisosAccionesEnum.VerPacientes,
	//		() => repositorio.SelectPacienteWhereTurnoId(new TurnoId(id)),
	//		notFoundMessage: $"No existe paciente con id {id}"
	//	);


	//[HttpGet("AsDomain")]
	//public async Task<ActionResult<Carajo>> GetPacientesAsDomain() {


	//Result<Turno2025> result = await ServiciosPublicos.PersistirComoCanceladoAsync(
	//	dto.TurnoId,
	//	dto.OutcomeFecha,
	//	dto.OutcomeComentario,
	//	repositorio
	//);
	//return result.MatchAndSet<IActionResult>(
	//	ok => Ok(ok.ToModel()),
	//	err => Problem(err)
	//);

	//       IEnumerable<PacienteDbModel> vasdfr = (await repositorio.SelectPacientes()).UnwrapAsOk();
	//       IEnumerable<Result<Paciente2025>> resulttt = vasdfr.Select(x => x.ToDomainAgg());
	//       IEnumerable<Paciente2025> resultt222t = resulttt.Select(x => x.UnwrapAsOk());
	//	return Ok(resultt222t);


	//}



	[HttpGet("{id:int}")]
	public Task<ActionResult<PacienteDbModel?>> GetPacientePorId(int id)
		=> this.SafeExecute(
			logger,
			PermisosAccionesEnum.VerPacientes,
			() => repositorio.SelectPacienteWhereId(new PacienteId(id)),
			notFoundMessage: $"No existe paciente con id {id}"
		);




	[HttpGet("{id}/turnos")]
	public Task<ActionResult<IEnumerable<TurnoDbModel>>> GetTurnosPorPaciente([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			PermisosAccionesEnum.VerTurnos,
			() => repositorio.SelectTurnosWherePacienteId(new PacienteId(id)),
			notFoundMessage: $"No existen turnos con pacienteid {id}"
		);



	[HttpDelete("{id:int}")]
	public Task<ActionResult<Unit>> DeletePaciente(int id)
		=> this.SafeExecute(
			logger,
			PermisosAccionesEnum.DeleteEntidades,
			() => repositorio.DeletePacienteWhereId(new PacienteId(id)),
			notFoundMessage: $"No existe paciente con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<ActionResult<PacienteDbModel>> UpdatePaciente(int id, [FromBody] PacienteDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisosAccionesEnum.UpdatePacientes,
		dto,
		x => x.ToDomain(),
		paciente => repositorio.UpdatePacienteWhereId(new PacienteId(id), paciente),
		notFoundMessage: $"No existe paciente con id {id}"
	);



	[HttpPost]
	public Task<ActionResult<PacienteId>> CrearPaciente([FromBody] PacienteDto dto) {
		Console.WriteLine(dto.ToString());

		return this.SafeExecuteWithDomain(
		   logger,
		   PermisosAccionesEnum.CrearPacientes,
		   dto,
		   x => x.ToDomain(),
		   paciente => repositorio.InsertPacienteReturnId(paciente)
	   );
	}


}