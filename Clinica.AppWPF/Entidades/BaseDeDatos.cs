using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using System.Windows;

namespace Clinica.AppWPF {
	public abstract class BaseDeDatosAbstracta{
		public Dictionary<string, Turno> DictTurnos = [];
		public Dictionary<string, MedicoDto> DictMedicos = [];
		public Dictionary<string, Paciente> DictPacientes = [];
		public virtual bool ConectadaExitosamente { get; protected set; } = false;
		
		// Read methods
		public abstract List<MedicoDto> ReadMedicos();
		public abstract List<Paciente> ReadPacientes();
		public abstract List<Turno> ReadTurnos();

		// Create methods
		public abstract bool CreateMedico(MedicoType instance, MedicoDto instanceDto);
		public abstract bool CreatePaciente(PacienteType instance, Paciente instanceDto);
		public abstract bool CreateTurno(TurnoType instance, Turno instanceDto);

		// Update methods
		public abstract bool UpdateMedico(MedicoType instance, string instanceId);
		public abstract bool UpdatePaciente(PacienteType instance, string instanceId);
		public abstract bool UpdateTurno(TurnoType instance, Turno instanceDto);

		// Delete methods
		public abstract bool DeleteMedico(MedicoDto instance);
		public abstract bool DeletePaciente(Paciente instance);
		public abstract bool DeleteTurno(Turno instance);
		
		// Filtros
		public List<Turno> ReadTurnosWhereMedicoId(MedicoDto instance) {
			if (instance is null){
				return null;
			}
			return DictTurnos.Values.Where(t => t.MedicoId == instance.Id).ToList();
		}
		public List<Turno> ReadTurnosWherePacienteId(Paciente instance) {
			if (instance is null){
				return null;
			}
			return DictTurnos.Values.Where(t => t.PacienteId == instance.Id).ToList();
		}

		public List<MedicoDto> ReadMedicosWhereEspecialidad(string especialidad) {
			return DictMedicos.Values.Where(m => m.Especialidad == especialidad).ToList();
		}
		public List<string> ReadDistinctEspecialidades() {
			return DictMedicos.Values.Select(medico => medico.Especialidad).Distinct().ToList();
		}

		//------------------------settings----------------------//
		// public abstract bool EliminarDatabaseExitosamente();

	}
	
	
}