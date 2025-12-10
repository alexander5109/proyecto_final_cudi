using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.IInterfaces;



public interface IServiciosDeDominio {
	Task<Result<IReadOnlyList<Disponibilidad2025>>> SolicitarDisponibilidades(EspecialidadCodigo solicitudEspecialidadCodigo, DateTime aPartirDeCuando, int cuantos, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025Agg>> PersistirProgramarTurnoAsync(PacienteId pacienteId, DateTime fechaSolicitud, Disponibilidad2025 disponibilidad, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025Agg>> PersistirComoReprogramado(TurnoId turnoOriginalId, DateTime outcomeFecha, string outcomeComentario, IRepositorioDominioServices repositorio);

	Task<Result<Turno2025Agg>> PersistirComoCanceladoAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string outcomeComentario, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025Agg>> PersistirComoAusenteAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string outcomeComentario, IRepositorioDominioServices repositorio);
	Task<Result<Turno2025Agg>> PersistirComoConcretadoAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDominioServices repositorio);
}