using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.DataAccess;

public static partial class IRepositorioInterfaces {
    public interface IRepositorioMedicos {
		Task<Result<Unit>> DeleteMedicoWhereId(MedicoId id);
		Task<Result<MedicoId>> InsertMedicoReturnId(Medico2025 instance);
		Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicos();
		Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId id);
		Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicosWhereEspecialidadCodigo(EspecialidadCodigo code);
		Task<Result<MedicoDbModel?>> SelectMedicoWhereId(MedicoId id);
		Task<Result<Unit>> UpdateMedicoWhereId(MedicoId id, Medico2025 instance);
	}
}
