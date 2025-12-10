using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.ApiDtos.TurnoDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioTurnos {
	Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id);
	Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 instance);
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnos();
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId id);
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWherePacienteId(PacienteId id);
	Task<Result<TurnoDbModel?>> SelectTurnoWhereId(TurnoId id);
	Task<Result<TurnoDbModel>> UpdateTurnoWhereId(TurnoId id, Turno2025 instance);
}
