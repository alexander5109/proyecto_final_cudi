using System;
using System.ComponentModel;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

public class SecretariaPacienteFormularioViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	private void Notify(string prop) => PropertyChanged?.Invoke(this, new(prop));

	// -----------------------------
	// PROPIEDADES
	// -----------------------------
	private PacienteId? _id;
	public PacienteId? Id {
		get => _id;
		set { _id = value; Notify(nameof(Id)); }
	}

	private string _dni = "";
	public string Dni {
		get => _dni;
		set { _dni = value; Notify(nameof(Dni)); }
	}

	private string _nombre = "";
	public string Nombre {
		get => _nombre;
		set { _nombre = value; Notify(nameof(Nombre)); }
	}

	private string _apellido = "";
	public string Apellido {
		get => _apellido;
		set { _apellido = value; Notify(nameof(Apellido)); }
	}

	private DateTime _fechaIngreso;
	public DateTime FechaIngreso {
		get => _fechaIngreso;
		set { _fechaIngreso = value; Notify(nameof(FechaIngreso)); }
	}

	private string _domicilio = "";
	public string Domicilio {
		get => _domicilio;
		set { _domicilio = value; Notify(nameof(Domicilio)); }
	}

	private string _localidad = "";
	public string Localidad {
		get => _localidad;
		set { _localidad = value; Notify(nameof(Localidad)); }
	}

	private ProvinciaCodigo2025 _provinciaCodigo;
	public ProvinciaCodigo2025 ProvinciaCodigo {
		get => _provinciaCodigo;
		set { _provinciaCodigo = value; Notify(nameof(ProvinciaCodigo)); Notify(nameof(Provincia)); }
	}

	public string Provincia => ProvinciaCodigo.ATexto();

	private string _telefono = "";
	public string Telefono {
		get => _telefono;
		set { _telefono = value; Notify(nameof(Telefono)); }
	}

	private string _email = "";
	public string Email {
		get => _email;
		set { _email = value; Notify(nameof(Email)); }
	}

	private DateTime _fechaNacimiento = DateTime.MinValue;
	public DateTime FechaNacimiento {
		get => _fechaNacimiento;
		set { _fechaNacimiento = value; Notify(nameof(FechaNacimiento)); }
	}

	public string Displayear => $"{Id}: {Nombre} {Apellido}";
}
