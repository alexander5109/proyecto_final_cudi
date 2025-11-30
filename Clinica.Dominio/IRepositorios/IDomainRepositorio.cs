using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.Dtos.DomainDtos;
using static Clinica.Dominio.Dtos.ApiDtos;

namespace Clinica.Dominio.IRepositorios;

public interface IBaseDeDatosRepositorio {



	Task<Result<PacienteId>> CreatePaciente(Paciente2025 paciente);



	Task<Result<Unit>> DeletePaciente(int id);
	string EmitirJwt(UsuarioBase2025 usuario);
	Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 turno);
	Task<Result<UsuarioId>> InsertUsuarioReturnId(NombreUsuario nombre, ContraseñaHasheada password, byte enumRole);
	Task<IEnumerable<HorarioMedicoDto>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<IEnumerable<MedicoDto>> SelectMedicos();
	Task<IEnumerable<MedicoDto>> SelectMedicosWhereEspecialidad(EspecialidadMedica2025 especialidad);
	Task<Result<IEnumerable<PacienteDto>>> SelectPacientes();
	Task<Result<IEnumerable<PacienteListDto>>> SelectPacientesList();
	Task<IEnumerable<TurnoDto>> SelectTurnos();
	Task<IEnumerable<TurnoDto>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<UsuarioBase2025>> SelectUsuarioWhereId(UsuarioId id);
	Task<Result<UsuarioBase2025>> SelectUsuarioWhereNombre(NombreUsuario nombre);
	Task<Result<Unit>> UpdatePaciente(PacienteId id, PacienteDto dto);
	Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 turno);
	Task<Result<UsuarioBase2025>> ValidarCredenciales(string username, string password);




}