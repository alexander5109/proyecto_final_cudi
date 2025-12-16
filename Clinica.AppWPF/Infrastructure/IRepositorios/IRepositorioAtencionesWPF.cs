using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.IRepositorios;



public interface IRepositorioAtencionesWPF {
	Task<List<AtencionDbModel>> SelectAtenciones();
	Task<List<AtencionDbModel>> SelectAtencionesWherePacienteId(PacienteId id);
	Task<List<TurnoDbModel>> SelectTurnosWhereMedicoId(MedicoId id);
	Task<List<TurnoDbModel>> SelectTurnosWhereMedicoIdDeLaFecha(MedicoId id, DateOnly fecha);
	Task<ResultWpf<UnitWpf>> AgendarAtencionConDiagnostico(TurnoId turnoSource, PacienteId pacienteId);

	//string GetFromCacheMedicoDisplayWhereId(MedicoId id);
	//App.Repositorio.Medicos.GetFromCacheMedicoDisplayWhereId(d.MedicoId);
}


