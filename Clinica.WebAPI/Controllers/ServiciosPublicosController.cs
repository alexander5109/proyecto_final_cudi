using System.ComponentModel;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeValor;
using Clinica.Shared.ApiDtos;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.ApiDtos.ServiciosPublicosDtos;
namespace Clinica.WebAPI.Controllers;





[ApiController]
[Route("api/[controller]")]
public class ServiciosPublicosController(IRepositorio repositorio, IServiciosPublicos servicios, ILogger<ServiciosPublicosController> logger) : ControllerBase {

	[HttpGet("Turnos/Disponibilidades")]
	public async Task<IActionResult> VerDisponibilidades(
		[FromQuery] EspecialidadCodigo EspecialidadCodigo,
		[FromQuery] int cuantos,
		[FromQuery, DefaultValue("2025-12-03T00:00:00")] DateTime? aPartirDeCuando
	) {
		DateTime desde = aPartirDeCuando ?? DateTime.Now; // default real acá

		Result<IReadOnlyList<Disponibilidad2025>> result = await servicios.SolicitarDisponibilidades(
			EspecialidadCodigo,
			desde,
			cuantos,
			repositorio
		);

		return result switch {
			Result<IReadOnlyList<Disponibilidad2025>>.Ok ok =>
				Ok(ok.Valor),

			Result<IReadOnlyList<Disponibilidad2025>>.Error err =>
				BadRequest(new { error = err.Mensaje }),

			_ => StatusCode(500)
		};
	}






	[HttpPost("Turnos/Programar")]
	public Task<IActionResult> ProgramarTurno([FromBody] ProgramarTurnoDto dto) {
		return this.SafeExecuteApi(
			logger,
			PermisoSistema.GestionDeTurnos,
			operation: async () => (
				await servicios.PersistirProgramarTurnoAsync(
					dto.PacienteId,
					dto.FechaSolicitud,
					dto.Disponibilidad.ToDomain(),
					repositorio
				)
			).ToApi(statusCodeOnError: 409)
		);
	}

	[HttpPut("Turnos/Cancelar")]
	public async Task<ActionResult<TurnoDbModel>> CancelarTurno([FromBody] ModificarTurnoDto dto) {
        Result<Turno2025Agg> result = await servicios.PersistirComoCanceladoAsync(
			dto.TurnoId,
			dto.FechaSolicitud,
			dto.Comentario,
			repositorio
		);

		return result switch {
			Result<Turno2025Agg>.Ok ok => Ok(ok.Valor.ToModel()),
			Result<Turno2025Agg>.Error err => Conflict(new { error = err.Mensaje }),
			_ => StatusCode(500)
		};
	}



	[HttpPut("Turnos/Reprogramar")]
	public Task<IActionResult> ReprogramarTurno([FromBody] ModificarTurnoDto dto) {
		return this.SafeExecuteApi(
			logger,
			PermisoSistema.GestionDeTurnos,
			operation: async () => (
				await servicios.PersistirComoReprogramado(
					dto.TurnoId,
					dto.FechaSolicitud,
					dto.Comentario,
					repositorio
				)
			).ToApi(statusCodeOnError: 409)
		);
	}



	[HttpPut("Turnos/Concretar")]
	public Task<IActionResult> ConcretarTurno([FromBody] ConcretarTurnoDto dto) {
		return this.SafeExecuteApi(
			logger,
			PermisoSistema.GestionDeTurnos,
			operation: async () => (
				await servicios.PersistirComoConcretadoAsync(
					dto.TurnoId,
					dto.FechaSolicitud,
					dto.Comentario,
					repositorio
				)
			).ToApi(statusCodeOnError: 409)
		);
	}


	[HttpPut("Turnos/ConcretarComoAusente")]
	public Task<IActionResult> ConcretarTurno([FromBody] ModificarTurnoDto dto) {
		return this.SafeExecuteApi(
			logger,
			PermisoSistema.GestionDeTurnos,
			operation: async () => (
				await servicios.PersistirComoAusenteAsync(
					dto.TurnoId,
					dto.FechaSolicitud,
					dto.Comentario,
					repositorio
				)
			).ToApi(statusCodeOnError: 409)
		);
	}

}
