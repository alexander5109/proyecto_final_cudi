using System.ComponentModel;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeValor;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.WebAPI.Controllers.ServiciosPublicosControllerDtos;
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




	[HttpPost("Turnos/Cancelar")]
	public Task<IActionResult> CancelarTurno([FromBody] ModificarTurnoDto dto) {
		return this.SafeExecuteApi(
			logger,
			PermisoSistema.GestionDeTurnos,
			operation: async () => (
				await servicios.PersistirComoCanceladoAsync(
					dto.TurnoId,
					dto.FechaSolicitud,
					dto.Comentario,
					repositorio
				)
			).ToApi(statusCodeOnError: 409)
		);
	}



	[HttpPost("Turnos/Reprogramar")]
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



	[HttpPost("Turnos/Concretar")]
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


	[HttpPost("Turnos/ConcretarComoAusente")]
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
