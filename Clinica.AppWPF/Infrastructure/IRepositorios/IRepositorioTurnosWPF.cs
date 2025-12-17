using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.IRepositorios;

public interface IRepositorioTurnosWPF {
	Task<List<TurnoDbModel>> SelectTurnos();
	Task<List<TurnoDbModel>> SelectTurnosWherePacienteId(PacienteId2025 id);
	Task<List<TurnoDbModel>> SelectTurnosWhereMedicoId(MedicoId2025 id);
	Task<ResultWpf<UnitWpf>> AgendarNuevoTurno(PacienteId2025 pacienteId, DateTime fechaSolicitudOriginal, Disponibilidad2025 disponibilidad);
	Task<ResultWpf<UnitWpf>> CancelarTurno(TurnoId2025 turnoId, DateTime fechaOutcome, string? reason);
	Task<ResultWpf<UnitWpf>> ReprogramarTurno(TurnoId2025 turnoId, DateTime fechaOutcome, string? reason);
	Task<ResultWpf<UnitWpf>> MarcarTurnoComoAusente(TurnoId2025 turnoId, DateTime fechaOutcome, string? reason);
	Task<ResultWpf<UnitWpf>> MarcarTurnoComoConcretado(TurnoId2025 turnoId, DateTime fechaOutcome, string? reason);
}


