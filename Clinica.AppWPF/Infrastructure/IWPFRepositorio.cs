using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;


public static class IWPFRepositorioInterfaces {
	public interface IWPFRepositorio :
		IWPFRepositorioMedicos,
		IWPFRepositorioPacientes {
	}

	public interface IWPFRepositorioMedicos {
		Task<Result<Unit>> DeleteMedicoWhereId(MedicoId id);
		Task<Result<MedicoId>> InsertMedicoReturnId(Medico2025 instance);
		Task<Result<Unit>> UpdateMedicoWhereId(Medico2025 instance);
		Task<List<MedicoDto>> SelectMedicos();
		Task<List<TurnoDto>> SelectTurnosWhereMedicoId(MedicoId id);
		Task<List<MedicoDto>> SelectMedicosWhereEspecialidadCode(EspecialidadCodigo code);
		Task<MedicoDto?> SelectMedicoWhereId(MedicoId id);
	}

	public interface IWPFRepositorioPacientes {
		Task<Result<Unit>> DeletePacienteWhereId(PacienteId id);
		Task<Result<PacienteId>> InsertPacienteReturnId(Paciente2025 instance);
		Task<List<PacienteDto>> SelectPacientes();
		Task<List<TurnoDto>> SelectTurnosWherePacienteId(PacienteId id);
		Task<PacienteDto?> SelectPacienteWhereId(PacienteId id);
		Task<Result<Unit>> UpdatePacienteWhereId(Paciente2025 instance);
	}

}


//public interface IRepositorio {






// Read methods
//Task<List<MedicoDto>> ReadMedicos();
//Task<List<PacienteDto>> ReadPacientes();
//Task<List<TurnoDto>> ReadTurnos();
//Task<List<EspecialidadMedicaViewModel>> ReadDistinctEspecialidades();  //WORTH CACHE-ING

//// Get methods
//Task<WindowModificarMedicoViewModel> GetMedicoById(int id);
//Task<WindowModificarPacienteViewModel> GetPacienteById(int id);
//Task<WindowModificarTurnoViewModel> GetTurnoById(int id);
//Task<EspecialidadMedicaViewModel> GetEspecialidadById(int id);


//// Create methods
//Task<bool> CreateMedico(WindowModificarMedicoViewModel instance);
//Task<bool> CreatePaciente(WindowModificarPacienteViewModel instance);
//Task<bool> CreateTurno(WindowModificarTurnoViewModel instance);
////public abstract bool CreateEspecialidad(WindowModificarEspecialidadViewModel instance);

//// Update methods
//Task<bool> UpdateMedico(WindowModificarMedicoViewModel instance);
//Task<bool> UpdatePaciente(WindowModificarPacienteViewModel instance);
//Task<bool> UpdateTurno(WindowModificarTurnoViewModel instance);
////public abstract bool UpdateEspecialidad(WindowModificarEspecialidadViewModel instance);

//// Delete methods
//Task<bool> DeleteMedico(WindowModificarMedicoViewModel instance);
//Task<bool> DeletePaciente(WindowModificarPacienteViewModel instance);
//Task<bool> DeleteTurno(WindowModificarTurnoViewModel instance);

//// Filtros
//Task<List<WindowModificarTurnoViewModel>> ReadTurnosWhereMedicoId(int? medicoId);
//Task<List<WindowModificarTurnoViewModel>> ReadTurnosWherePacienteId(int? pacienteId);
//Task<List<WindowModificarMedicoViewModel>> ReadMedicosWhereEspecialidad(int? especialidadCodigoInterno);

//}

