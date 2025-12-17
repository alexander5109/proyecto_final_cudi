using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioAtenciones {
	Task<Result<IEnumerable<AtencionDbModel>>> SelectAtenciones();
	Task<Result<IEnumerable<AtencionDbModel>>> SelectAtencionesWherePacienteId(PacienteId2025 id);
	Task<Result<IEnumerable<AtencionDbModel>>> SelectAtencionesWhereMedicoId(MedicoId2025 id);
	Task<Result<AtencionDbModel?>> SelectAtencionWhereTurnoId(TurnoId2025 id);
	Task<Result<AtencionId2025>> InsertAtencionReturnId(Atencion2025 instance);
	Task<Result<Unit>> UpdateObservacionesWhereId(AtencionId2025 id, string observaciones);
	Task<Result<Unit>> DeleteAtencionWhereId(AtencionId2025 id);
}
