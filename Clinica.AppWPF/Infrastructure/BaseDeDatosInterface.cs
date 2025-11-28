using Clinica.AppWPF.ViewModels;

namespace Clinica.AppWPF.Infrastructure;

public interface BaseDeDatosInterface {
	// Read methods
	public List<WindowModificarMedicoViewModel> ReadMedicos();
	public List<WindowModificarPacienteViewModel> ReadPacientes();
	public List<WindowModificarTurnoViewModel> ReadTurnos();
	public List<EspecialidadMedicaViewModel> ReadDistinctEspecialidades();  //WORTH CACHE-ING

	// Get methods
	public WindowModificarMedicoViewModel GetMedicoById(int id);
	public WindowModificarPacienteViewModel GetPacienteById(int id);
	public WindowModificarTurnoViewModel GetTurnoById(int id);
	public EspecialidadMedicaViewModel GetEspecialidadById(int id);


	// Create methods
	public bool CreateMedico(WindowModificarMedicoViewModel instance);
	public bool CreatePaciente(WindowModificarPacienteViewModel instance);
	public bool CreateTurno(WindowModificarTurnoViewModel instance);
	//public abstract bool CreateEspecialidad(WindowModificarEspecialidadViewModel instance);

	// Update methods
	public bool UpdateMedico(WindowModificarMedicoViewModel instance);
	public bool UpdatePaciente(WindowModificarPacienteViewModel instance);
	public bool UpdateTurno(WindowModificarTurnoViewModel instance);
	//public abstract bool UpdateEspecialidad(WindowModificarEspecialidadViewModel instance);

	// Delete methods
	public bool DeleteMedico(WindowModificarMedicoViewModel instance);
	public bool DeletePaciente(WindowModificarPacienteViewModel instance);
	public bool DeleteTurno(WindowModificarTurnoViewModel instance);

	// Filtros
	public List<WindowModificarTurnoViewModel> ReadTurnosWhereMedicoId(int? medicoId);
	public List<WindowModificarTurnoViewModel> ReadTurnosWherePacienteId(int? pacienteId);
	public List<WindowModificarMedicoViewModel> ReadMedicosWhereEspecialidad(int? especialidadCodigoInterno);

}

