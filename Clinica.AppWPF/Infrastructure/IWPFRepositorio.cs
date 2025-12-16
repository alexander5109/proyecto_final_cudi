using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.ApiDtos.ServiciosPublicosDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure;



public static class IWPFRepositorioInterfaces {






	public interface IWPFRepositorio :
		IWPFRepositorioMedicos,
		IWPFRepositorioPacientes,
		IWPFRepositorioDominio,
		IWPFRepositorioTurnos,
		IWPFRepositorioHorarios,
		IWPFRepositorioUsuarios {

		//Task RefreshMedicos();
		//Task RefreshPacientes();
		//Task RefreshUsuarios();
		//Task RefreshHorarios();
    }
	public interface IWPFRepositorioUsuarios {
		Task<ResultWpf<UnitWpf>> DeleteUsuarioWhereId(UsuarioId id);
		Task<ResultWpf<UsuarioId>> InsertUsuarioReturnId(Usuario2025 instance);
		Task<ResultWpf<UnitWpf>> UpdateUsuarioWhereId(Usuario2025EdicionAgg instance);
		Task<List<UsuarioDbModel>> SelectUsuarios();
		Task<UsuarioDbModel?> SelectUsuarioProfileWhereUsername(string username);
		Task<IReadOnlyCollection<AccionesDeUsuarioEnum>> SelectAccionesDeUsuarioWhereEnumRole(UsuarioRoleEnum enumRole);
		Task<IReadOnlyCollection<AccionesDeUsuarioEnum>> SelectAccionesDeUsuario();


	}


	
	public interface IWPFRepositorioMedicos {
		Task<ResultWpf<UnitWpf>> DeleteMedicoWhereId(MedicoId id);
		Task<ResultWpf<MedicoId>> InsertMedicoReturnId(Medico2025 instance);
		Task<ResultWpf<UnitWpf>> UpdateMedicoWhereId(Medico2025Agg instance);
		//Task<ResultWpf<UnitWpf>> UpdateMedicoWhereIdWithHorarios(MedicoId id, Medico2025 instance, IEnumerable<HorarioDtos.HorarioDto> horarios);
		//Task<List<MedicoDbModel>> SelectMedicos();
		Task<List<MedicoDbModel>> SelectMedicos();
		Task EnsureMedicosLoaded();
		Task<List<MedicoDbModel>> SelectMedicosWhereEspecialidadCodigo(EspecialidadEnum code);
		Task<MedicoDbModel?> SelectMedicoWhereId(MedicoId id);
	}

	public interface IWPFRepositorioPacientes {
		Task EnsurePacientesLoaded();
		Task<ResultWpf<UnitWpf>> DeletePacienteWhereId(PacienteId id);
		Task<ResultWpf<PacienteId>> InsertPacienteReturnId(Paciente2025 instance);
		Task<List<PacienteDbModel>> SelectPacientes();
		Task<PacienteDbModel?> SelectPacienteWhereId(PacienteId id);
		Task<ResultWpf<UnitWpf>> UpdatePacienteWhereId(Paciente2025Agg instance);
	}

	public interface IWPFRepositorioDominio {
		Task<List<Disponibilidad2025>> SelectDisponibilidades(SolicitarDisponibilidadesDto solicitud);

	}

	public interface IWPFRepositorioHorarios {
		//Task<List<HorarioDbModel>> SelectHorarios();
		Task<IReadOnlyList<HorarioDbModel>?> SelectHorariosWhereMedicoId(MedicoId id);
		Task<IReadOnlyList<DayOfWeek>?> SelectDiasDeAtencionWhereMedicoId(MedicoId id);
		Task<ResultWpf<UnitWpf>> UpdateHorariosWhereMedicoId(HorariosMedicos2026Agg agregado);
	}

	public interface IWPFRepositorioTurnos {
		Task<List<TurnoDbModel>> SelectTurnos();
		Task<List<TurnoDbModel>> SelectTurnosWherePacienteId(PacienteId id);
		Task<List<TurnoDbModel>> SelectTurnosWhereMedicoId(MedicoId id);
		Task<ResultWpf<UnitWpf>> AgendarNuevoTurno(PacienteId pacienteId, DateTime fechaSolicitudOriginal, Disponibilidad2025 disponibilidad);
		Task<ResultWpf<UnitWpf>> CancelarTurno(TurnoId turnoId, DateTime fechaOutcome, string? reason);
		Task<ResultWpf<UnitWpf>> ReprogramarTurno(TurnoId turnoId, DateTime fechaOutcome, string? reason);
		Task<ResultWpf<UnitWpf>> MarcarTurnoComoAusente(TurnoId turnoId, DateTime fechaOutcome, string? reason);
		Task<ResultWpf<UnitWpf>> MarcarTurnoComoConcretado(TurnoId turnoId, DateTime fechaOutcome, string? reason);
	}
}



//public interface IRepositorio {






// Read methods
//Task<List<MedicoViewModel2025>> ReadMedicos();
//Task<List<PacienteDto>> ReadPacientes();
//Task<List<TurnoViewModel2025>> ReadTurnos();
//Task<List<EspecialidadMedicaViewModel>> ReadDistinctEspecialidades();  //WORTH CACHE-ING

//// Get methods
//Task<MedicoViewModel2025> GetMedicoById(int id);
//Task<DialogoPacienteModificarVM> GetPacienteById(int id);
//Task<WindowModificarTurnoViewModel> GetTurnoById(int id);
//Task<EspecialidadMedicaViewModel> GetEspecialidadById(int id);


//// Create methods
//Task<bool> CreateMedico(MedicoViewModel2025 instance);
//Task<bool> CreatePaciente(DialogoPacienteModificarVM instance);
//Task<bool> CreateTurno(WindowModificarTurnoViewModel instance);
////public abstract bool CreateEspecialidad(WindowModificarEspecialidadViewModel instance);

//// Update methods
//Task<bool> UpdateMedico(MedicoViewModel2025 instance);
//Task<bool> UpdatePaciente(DialogoPacienteModificarVM instance);
//Task<bool> UpdateTurno(WindowModificarTurnoViewModel instance);
////public abstract bool UpdateEspecialidad(WindowModificarEspecialidadViewModel instance);

//// Delete methods
//Task<bool> DeleteMedico(MedicoViewModel2025 instance);
//Task<bool> DeletePaciente(DialogoPacienteModificarVM instance);
//Task<bool> DeleteTurno(WindowModificarTurnoViewModel instance);

//// Filtros
//Task<List<WindowModificarTurnoViewModel>> ReadTurnosWhereMedicoId(int? medicoId);
//Task<List<WindowModificarTurnoViewModel>> ReadTurnosWherePacienteId(int? SelectedPacienteId);
//Task<List<MedicoViewModel2025>> ReadMedicosWhereEspecialidad(int? EspecialidadEnum);

//}

