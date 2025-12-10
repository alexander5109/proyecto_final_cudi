using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioTurnos {
	Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id);
	Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 instance);
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnos();
	Task<Result<TurnoDbModel?>> SelectTurnoWhereId(TurnoId id);
	Task<Result<Turno2025>> UpdateTurnoWhereId(TurnoId id, Turno2025 instance);
}
