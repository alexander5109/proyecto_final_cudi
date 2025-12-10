using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioHorarios {
	Task<Result<Unit>> DeleteHorarioWhereId(HorarioId id);
	Task<Result<HorarioId>> InsertHorarioReturnId(Horario2025 instance);
	Task<Result<IEnumerable<HorarioDbModel>>> SelectHorarios();
	Task<Result<HorarioDbModel?>> SelectHorarioWhereId(HorarioId id);
	Task<Result<Unit>> UpdateHorarioWhereId(HorarioId id, Horario2025 instance);
}
