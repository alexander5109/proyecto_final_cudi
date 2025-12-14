using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioMedicos {
	Task<Result<Unit>> DeleteMedicoWhereId(MedicoId id);
	Task<Result<MedicoId>> InsertMedicoReturnId(Medico2025 instance);
	Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicos();
	//Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicosWithHorarios();
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId id);
	Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicosWhereEspecialidadCodigo(EspecialidadEnumCodigo code);
	Task<Result<MedicoDbModel?>> SelectMedicoWhereId(MedicoId id);
	//Task<Result<MedicoDbModel?>> SelectMedicoWithHorarioWhereId(MedicoId id);
	Task<Result<MedicoDbModel>> UpdateMedicoWhereId(MedicoId id, Medico2025 instance);
}
