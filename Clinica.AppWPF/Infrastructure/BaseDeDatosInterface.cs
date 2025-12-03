using Clinica.AppWPF.ViewModels;

namespace Clinica.AppWPF.Infrastructure;

public interface BaseDeDatosInterface {
	// Read methods
	Task<List<WindowModificarMedicoViewModel>> ReadMedicos();
	Task<List<WindowModificarPacienteViewModel>> ReadPacientes();
	Task<List<WindowModificarTurnoViewModel>> ReadTurnos();
	Task<List<EspecialidadMedicaViewModel>> ReadDistinctEspecialidades();  //WORTH CACHE-ING

	// Get methods
	Task<WindowModificarMedicoViewModel> GetMedicoById(int id);
	Task<WindowModificarPacienteViewModel> GetPacienteById(int id);
	Task<WindowModificarTurnoViewModel> GetTurnoById(int id);
	Task<EspecialidadMedicaViewModel> GetEspecialidadById(int id);


	// Create methods
	Task<bool> CreateMedico(WindowModificarMedicoViewModel instance);
	Task<bool> CreatePaciente(WindowModificarPacienteViewModel instance);
	Task<bool> CreateTurno(WindowModificarTurnoViewModel instance);
	//public abstract bool CreateEspecialidad(WindowModificarEspecialidadViewModel instance);

	// Update methods
	Task<bool> UpdateMedico(WindowModificarMedicoViewModel instance);
	Task<bool> UpdatePaciente(WindowModificarPacienteViewModel instance);
	Task<bool> UpdateTurno(WindowModificarTurnoViewModel instance);
	//public abstract bool UpdateEspecialidad(WindowModificarEspecialidadViewModel instance);

	// Delete methods
	Task<bool> DeleteMedico(WindowModificarMedicoViewModel instance);
	Task<bool> DeletePaciente(WindowModificarPacienteViewModel instance);
	Task<bool> DeleteTurno(WindowModificarTurnoViewModel instance);

	// Filtros
	Task<List<WindowModificarTurnoViewModel>> ReadTurnosWhereMedicoId(int? medicoId);
	Task<List<WindowModificarTurnoViewModel>> ReadTurnosWherePacienteId(int? pacienteId);
	Task<List<WindowModificarMedicoViewModel>> ReadMedicosWhereEspecialidad(int? especialidadCodigoInterno);

}

