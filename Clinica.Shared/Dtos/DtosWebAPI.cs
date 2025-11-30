using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.Dtos;

public static class ApiDtos {
	public record ReprogramarTurnoRequestDto(
		DateTime NuevaFechaDesde,
		DateTime NuevaFechaHasta
	);
	public record LoginRequestDto(string Username, string Password);
	public record LoginResponseDto(string Nombre, string Rol, string Token);


	public record CrearTurnoRequestDto(
		int PacienteId,
		int MedicoId,
		int EspecialidadCodigo,
		DateTime Desde,
		DateTime Hasta
	);

	public record CancelarTurnoRequest(
		DateTime OutcomeFecha,
		string OutcomeComentario
	);

	public record DisponibilidadDTO(
		int MedicoId,
		DateTime FechaInicio,
		DateTime FechaFin,
		int EspecialidadCodigoInterno
	);
	public static DisponibilidadDTO ToDto(this DisponibilidadEspecialidad2025 d) => new(d.MedicoId.Valor,
		d.FechaHoraDesde,
		d.FechaHoraHasta,
		d.Especialidad.CodigoInterno.Valor
	);


	public record ReprogramarTurnoRequest(
		DateTime OutcomeFecha,
		string OutcomeComentario
	);

	public record SolicitarTurnoRequest(
		int PacienteId,
		string Especialidad   // string para no acoplar la API al enum interno
	);

}