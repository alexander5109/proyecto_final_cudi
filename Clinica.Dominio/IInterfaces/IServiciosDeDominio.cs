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
		MedicoId2025? medicoPreferido,
		IRepositorioDominioServices repositorio
	);
	Task<Result<Turno2025Agg>> PersistirProgramarTurnoAsync(PacienteId2025 pacienteId, DateTime fechaSolicitud, Disponibilidad2025 disponibilidad, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025>> PersistirComoReprogramado(TurnoId2025 turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDominioServices repositorio);

	Task<Result<Turno2025>> PersistirComoCanceladoAsync(TurnoId2025 turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025>> PersistirComoAusenteAsync(TurnoId2025 turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025>> PersistirComoConcretadoAsync(TurnoId2025 turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDominioServices repositorio);
}
		