using Clinica.AppWPF.Infrastructure.IRepositorios;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.DBModels;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public class RepositorioAtencionesWPF : IRepositorioAtencionesWPF {
	Task<ResultWpf<UnitWpf>> IRepositorioAtencionesWPF.AgendarAtencionConDiagnostico(TurnoId2025 turnoSource, PacienteId2025 pacienteId) {
		throw new NotImplementedException();
	}

	Task<List<AtencionDbModel>> IRepositorioAtencionesWPF.SelectAtenciones() {
		throw new NotImplementedException();
	}

    Task<List<AtencionDbModel>> IRepositorioAtencionesWPF.SelectAtencionesWherePacienteId(PacienteId2025 id) {
		throw new NotImplementedException();
	}

    Task<List<TurnoDbModel>> IRepositorioAtencionesWPF.SelectTurnosWhereMedicoId(MedicoId2025 id) {
		throw new NotImplementedException();
	}

	Task<List<TurnoDbModel>> IRepositorioAtencionesWPF.SelectTurnosWhereMedicoIdDeLaFecha(MedicoId2025 id, DateOnly fecha) {
		throw new NotImplementedException();
	}
}
