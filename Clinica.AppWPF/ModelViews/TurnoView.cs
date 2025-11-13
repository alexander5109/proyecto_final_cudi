using Clinica.AppWPF.ModelViews;
using Newtonsoft.Json;
using System.Windows;
using SystemTextJson = System.Text.Json;

namespace Clinica.AppWPF.Entidades; 
//---------------------------------Tablas.Turnos-------------------------------//
public class TurnoView {
	public string ?Id { get; set; }
	public string ?PacienteId { get; set; }
	public string ?MedicoId { get; set; }
	public DateTime ?Fecha { get; set; }
	public TimeSpan ?Hora { get; set; }
	public int DuracionMinutos { get; set; }

	public TurnoView() { }
	
	// Constructor de PAciente para JSON
	public TurnoView(SystemTextJson.JsonElement json){
	}

	// Constructor de PAciente en base a una ventana
	public TurnoView(TurnosModificar window){
		this.LeerDesdeVentana(window);
	}
	
	[JsonIgnore]
	public MedicoView MedicoRelacionado{
		get{
			if (App.BaseDeDatos.DictMedicos.TryGetValue(MedicoId, out MedicoView medicoRelacionado)){
				return medicoRelacionado;
			}
			else{
				// MessageBox.Show("Error de integridad. No existe medico con esa ID", "Error de integridad", MessageBoxButton.OK, MessageBoxImage.Error);
				return null; 
			}
		}
	}
	
	[JsonIgnore]
	public PacienteView PacienteRelacionado{
		get{
			if (App.BaseDeDatos.DictPacientes.TryGetValue(PacienteId, out PacienteView pacienteRelacionado)){
				return pacienteRelacionado;
			}
			else{
				// MessageBox.Show("Error de integridad. No existe paciente con esa ID", "Error de integridad", MessageBoxButton.OK, MessageBoxImage.Error);
				return null; 
			}
		}
	}
	
}
