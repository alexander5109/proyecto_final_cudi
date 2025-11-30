using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.Dtos.DomainDtos;
using static Clinica.Dominio.Dtos.ApiDtos;

namespace Clinica.Dominio.IRepositorios;

public interface IBaseDeDatosRepositorio {
    Task<Result<Turno2025>> AgendarTurnoAsync(int pacienteId, int medicoId, EspecialidadMedica2025 especialidad, DateTime desde, DateTime hasta);
    Task<Result<Turno2025>> CancelarTurnoAsync(int id, Option<string> option);


    Task<Result<PacienteId>> CreatePaciente(Paciente2025 paciente);



	Task<Result<Unit>> DeletePaciente(int id);
	string EmitirJwt(UsuarioBase2025 usuario);
	Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 turno);
	Task<Result<UsuarioId>> InsertUsuarioReturnId(NombreUsuario nombre, ContraseñaHasheada password, byte enumRole);
    Task<Result<Turno2025>> MarcarTurnoComoAusenteAsync(int id, Option<string> option);
    Task<Result<Turno2025>> MarcarTurnoComoConcretadoAsync(int id, Option<string> option);
    Task<Result<Turno2025>> ObtenerTurnoPorIdAsync(TurnoId id);
    Task<Result<Turno2025>> ReprogramarTurnoAsync(int id, DateTime nuevaFechaDesde, DateTime nuevaFechaHasta);
    Task<IEnumerable<HorarioMedicoDto>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<IEnumerable<MedicoDto>> SelectMedicos();
	Task<IEnumerable<MedicoDto>> SelectMedicosWhereEspecialidad(EspecialidadMedica2025 especialidad);
	Task<Result<IEnumerable<PacienteDto>>> SelectPacientes();
	Task<Result<IEnumerable<PacienteListDto>>> SelectPacientesList();
	Task<IEnumerable<TurnoDto>> SelectTurnos();
	Task<IEnumerable<TurnoDto>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<UsuarioBase2025>> SelectUsuarioWhereId(UsuarioId id);
	Task<Result<UsuarioBase2025>> SelectUsuarioWhereNombre(NombreUsuario nombre);
    Task<Result<IReadOnlyList<DisponibilidadEspecialidad2025>>> SolicitarDisponibilidadesPara(EspecialidadMedica2025 especialidad, DateTime now, int cuantos);
    Task<Result<Unit>> UpdatePaciente(PacienteId id, PacienteDto dto);
	Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 turno);
	Task<Result<UsuarioBase2025>> ValidarCredenciales(string username, string password);




}