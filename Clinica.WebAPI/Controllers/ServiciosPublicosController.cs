using System.ComponentModel;
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
namespace Clinica.WebAPI.Controllers;





[ApiController]
[Route("api/[controller]")]
public class ServiciosPublicosController(IRepositorioDominioServices repositorio, IServiciosDeDominio servicios, ILogger<ServiciosPublicosController> logger) : ControllerBase {

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
	public async Task<ActionResult<TurnoDto>> CancelarTurno([FromBody] ModificarTurnoDto dto) {
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
