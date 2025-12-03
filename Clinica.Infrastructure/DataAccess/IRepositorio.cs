using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.Infrastructure.DataAccess;

public static class IRepositorioInterfaces {
	public interface IRepositorio :
		IRepositorioDomain,
		IRepositorioUsuarios,
		IRepositorioTurnos,
		IRepositorioPacientes,
		IRepositorioMedicos {
	}

	public interface IRepositorioPacientes {
		Task<Result<Unit>> DeletePacienteWhereId(PacienteId id);
		Task<Result<PacienteId>> InsertPacienteReturnId(Paciente2025 instance);
		Task<Result<IEnumerable<PacienteDbModel>>> SelectPacientes();
		Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWherePacienteId(PacienteId id);
		Task<Result<PacienteDbModel?>> SelectPacienteWhereId(PacienteId id);
		Task<Result<Unit>> UpdatePacienteWhereId(Paciente2025 instance);
	}

	public interface IRepositorioMedicos {
		Task<Result<Unit>> DeleteMedicoWhereId(MedicoId id);
		Task<Result<MedicoId>> InsertMedicoReturnId(Medico2025 instance);
		Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicos();
		Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId id);
		Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicosWhereEspecialidadCode(EspecialidadCodigo2025 code);
		Task<Result<MedicoDbModel?>> SelectMedicoWhereId(MedicoId id);
		Task<Result<Unit>> UpdateMedicoWhereId(Medico2025 instance);
	}

	public interface IRepositorioTurnos {
		Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id);
		Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 instance);
		Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnos();
		Task<Result<TurnoDbModel?>> SelectTurnoWhereId(TurnoId id);
		Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 instance);
	}
	public interface IRepositorioUsuarios {
		Task<Result<Unit>> DeleteUsuarioWhereId(UsuarioId id);
		Task<Result<UsuarioId>> InsertUsuarioReturnId(Usuario2025 instance);
		Task<Result<IEnumerable<UsuarioDbModel>>> SelectUsuarios();
		Task<Result<UsuarioDbModel?>> SelectUsuarioWhereId(UsuarioId id);
		Task<Result<Unit>> UpdateUsuarioWhereId(Usuario2025 instance);
	}


	public interface IRepositorioHorarioMedicos {
		Task<Result<Unit>> DeleteHorarioMedicoWhereId(HorarioId id);
		Task<Result<HorarioId>> InsertHorarioMedicoReturnId(HorarioMedico2025 instance);
		Task<Result<IEnumerable<HorarioMedicoDbModel>>> SelectHorarioMedicos();
		Task<Result<HorarioMedicoDbModel?>> SelectHorarioMedicoWhereId(HorarioId id);
		Task<Result<Unit>> UpdateHorarioMedicoWhereId(HorarioMedico2025 instance);
	}
}
