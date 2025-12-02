using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.IRepositorios.QueryModels;

namespace Clinica.Dominio.IRepositorios;

public interface IRepositorioDomain {
	Task<Result<IEnumerable<TurnoQM>>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<IEnumerable<HorarioMedicoQM>>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<IEnumerable<MedicoQM>>> SelectMedicosWhereEspecialidadCode(EspecialidadCodigo2025 code);
	Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 turno); //this 2 can stay cause doesnt ask a model
	Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 turno); //this 2 can stay cause doesnt ask a model
	Task<Result<Usuario2025>> SelectUsuarioWhereName(NombreUsuario nombre); //need domain entitiy because this is not really data to query, it's data that immediatly needs domain methods.
}


public interface IRepositorioApi {

	Task<Result<IEnumerable<Turno2025>>> SelectTurnos();
	Task<Result<IEnumerable<Result<Turno2025>>>> SelectTurnosWherePacienteId(PacienteId id);

	Task<Result<IEnumerable<Turno2025>>> SelectTurnosWhereMedicoId(MedicoId id);
	Task<Result<IEnumerable<Paciente2025>>> SelectPacientes();
	Task<Result<IEnumerable<Medico2025>>> SelectMedicos();


	Task<Result<Usuario2025>> SelectUsuarioWhereId(UsuarioId id);
	Task<Result<Medico2025>> SelectMedicoWhereId(MedicoId id);
	Task<Result<Paciente2025>> SelectPacienteWhereId(PacienteId id);
	Task<Result<Turno2025>> SelectTurnoWhereId(TurnoId id);


	Task<Result<UsuarioId>> InsertUsuarioReturnId(Usuario2025 usuario);
	Task<Result<PacienteId>> InsertPacienteReturnId(Paciente2025 paciente);
	Task<Result<MedicoId>> InsertMedicoReturnId(Medico2025 paciente);
	Task<Result<Unit>> UpdateMedicoWhereId(Medico2025 medico);
	Task<Result<Unit>> UpdatePacienteWhereId(Paciente2025 paciente);

	Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id);
	Task<Result<Unit>> DeletePacienteWhereId(PacienteId id);
	Task<Result<Unit>> DeleteMedicoWhereId(MedicoId id);
	Task<Result<Unit>> DeleteUsuarioWhereId(UsuarioId id);

}