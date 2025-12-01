using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Dtos;

public static class ApiDtos {

	public record PacienteListDto(
		PacienteId Id,
		string Dni,
		string Nombre,
		string Apellido,
		string Email,
		string Telefono
	);
	public record TurnoListDto(
		TurnoId Id,
		TimeSpan Hora,
		DateTime Fecha,
		EspecialidadCodigo2025 EspecialidadCodigoInterno,
		TurnoOutcomeEstadoCodigo2025 Estado,
		MedicoId MedicoId
	);

	public record MedicoListDto(
		string Dni,
		string Nombre,
		string Apellido,
		EspecialidadCodigo2025 EspecialidadCodigoInterno
	);











	public record ReprogramarTurnoRequestDto(
		DateTime NuevaFechaDesde,
		DateTime NuevaFechaHasta
	);
	public record LoginRequestDto(string Username, string Password);
	public record LoginResponseDto(string Nombre, string Rol, string Token);


	public record CrearTurnoRequestDto(
		PacienteId PacienteId,
		MedicoId MedicoId,
		EspecialidadCodigo2025 EspecialidadCodigo,
		DateTime Desde,
		DateTime Hasta
	);

	public record CancelarTurnoRequest(
		DateTime OutcomeFecha,
		string OutcomeComentario
	);

	public record DisponibilidadDTO(
		MedicoId MedicoId,
		DateTime FechaInicio,
		DateTime FechaFin,
		EspecialidadCodigo2025 EspecialidadCodigoInterno
	);


	public record ReprogramarTurnoRequest(
		DateTime OutcomeFecha,
		string OutcomeComentario
	);

	public record SolicitarTurnoRequest(
		PacienteId PacienteId,
		string Especialidad   // string para no acoplar la API al enum interno
	);

}
