using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.IRepositorios;



public interface IRepositorioAtencionesWPF {
	Task<List<AtencionDbModel>> SelectAtenciones();
	Task<List<AtencionDbModel>> SelectAtencionesWherePacienteId(PacienteId2025 id);
	Task<List<TurnoDbModel>> SelectTurnosWhereMedicoId(MedicoId2025 id);
	Task<List<TurnoDbModel>> SelectTurnosWhereMedicoIdDeLaFecha(MedicoId2025 id, DateOnly fecha);
	Task<ResultWpf<UnitWpf>> AgendarAtencionConDiagnostico(AtencionDto atencionDto);

	//string GetFromCacheMedicoDisplayWhereId(MedicoId2025 id);
	//App.Repositorio.Medicos.GetFromCacheMedicoDisplayWhereId(d.MedicoId2025);
}


