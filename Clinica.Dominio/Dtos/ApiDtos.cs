using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Dtos;

public static class ApiDtos {

	public record PacienteListDto(
		int Id,
		string Dni,
		string Nombre,
		string Apellido,
		string Email,
		string Telefono
	);
	public record TurnoListDto(
		int Id,
		TimeSpan Hora,
		DateTime Fecha,
		byte EspecialidadCodigoInterno,
		byte Estado,
		int MedicoId
	);

	public record MedicoListDto(
		string Dni,
		string Nombre,
		string Apellido,
		byte EspecialidadCodigoInterno
	);











	public record ReprogramarTurnoRequestDto(
		DateTime NuevaFechaDesde,
		DateTime NuevaFechaHasta
	);
	public record LoginRequestDto(string Username, string Password);
	public record LoginResponseDto(string Nombre, string Rol, string Token);


	public record CrearTurnoRequestDto(
		int PacienteId,
		int MedicoId,
		EspecialidadCodigo2025 EspecialidadCodigo,
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
		EspecialidadCodigo2025 EspecialidadCodigoInterno
	);
	public static DisponibilidadDTO ToDto(this DisponibilidadEspecialidad2025 d) => new(d.MedicoId.Valor,
		d.FechaHoraDesde,
		d.FechaHoraHasta,
		d.Especialidad.CodigoInternoValor
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
