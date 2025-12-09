using Clinica.Dominio.Entidades;
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
public class PacientesController(
	IRepositorioPacientes repositorio, 
	ILogger<PacientesController> logger
) : ControllerBase {


	[HttpGet]
	public Task<IActionResult> GetPacientes()
	=> this.SafeExecute(
		logger,
		PermisoSistema.VerPacientes,
		() => repositorio.SelectPacientes()
	);


	//tonto.
	//[HttpGet("por-turnos/{id:int}")]
	//public Task<IActionResult> GetPacientePorTurnoId(int id)
	//	=> this.SafeExecute(
	//		logger,
	//		PermisoSistema.VerPacientes,
	//		() => repositorio.SelectPacienteWhereTurnoId(new TurnoId(id)),
	//		notFoundMessage: $"No existe paciente con id {id}"
	//	);


	//[HttpGet("AsDomain")]
	//public async Task<IActionResult> GetPacientesAsDomain() {


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
	//       IEnumerable<Result<Paciente2025>> resulttt = vasdfr.Select(x => x.ToDomain());
	//       IEnumerable<Paciente2025> resultt222t = resulttt.Select(x => x.UnwrapAsOk());
	//	return Ok(resultt222t);


	//}



	[HttpGet("{id:int}")]
	public Task<IActionResult> GetPacientePorId(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerPacientes,
			() => repositorio.SelectPacienteWhereId(new PacienteId(id)),
			notFoundMessage: $"No existe paciente con id {id}"
		);




	[HttpGet("{id}/turnos")]
	public Task<IActionResult> GetTurnosPorPaciente([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.VerTurnos,
			() => repositorio.SelectTurnosWherePacienteId(new PacienteId(id)),
			notFoundMessage: $"No existen turnos con pacienteid {id}"
		);



	[HttpDelete("{id:int}")]
	public Task<IActionResult> DeletePaciente(int id)
		=> this.SafeExecute(
			logger,
			PermisoSistema.DeleteEntidades,
			() => repositorio.DeletePacienteWhereId(new PacienteId(id)),
			notFoundMessage: $"No existe paciente con id {id}"
		);



	[HttpPut("{id:int}")]
	public Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteApiDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.UpdatePacientes,
		dto,
		x => x.ToDomain(),
		paciente => repositorio.UpdatePacienteWhereId(new PacienteId(id), paciente),
		notFoundMessage: $"No existe paciente con id {id}"
	);



	[HttpPost]
	public Task<IActionResult> CrearPaciente([FromBody] PacienteApiDto dto)
	=> this.SafeExecuteWithDomain(
		logger,
		PermisoSistema.CrearPacientes,
		dto,
		x => x.ToDomain(),
		paciente => repositorio.InsertPacienteReturnId(paciente)
	);



}