using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioAtenciones {
	Task<Result<IEnumerable<AtencionDbModel>>> SelectAtenciones();
	Task<Result<IEnumerable<AtencionDbModel>>> SelectAtencionesWherePacienteId(PacienteId id);
	Task<Result<AtencionDbModel?>> SelectAtencionWhereTurnoId(TurnoId id);
	Task<Result<AtencionId>> InsertAtencionReturnId(AtencionDbModel instance);
	Task<Result<Unit>> UpdateObservacionesWhereId(AtencionId id, string observaciones);
	Task<Result<Unit>> DeleteAtencionWhereId(AtencionId id);
}
