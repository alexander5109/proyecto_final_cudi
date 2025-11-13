using SystemTextJson = System.Text.Json;
using Newtonsoft.Json;

namespace Clinica.AppWPF.Entidades; 
//---------------------------------Tablas.Pacientes-------------------------------//
public class PacienteView {
	public string? Id { get; set; }
	public string? Dni { get; set; }
	public string? Name { get; set; }
	public string? LastName { get; set; }
	public DateTime? FechaIngreso { get; set; }  // Corrige a DateTime
	public string? Email { get; set; }
	public string? Telefono { get; set; }
	public DateTime? FechaNacimiento { get; set; }
	public string? Domicilio { get; set; }
	public string? Localidad { get; set; }
	public string? Provincia { get; set; }

	[JsonIgnore]
	public string Displayear => $"{Id}: {Name} {LastName}";

	public PacienteView() { }

	public PacienteView(PacientesModificar window) {
		this.LeerDesdeVentana(window);
	}
}
