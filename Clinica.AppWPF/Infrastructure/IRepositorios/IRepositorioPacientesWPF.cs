using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure;


public interface IRepositorioPacientesWPF {
	Task EnsurePacientesLoaded();
	Task<ResultWpf<UnitWpf>> DeletePacienteWhereId(PacienteId2025 id);
	Task<ResultWpf<PacienteId2025>> InsertPacienteReturnId(Paciente2025 instance);
	Task<List<PacienteDbModel>> SelectPacientes();
	Task<PacienteDbModel?> SelectPacienteWhereId(PacienteId2025 id);
	Task<ResultWpf<UnitWpf>> UpdatePacienteWhereId(Paciente2025Agg instance);
    Task RefreshCache();
    PacienteDbModel? GetFromCachePacienteWhereId(PacienteId2025 pacienteId);
}
