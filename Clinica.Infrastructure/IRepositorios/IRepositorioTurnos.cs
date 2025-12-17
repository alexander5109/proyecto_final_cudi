using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioTurnos {
	Task<Result<Unit>> DeleteTurnoWhereId(TurnoId2025 id);
	Task<Result<TurnoId2025>> InsertTurnoReturnId(Turno2025 instance);
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnos();
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId2025 id);
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWherePacienteId(PacienteId2025 id);
	Task<Result<TurnoDbModel?>> SelectTurnoWhereId(TurnoId2025 id);
	Task<Result<TurnoDbModel>> UpdateTurnoWhereId(TurnoId2025 id, Turno2025 instance);
}
