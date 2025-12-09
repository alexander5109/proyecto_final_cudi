namespace Clinica.WebAPI.Controllers;



public static class ServiciosPublicosControllerDtos {

	public record ProgramarTurnoDto(
		int PacienteId,
		int DisponibilidadId,
		DateTime FechaSolicitud
	);

	public record OutcomeTurnoDto(
		int TurnoId,
		DateTime Fecha,
		string? Comentario
	);
}
