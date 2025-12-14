using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.IInterfaces;



public interface IServiciosDeDominio {
	Task<Result<IReadOnlyList<Disponibilidad2025>>> SolicitarDisponibilidades(
		EspecialidadEnum solicitudEspecialidadCodigo, 
		DateTime aPartirDeCuando, 
		int cuantos,
		DayOfWeek? diaSemanaPreferido,
		IRepositorioDominioServices repositorio
	);
	Task<Result<Turno2025Agg>> PersistirProgramarTurnoAsync(PacienteId pacienteId, DateTime fechaSolicitud, Disponibilidad2025 disponibilidad, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025>> PersistirComoReprogramado(TurnoId turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDominioServices repositorio);

	Task<Result<Turno2025>> PersistirComoCanceladoAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025>> PersistirComoAusenteAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025>> PersistirComoConcretadoAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDominioServices repositorio);
}