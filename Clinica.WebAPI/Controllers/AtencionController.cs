using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Infrastructure.IRepositorios;
using Clinica.Shared.ApiDtos;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AtencionesController(
	IRepositorioAtenciones repositorio,
	ILogger<AtencionesController> logger
) : ControllerBase {
	// GET: /api/Atenciones
	[HttpGet]
	public Task<ActionResult<IEnumerable<AtencionDbModel>>> GetAtenciones()
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerAtenciones,
			() => repositorio.SelectAtenciones()
		);

	// GET: /api/Atenciones/{id}
	[HttpGet("{id:int}")]
	public Task<ActionResult<AtencionDbModel?>> GetAtencionPorId(int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerAtenciones,
			() => repositorio.SelectAtencionWhereTurnoId(TurnoId2025.Crear(id)),
			notFoundMessage: $"No existe atención para el turno con id {id}"
		);

	// GET: /api/Atenciones/paciente/{id}
	[HttpGet("paciente/{id:int}")]
	public Task<ActionResult<IEnumerable<AtencionDbModel>>> GetAtencionesPorPaciente([FromRoute] int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.VerAtenciones,
			() => repositorio.SelectAtencionesWherePacienteId(PacienteId2025.Crear(id)),
			notFoundMessage: $"No existen atenciones para el paciente con id {id}"
		);

	// POST: /api/Atenciones
	[HttpPost]
	public Task<ActionResult<AtencionId2025>> CrearAtencion([FromBody] AtencionDto dto)
		=> this.SafeExecuteWithDomain(
			logger,
			AccionesDeUsuarioEnum.CrearAtenciones,
			dto,
			x => x.ToDomain(),
			atencion => repositorio.InsertAtencionReturnId(atencion)
		);

	// PUT: /api/Atenciones/{id}
	//[HttpPut("{id:int}")]
	//public Task<ActionResult<Unit>> ModificarObservaciones(int id, [FromBody] ModificarObservacionDto dto)
	//	=> this.SafeExecuteWithDomain(
	//		logger,
	//		AccionesDeUsuarioEnum.ModificarAtenciones,
	//		dto,
	//		x => x.Observaciones,
	//		obs => repositorio.UpdateObservacionesWhereId(AtencionId2025.Crear(id), obs),
	//		notFoundMessage: $"No existe atención con id {id}"
	//	);

	// DELETE: /api/Atenciones/{id}
	[HttpDelete("{id:int}")]
	public Task<ActionResult<Unit>> DeleteAtencion(int id)
		=> this.SafeExecute(
			logger,
			AccionesDeUsuarioEnum.EliminarEntidades,
			() => repositorio.DeleteAtencionWhereId(AtencionId2025.Crear(id)),
			notFoundMessage: $"No existe atención con id {id}"
		);
}
