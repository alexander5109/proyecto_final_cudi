using Clinica.AppWPF.ViewModels;

namespace Clinica.AppWPF.Infrastructure;

public interface BaseDeDatosInterface {
	// Read methods
	public Task<List<WindowModificarMedicoViewModel>> ReadMedicos();
	public Task<List<WindowModificarPacienteViewModel>> ReadPacientes();
	public Task<List<WindowModificarTurnoViewModel>> ReadTurnos();
	public Task<List<EspecialidadMedicaViewModel>> ReadDistinctEspecialidades();  //WORTH CACHE-ING

	// Get methods
	public Task<WindowModificarMedicoViewModel> GetMedicoById(int id);
	public Task<WindowModificarPacienteViewModel> GetPacienteById(int id);
	public Task<WindowModificarTurnoViewModel> GetTurnoById(int id);
	public Task<EspecialidadMedicaViewModel> GetEspecialidadById(int id);


	// Create methods
	public Task<bool> CreateMedico(WindowModificarMedicoViewModel instance);
	public Task<bool> CreatePaciente(WindowModificarPacienteViewModel instance);
	public Task<bool> CreateTurno(WindowModificarTurnoViewModel instance);
	//public abstract bool CreateEspecialidad(WindowModificarEspecialidadViewModel instance);

	// Update methods
	public Task<bool> UpdateMedico(WindowModificarMedicoViewModel instance);
	public Task<bool> UpdatePaciente(WindowModificarPacienteViewModel instance);
	public Task<bool> UpdateTurno(WindowModificarTurnoViewModel instance);
	//public abstract bool UpdateEspecialidad(WindowModificarEspecialidadViewModel instance);

	// Delete methods
	public Task<bool> DeleteMedico(WindowModificarMedicoViewModel instance);
	public Task<bool> DeletePaciente(WindowModificarPacienteViewModel instance);
	public Task<bool> DeleteTurno(WindowModificarTurnoViewModel instance);

	// Filtros
	public Task<List<WindowModificarTurnoViewModel>> ReadTurnosWhereMedicoId(int? medicoId);
	public Task<List<WindowModificarTurnoViewModel>> ReadTurnosWherePacienteId(int? pacienteId);
	public Task<List<WindowModificarMedicoViewModel>> ReadMedicosWhereEspecialidad(int? especialidadCodigoInterno);

}

