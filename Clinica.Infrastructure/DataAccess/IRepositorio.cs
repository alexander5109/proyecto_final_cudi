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
		public Task<Result<Unit>> DeletePacienteWhereId(PacienteId id);
		public Task<Result<PacienteId>> InsertPacienteReturnId(Paciente2025 instance);
		public Task<Result<IEnumerable<PacienteDbModel>>> SelectPacientes();
		public Task<Result<PacienteDbModel?>> SelectPacienteWhereId(PacienteId id);
		public Task<Result<Unit>> UpdatePacienteWhereId(Paciente2025 instance);
	}

	public interface IRepositorioMedicos {
		public Task<Result<Unit>> DeleteMedicoWhereId(MedicoId id);
		public Task<Result<MedicoId>> InsertMedicoReturnId(Medico2025 instance);
		public Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicos();
		public Task<Result<IEnumerable<MedicoDbModel>>> SelectMedicosWhereEspecialidadCode(EspecialidadCodigo2025 code);
		public Task<Result<MedicoDbModel?>> SelectMedicoWhereId(MedicoId id);
		public Task<Result<Unit>> UpdateMedicoWhereId(Medico2025 instance);
	}

	public interface IRepositorioTurnos {
		public Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id);
		public Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 instance);
		public Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnos();
		public Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId id);
		public Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWherePacienteId(PacienteId id);
		public Task<Result<TurnoDbModel?>> SelectTurnoWhereId(TurnoId id);
		public Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 instance);
	}
	public interface IRepositorioUsuarios {
		public Task<Result<Unit>> DeleteUsuarioWhereId(UsuarioId id);
		public Task<Result<UsuarioId>> InsertUsuarioReturnId(Usuario2025 instance);
		public Task<Result<IEnumerable<UsuarioDbModel>>> SelectUsuarios();
		public Task<Result<UsuarioDbModel?>> SelectUsuarioWhereId(UsuarioId id);
		public Task<Result<Unit>> UpdateUsuarioWhereId(Usuario2025 instance);
	}




	public interface IRepositorioHorarioMedicos {
		Task<Result<Unit>> DeleteHorarioMedicoWhereId(HorarioId id);
		Task<Result<HorarioId>> InsertHorarioMedicoReturnId(HorarioMedico2025 instance);
		Task<Result<IEnumerable<HorarioMedicoDbModel>>> SelectHorarioMedicos();
		Task<Result<HorarioMedicoDbModel?>> SelectHorarioMedicoWhereId(HorarioId id);
		Task<Result<Unit>> UpdateHorarioMedicoWhereId(HorarioMedico2025 instance);
	}
}
