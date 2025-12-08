using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;



public static class IWPFRepositorioInterfaces {



	


	public interface IWPFRepositorio :
		IWPFRepositorioMedicos,
		IWPFRepositorioPacientes,
		IWPFRepositorioDominio,
		IWPFRepositorioTurnos {
	}

	public interface IWPFRepositorioMedicos {
		Task<ResultWpf<UnitWpf>> DeleteMedicoWhereId(MedicoId id);
		Task<ResultWpf<MedicoId>> InsertMedicoReturnId(Medico2025 instance);
		Task<ResultWpf<UnitWpf>> UpdateMedicoWhereId(Medico2025Agg instance);
		Task<List<MedicoDto>> SelectMedicos();
		Task<List<MedicoDto>> SelectMedicosWhereEspecialidadCodigo(EspecialidadCodigo code);
		Task<MedicoDto?> SelectMedicoWhereId(MedicoId id);
	}

	public interface IWPFRepositorioPacientes {
		Task<ResultWpf<UnitWpf>> DeletePacienteWhereId(PacienteId id);
		Task<ResultWpf<PacienteId>> InsertPacienteReturnId(Paciente2025 instance);
		Task<List<PacienteDto>> SelectPacientes();
		Task<PacienteDto?> SelectPacienteWhereId(PacienteId id);
		Task<ResultWpf<UnitWpf>> UpdatePacienteWhereId(Paciente2025Agg instance);
	}

	public interface IWPFRepositorioDominio {
		Task<List<Disponibilidad2025>> SelectDisponibilidades(EspecialidadCodigo especialidad, int cuantos, DateTime apartirDeCuando);

	}

	public interface IWPFRepositorioTurnos {
		Task<List<TurnoDto>> SelectTurnos();
		Task<List<TurnoDto>> SelectTurnosWherePacienteId(PacienteId id);
		Task<List<TurnoDto>> SelectTurnosWhereMedicoId(MedicoId id);
		Task<ResultWpf<TurnoDto>> AgendarNuevoTurno(PacienteId pacienteId, DateTime fechaSolicitudOriginal, Disponibilidad2025 disponibilidad);
		Task<ResultWpf<TurnoDto>> CancelarTurno(TurnoId turnoId, DateTime fechaOutcome, string reason);
		Task<ResultWpf<TurnoDto>> ReprogramarTurno(TurnoId turnoId, DateTime fechaOutcome, string reason);
		Task<ResultWpf<TurnoDto>> MarcarTurnoComoAusente(TurnoId turnoId, DateTime fechaOutcome, string reason);
		Task<ResultWpf<TurnoDto>> MarcarTurnoComoConcretado(TurnoId turnoId, DateTime fechaOutcome);
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
//Task<SecretariaPacienteFormularioViewModel> GetPacienteById(int id);
//Task<WindowModificarTurnoViewModel> GetTurnoById(int id);
//Task<EspecialidadMedicaViewModel> GetEspecialidadById(int id);


//// Create methods
//Task<bool> CreateMedico(MedicoViewModel2025 instance);
//Task<bool> CreatePaciente(SecretariaPacienteFormularioViewModel instance);
//Task<bool> CreateTurno(WindowModificarTurnoViewModel instance);
////public abstract bool CreateEspecialidad(WindowModificarEspecialidadViewModel instance);

//// Update methods
//Task<bool> UpdateMedico(MedicoViewModel2025 instance);
//Task<bool> UpdatePaciente(SecretariaPacienteFormularioViewModel instance);
//Task<bool> UpdateTurno(WindowModificarTurnoViewModel instance);
////public abstract bool UpdateEspecialidad(WindowModificarEspecialidadViewModel instance);

//// Delete methods
//Task<bool> DeleteMedico(MedicoViewModel2025 instance);
//Task<bool> DeletePaciente(SecretariaPacienteFormularioViewModel instance);
//Task<bool> DeleteTurno(WindowModificarTurnoViewModel instance);

//// Filtros
//Task<List<WindowModificarTurnoViewModel>> ReadTurnosWhereMedicoId(int? medicoId);
//Task<List<WindowModificarTurnoViewModel>> ReadTurnosWherePacienteId(int? SelectedPacienteId);
//Task<List<MedicoViewModel2025>> ReadMedicosWhereEspecialidad(int? EspecialidadCodigo);

//}

