using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.IRepositorios;

public record AtencionDbModel(
	AtencionId Id,
	TurnoId TurnoId,
	PacienteId PacienteId,
	DateTime Fecha,
	string Observaciones
);


public interface IRepositorioAtencionesWPF {
	Task<List<AtencionDbModel>> SelectAtenciones();
	Task<List<AtencionDbModel>> SelectAtencionesWherePacienteId(PacienteId id);
	Task<List<TurnoDbModel>> SelectTurnosWhereMedicoId(MedicoId id);
	Task<List<TurnoDbModel>> SelectTurnosWhereMedicoIdDeLaFecha(MedicoId id, DateOnly fecha);
	Task<ResultWpf<UnitWpf>> AgendarAtencionConDiagnostico(TurnoId turnoSource, PacienteId pacienteId);

	//string GetFromCacheMedicoDisplayWhereId(MedicoId id);
	//App.Repositorio.Medicos.GetFromCacheMedicoDisplayWhereId(d.MedicoId);
}


