using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.AppWPF.Entidades; 
public abstract class BaseDeDatosAbstracta{
	public Dictionary<string, TurnoView> DictTurnos = [];
	public Dictionary<string, MedicoView> DictMedicos = [];
	public Dictionary<string, PacienteView> DictPacientes = [];
	public virtual bool ConectadaExitosamente { get; protected set; } = false;
	
	// Read methods
	public abstract List<MedicoView> ReadMedicos();
	public abstract List<PacienteView> ReadPacientes();
	public abstract List<TurnoView> ReadTurnos();

	// Crear methods
	public abstract bool CreateMedico(Medico2025 instance, MedicoView instanceDto);
	public abstract bool CreatePaciente(Paciente2025 instance, PacienteView instanceDto);
	public abstract bool CreateTurno(Turno2025 instance, TurnoView instanceDto);

	// Update methods
	public abstract bool UpdateMedico(Medico2025 instance, string instanceId);
	public abstract bool UpdatePaciente(Paciente2025 instance, string instanceId);
	public abstract bool UpdateTurno(Turno2025 instance, TurnoView instanceDto);

	// Delete methods
	public abstract bool DeleteMedico(MedicoView instance);
	public abstract bool DeletePaciente(PacienteView instance);
	public abstract bool DeleteTurno(TurnoView instance);
	
	// Filtros
	public List<TurnoView> ReadTurnosWhereMedicoId(MedicoView instance) {
		if (instance is null){
			return null;
		}
		return DictTurnos.Values.Where(t => t.MedicoId == instance.Id).ToList();
	}
	public List<TurnoView> ReadTurnosWherePacienteId(PacienteView instance) {
		if (instance is null){
			return null;
		}
		return DictTurnos.Values.Where(t => t.PacienteId == instance.Id).ToList();
	}

	public List<MedicoView> ReadMedicosWhereEspecialidad(string especialidad) {
		return DictMedicos.Values.Where(m => m.Especialidad == especialidad).ToList();
	}
	public List<string> ReadDistinctEspecialidades() {
		return DictMedicos.Values.Select(medico => medico.Especialidad).Distinct().ToList();
	}

	//------------------------settings----------------------//
	// public abstract bool EliminarDatabaseExitosamente();

}

