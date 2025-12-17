using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioPacientes {
	Task<Result<Unit>> DeletePacienteWhereId(PacienteId2025 id);
	Task<Result<PacienteId2025>> InsertPacienteReturnId(Paciente2025 instance);
	Task<Result<IEnumerable<PacienteDbModel>>> SelectPacientes();
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWherePacienteId(PacienteId2025 id);
	Task<Result<PacienteDbModel?>> SelectPacienteWhereId(PacienteId2025 id);
	//Task<Result<PacienteDbModel?>> SelectPacienteWhereTurnoId(TurnoId2025 id);
	Task<Result<PacienteDbModel>> UpdatePacienteWhereId(PacienteId2025 id, Paciente2025 instance);
}
