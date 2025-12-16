using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure;


public interface IRepositorioPacientesWPF {
	Task EnsurePacientesLoaded();
	Task<ResultWpf<UnitWpf>> DeletePacienteWhereId(PacienteId id);
	Task<ResultWpf<PacienteId>> InsertPacienteReturnId(Paciente2025 instance);
	Task<List<PacienteDbModel>> SelectPacientes();
	Task<PacienteDbModel?> SelectPacienteWhereId(PacienteId id);
	Task<ResultWpf<UnitWpf>> UpdatePacienteWhereId(Paciente2025Agg instance);
    Task RefreshCache();
    PacienteDbModel? GetFromCachePacienteWhereId(PacienteId pacienteId);
}
