using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioMedicos {
	Task<Result<Unit>> DeleteMedicoWhereId(MedicoId2025 id);
	Task<Result<MedicoId2025>> InsertMedicoReturnId(Medico2025 instance);
	Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicos();
	//Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicosWithHorarios();
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId2025 id);
	Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicosWhereEspecialidadCodigo(EspecialidadEnum code);
	Task<Result<MedicoDbModel?>> SelectMedicoWhereId(MedicoId2025 id);
	//Task<Result<MedicoDbModel?>> SelectMedicoWithHorarioWhereId(MedicoId2025 id);
	Task<Result<MedicoDbModel>> UpdateMedicoWhereId(MedicoId2025 id, Medico2025 instance);
}
