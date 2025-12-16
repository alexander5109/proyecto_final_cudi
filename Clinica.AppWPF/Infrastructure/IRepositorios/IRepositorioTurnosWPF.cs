using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.IRepositorios;

public interface IRepositorioTurnosWPF {
	Task<List<TurnoDbModel>> SelectTurnos();
	Task<List<TurnoDbModel>> SelectTurnosWherePacienteId(PacienteId id);
	Task<List<TurnoDbModel>> SelectTurnosWhereMedicoId(MedicoId id);
	Task<ResultWpf<UnitWpf>> AgendarNuevoTurno(PacienteId pacienteId, DateTime fechaSolicitudOriginal, Disponibilidad2025 disponibilidad);
	Task<ResultWpf<UnitWpf>> CancelarTurno(TurnoId turnoId, DateTime fechaOutcome, string? reason);
	Task<ResultWpf<UnitWpf>> ReprogramarTurno(TurnoId turnoId, DateTime fechaOutcome, string? reason);
	Task<ResultWpf<UnitWpf>> MarcarTurnoComoAusente(TurnoId turnoId, DateTime fechaOutcome, string? reason);
	Task<ResultWpf<UnitWpf>> MarcarTurnoComoConcretado(TurnoId turnoId, DateTime fechaOutcome, string? reason);
}


