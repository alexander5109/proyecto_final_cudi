using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure;



public interface IRepositorioMedicosWPF {
	Task<ResultWpf<UnitWpf>> DeleteMedicoWhereId(MedicoId id);
	Task<ResultWpf<MedicoId>> InsertMedicoReturnId(Medico2025 instance);
	Task<ResultWpf<UnitWpf>> UpdateMedicoWhereId(Medico2025Agg instance);
	Task<List<MedicoDbModel>> SelectMedicos();
	Task EnsureMedicosLoaded();
	Task<List<MedicoDbModel>> SelectMedicosWhereEspecialidadCodigo(EspecialidadEnum code);
	Task<MedicoDbModel?> SelectMedicoWhereId(MedicoId id);
	string GetFromCacheMedicoDisplayWhereId(MedicoId id);
	Task RefreshCache();
}


