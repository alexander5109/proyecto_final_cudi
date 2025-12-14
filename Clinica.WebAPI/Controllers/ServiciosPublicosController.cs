using Clinica.Dominio.FunctionalToolkit;
using Clinica.Shared.ApiDtos;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.ApiDtos.ServiciosPublicosDtos;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeEntidad;
using static Clinica.Shared.ApiDtos.TurnoDtos;
using Clinica.Dominio.TiposDeAgregado;
namespace Clinica.WebAPI.Controllers;





[ApiController]
[Route("api/[controller]")]
public class ServiciosPublicosController(
	IRepositorioDominioServices repositorio,
	IServiciosDeDominio servicios,
	ILogger<ServiciosPublicosController> logger
) : ControllerBase {

	[HttpGet("Turnos/Disponibilidades")]
	public async Task<IActionResult> VerDisponibilidades(
		[FromQuery] SolicitarDisponibilidadesDto dto
	) {
		DateTime desde = dto.APartirDeCuando ?? DateTime.Now;

		Result<IReadOnlyList<Disponibilidad2025>> result =
			await servicios.SolicitarDisponibilidades(
				dto.EspecialidadCodigo,
				desde,
				dto.Cuantos,
				dto.DiaSemanaPreferido,
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
	public Task<ActionResult<Turno2025Agg>> ProgramarTurno(
		[FromBody] ProgramarTurnoDto dto
	) {
		return this.SafeExecuteApi(
			logger,
			PermisosAccionesEnum.GestionDeTurnos,
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
	public async Task<ActionResult<TurnoDto>> CancelarTurno(
		[FromBody] ModificarTurnoDto dto
	) {
		Result<Turno2025> result = await servicios.PersistirComoCanceladoAsync(
			dto.TurnoId,
			dto.FechaSolicitud,
			dto.Comentario,
			repositorio
		);

		return result switch {
			Result<Turno2025>.Ok ok => Ok(ok.Valor.ToDto()),
			Result<Turno2025>.Error err => Conflict(new { error = err.Mensaje }),
			_ => StatusCode(500)
		};
	}



	[HttpPut("Turnos/Reprogramar")]
	public Task<ActionResult<Turno2025>> ReprogramarTurno(
		[FromBody] ModificarTurnoDto dto
	) {
		//if (dto.Comentario is null) {
		//	return new BadRequestObjectResult("El comentario es obligatorio");
		//}
		return this.SafeExecuteApi(
			logger,
			PermisosAccionesEnum.GestionDeTurnos,
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
	public Task<ActionResult<Turno2025>> ConcretarTurno(
		[FromBody] ConcretarTurnoDto dto
	) {
		return this.SafeExecuteApi(
			logger,
			PermisosAccionesEnum.GestionDeTurnos,
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
	public Task<ActionResult<Turno2025>> ConcretarTurno(
		[FromBody] ModificarTurnoDto dto
	) {
		return this.SafeExecuteApi(
			logger,
			PermisosAccionesEnum.GestionDeTurnos,
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
