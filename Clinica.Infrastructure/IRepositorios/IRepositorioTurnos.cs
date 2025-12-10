using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.DataAccess;

public static partial class IRepositorioInterfaces {
    public interface IRepositorioTurnos {
		Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id);
		Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 instance);
		Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnos();
		Task<Result<TurnoDbModel?>> SelectTurnoWhereId(TurnoId id);
		Task<Result<Turno2025Agg>> UpdateTurnoWhereId(TurnoId id, Turno2025 instance);
	}
}
