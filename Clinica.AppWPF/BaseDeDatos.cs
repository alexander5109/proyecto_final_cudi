using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.AppWPF; 
public abstract class BaseDeDatosAbstracta{
	public Dictionary<string, ModelViewTurno> DictTurnos = [];
	public Dictionary<string, ModelViewMedico> DictMedicos = [];
	public Dictionary<string, ModelViewPaciente> DictPacientes = [];
	public virtual bool ConectadaExitosamente { get; protected set; } = false;
	
	// Read methods
	public abstract List<ModelViewMedico> ReadMedicos();
	public abstract List<ModelViewPaciente> ReadPacientes();
	public abstract List<ModelViewTurno> ReadTurnos();

	// Crear methods
	public abstract bool CreateMedico(Medico2025 instance, ModelViewMedico instanceDto);
	public abstract bool CreatePaciente(Paciente2025 instance, ModelViewPaciente instanceDto);
	public abstract bool CreateTurno(Turno2025 instance, ModelViewTurno instanceDto);

	// Update methods
	public abstract bool UpdateMedico(Medico2025 instance, string instanceId);
	public abstract bool UpdatePaciente(Paciente2025 instance, string instanceId);
	public abstract bool UpdateTurno(Turno2025 instance, ModelViewTurno instanceDto);

	// Delete methods
	public abstract bool DeleteMedico(ModelViewMedico instance);
	public abstract bool DeletePaciente(ModelViewPaciente instance);
	public abstract bool DeleteTurno(ModelViewTurno instance);
	
	// Filtros
	public List<ModelViewTurno> ReadTurnosWhereMedicoId(ModelViewMedico instance) {
		return DictTurnos.Values.Where(t => t.MedicoId == instance.Id).ToList();
	}
	public List<ModelViewTurno> ReadTurnosWherePacienteId(ModelViewPaciente instance) {
		return DictTurnos.Values.Where(t => t.PacienteId == instance.Id).ToList();
	}

	public List<ModelViewMedico> ReadMedicosWhereEspecialidad(string especialidad) {
		return DictMedicos.Values.Where(m => m.Especialidad == especialidad).ToList();
	}
	public List<string> ReadDistinctEspecialidades() {
		return DictMedicos.Values.Select(medico => medico.Especialidad).Distinct().ToList();
	}

	//------------------------settings----------------------//
	// public abstract bool EliminarDatabaseExitosamente();

}

