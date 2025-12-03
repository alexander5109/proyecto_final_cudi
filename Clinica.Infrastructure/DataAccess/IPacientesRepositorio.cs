using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Shared.Dtos;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.Infrastructure.DataAccess;

public interface IPacientesRepositorio {
    Task<Result<Unit>> DeletePacienteWhereId(PacienteId id);
    Task<Result<PacienteId>> InsertPacienteReturnId(Paciente2025 paciente);
    Task<Result<IEnumerable<PacienteDbModel>>> SelectPacientes();
    Task<Result<PacienteDbModel?>> SelectPacienteWhereId(PacienteId id);
    Task<Result<Unit>> UpdatePacienteWhereId(Paciente2025 paciente);
}
public interface IHorarioMedicosRepositorio {
	Task<Result<Unit>> DeleteHorarioMedicoWhereId(HorarioId id);
	Task<Result<HorarioId>> InsertHorarioMedicoReturnId(HorarioMedico2025 HorarioMedico);
	Task<Result<IEnumerable<HorarioMedicoDbModel>>> SelectHorarioMedicos();
	Task<Result<HorarioMedicoDbModel?>> SelectHorarioMedicoWhereId(HorarioId id);
	Task<Result<Unit>> UpdateHorarioMedicoWhereId(HorarioMedico2025 paciente);
}

public interface IMedicosRepositorio {
    Task<Result<Unit>> DeleteMedicoWhereId(MedicoId id);
    Task<Result<MedicoId>> InsertMedicoReturnId(Medico2025 Medico);
    Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicos();
    Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicosWhereEspecialidadCode(EspecialidadCodigo2025 code);
    Task<Result<MedicoDbModel?>> SelectMedicoWhereId(MedicoId id);
    Task<Result<Unit>> UpdateMedicoWhereId(Medico2025 medico);
}

public interface ITurnosRepositorio {
    Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id);
    Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 Turno);
    Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnos();
    Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId id);
    Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWherePacienteId(PacienteId id);
    Task<Result<TurnoDbModel?>> SelectTurnoWhereId(TurnoId id);
    Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 Turno);
}