using Clinica.Dominio.Entidades;
using System.ComponentModel;

namespace Clinica.AppWPF.Entidades;

public class PacienteModelView : INotifyPropertyChanged {
	private Paciente2025EnDb _pacienteOriginal;

	public PacienteModelView(Paciente2025EnDb paciente) {
		_pacienteOriginal = paciente;
	}

	public string Id => _pacienteOriginal.Id;

	public string Dni {
		get => _pacienteOriginal.Paciente.Dni;
		set {
			// Si querés, podés validar aquí antes de setear un nuevo valor editable
			OnPropertyChanged(nameof(Dni));
		}
	}

	public string Name {
		get => _pacienteOriginal.Paciente.NombreCompleto.Nombre;
		set { OnPropertyChanged(nameof(Name)); }
	}

	public string LastName {
		get => _pacienteOriginal.Paciente.NombreCompleto.Apellido;
		set { OnPropertyChanged(nameof(LastName)); }
	}

	public DateTime FechaNacimiento {
		get => _pacienteOriginal.Paciente.FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue);
		set { OnPropertyChanged(nameof(FechaNacimiento)); }
	}

	public string Email {
		get => _pacienteOriginal.Paciente.Contacto.Email;
		set { OnPropertyChanged(nameof(Email)); }
	}

	public string Telefono {
		get => _pacienteOriginal.Paciente.Contacto.Telefono;
		set { OnPropertyChanged(nameof(Telefono)); }
	}

	public string Provincia => _pacienteOriginal.Paciente.Domicilio.Localidad.Provincia.Nombre;
	public string Localidad => _pacienteOriginal.Paciente.Domicilio.Localidad.Nombre;
	public string Domicilio => _pacienteOriginal.Paciente.Domicilio.Direccion;

	public string Displayear => $"{Id}: {Name} {LastName}";

	public Paciente2025EnDb PacienteOriginal => _pacienteOriginal;

	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string propertyName) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}