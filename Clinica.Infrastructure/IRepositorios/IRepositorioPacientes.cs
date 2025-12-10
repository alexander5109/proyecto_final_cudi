using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioPacientes {
	Task<Result<Unit>> DeletePacienteWhereId(PacienteId id);
	Task<Result<PacienteId>> InsertPacienteReturnId(Paciente2025 instance);
	Task<Result<IEnumerable<PacienteDbModel>>> SelectPacientes();
	Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWherePacienteId(PacienteId id);
	Task<Result<PacienteDbModel?>> SelectPacienteWhereId(PacienteId id);
	//Task<Result<PacienteDbModel?>> SelectPacienteWhereTurnoId(TurnoId id);
	Task<Result<PacienteDto>> UpdatePacienteWhereId(PacienteId id, Paciente2025 instance);
}
