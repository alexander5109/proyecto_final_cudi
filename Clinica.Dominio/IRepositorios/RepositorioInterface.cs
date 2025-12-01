using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.Dtos.DomainDtos;
using static Clinica.Dominio.Dtos.ApiDtos;

namespace Clinica.Dominio.IRepositorios;

public interface RepositorioInterface {
	// ----------------SELECT *
	Task<Result<IEnumerable<Turno2025>>> SelectTurnos();
	Task<Result<IEnumerable<Turno2025>>> SelectTurnosWherePacienteId(PacienteId id);
	Task<Result<IEnumerable<Turno2025>>> SelectTurnosWhereMedicoId(MedicoId id);
	Task<Result<IEnumerable<Turno2025>>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<IEnumerable<Paciente2025>>> SelectPacientes();
	Task<Result<IEnumerable<Medico2025>>> SelectMedicos();
	Task<Result<IEnumerable<Medico2025>>> SelectMedicosWhereEspecialidadCode(EspecialidadCodigo2025 code);
	Task<Result<IEnumerable<HorarioMedico2025>>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);

	// ----------------SELECT ONE
	Task<Result<UsuarioBase2025>> SelectUsuarioWhereName(NombreUsuario nombre);
	Task<Result<Medico2025>> SelectMedicoWhereId(MedicoId id);
	Task<Result<Paciente2025>> SelectPacienteWhereId(PacienteId id);
	Task<Result<Turno2025>> SelectTurnoWhereId(TurnoId id);
	Task<Result<UsuarioBase2025>> SelectUsuarioWhereId(UsuarioId id);
	// ----------------INSERT
	Task<Result<UsuarioId>> InsertUsuarioReturnId(UsuarioBase2025 usuario);
	Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 turno);
	Task<Result<PacienteId>> InsertPacienteReturnId(Paciente2025 paciente);
	Task<Result<MedicoId>> InsertMedicoReturnId(Medico2025 paciente);
	// ----------------UPDATE
	Task<Result<Unit>> UpdateMedicoWhereId(Medico2025 medico);
	Task<Result<Unit>> UpdatePacienteWhereId(Paciente2025 paciente);
	Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 turno);

	// ----------------DELETE
	Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id);
	Task<Result<Unit>> DeletePacienteWhereId(PacienteId id);
	Task<Result<Unit>> DeleteMedicoWhereId(MedicoId id);
	Task<Result<Unit>> DeleteUsuarioWhereId(UsuarioId id);
}