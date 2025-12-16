using Clinica.AppWPF.Infrastructure.IRepositorios;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public class RepositorioAtencionesWPF : IRepositorioAtencionesWPF {
	Task<ResultWpf<UnitWpf>> IRepositorioAtencionesWPF.AgendarAtencionConDiagnostico(TurnoId turnoSource, PacienteId pacienteId) {
		throw new NotImplementedException();
	}

	Task<List<AtencionDbModel>> IRepositorioAtencionesWPF.SelectAtenciones() {
		throw new NotImplementedException();
	}

	Task<List<AtencionDbModel>> IRepositorioAtencionesWPF.SelectAtencionesWherePacienteId(PacienteId id) {
		throw new NotImplementedException();
	}

	Task<List<TurnoDbModel>> IRepositorioAtencionesWPF.SelectTurnosWhereMedicoId(MedicoId id) {
		throw new NotImplementedException();
	}

	Task<List<TurnoDbModel>> IRepositorioAtencionesWPF.SelectTurnosWhereMedicoIdDeLaFecha(MedicoId id, DateOnly fecha) {
		throw new NotImplementedException();
	}
}
