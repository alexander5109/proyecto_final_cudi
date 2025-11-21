using Clinica.AppWPF.ViewModels;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.AppWPF; 
public abstract class BaseDeDatosAbstracta{
	public Dictionary<int, ViewModelTurno> DictTurnos = [];
	public Dictionary<int, ViewModelMedico> DictMedicos = [];
	public Dictionary<int, ViewModelPaciente> DictPacientes = [];
	//public Dictionary<int, ViewModelEspecialidadMedica> DictEspecialidades = [];
	public virtual bool ConectadaExitosamente { get; protected set; } = false;
	
	// Read methods
	public abstract List<ViewModelMedico> ReadMedicos();
	public abstract List<ViewModelPaciente> ReadPacientes();
	public abstract List<ViewModelTurno> ReadTurnos();

	// Crear methods
	public abstract bool CreateMedico(Medico2025 instance, ViewModelMedico instanceDto);
	public abstract bool CreatePaciente(Paciente2025 instance, ViewModelPaciente instanceDto);
	public abstract bool CreateTurno(Turno2025 instance, ViewModelTurno instanceDto);

	// Update methods
	public abstract bool UpdateMedico(Medico2025 instance, int? instanceId);
	public abstract bool UpdatePaciente(Paciente2025 instance, int? instanceId);
	public abstract bool UpdateTurno(Turno2025 instance, ViewModelTurno instanceDto);

	// Delete methods
	public abstract bool DeleteMedico(ViewModelMedico instance);
	public abstract bool DeletePaciente(ViewModelPaciente instance);
	public abstract bool DeleteTurno(ViewModelTurno instance);
	
	// Filtros
	public List<ViewModelTurno> ReadTurnosWhereMedicoId(int? medicoId) {
		if (medicoId == null) return new List<ViewModelTurno>();
		return DictTurnos.Values.Where(t => t.MedicoId == medicoId).ToList();
	}
	public List<ViewModelTurno> ReadTurnosWherePacienteId(int? pacienteId) {
		if (pacienteId == null) return new List<ViewModelTurno>();
		return DictTurnos.Values.Where(t => t.PacienteId == pacienteId).ToList();
	}

	public List<ViewModelMedico> ReadMedicosWhereEspecialidad(int? especialidadCodigoInterno) {
		if (especialidadCodigoInterno == null) return new List<ViewModelMedico>();
		return DictMedicos.Values.Where(m => m.EspecialidadCodigoInterno == especialidadCodigoInterno).ToList();
	}
	public string[] ReadDistinctEspecialidades() {
		return EspecialidadMedica2025.Titulos;
	}

	//------------------------settings----------------------//
	// public abstract bool EliminarDatabaseExitosamente();

}

