using Clinica.AppWPF;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Clinica.AppWPF.ModelViews; 
public class MedicoView : INotifyPropertyChanged {

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged([CallerMemberName] string? name = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	public MedicoView(
		ObservableCollection<HorarioMedicoView> horarios,
		string? id,
		string? name,
		string? lastName,
		string? dni,
		string? provincia,
		string? domicilio,
		string? localidad,
		string? especialidad,
		string? telefono,
		bool? guardia,
		DateTime? fechaIngreso,
		decimal? sueldoMinimoGarantizado
	) {
		_horarios = horarios;
		_id = id;
		_name = name;
		_lastName = lastName;
		_dni = dni;
		_provincia = provincia;
		_domicilio = domicilio;
		_localidad = localidad;
		_especialidad = especialidad;
		_telefono = telefono;
		_guardia = guardia;
		_fechaIngreso = fechaIngreso;
		_sueldoMinimoGarantizado = sueldoMinimoGarantizado;
	}

	// --- Campos privados con backing field ---
	private ObservableCollection<HorarioMedicoView> _horarios;
	public ObservableCollection<HorarioMedicoView> Horarios {
		get => _horarios;
		set { _horarios = value; OnPropertyChanged(); }
	}

	private string? _id;
	public string? Id {
		get => _id;
		set { _id = value; OnPropertyChanged(); }
	}

	private string? _name;
	public string? Name {
		get => _name;
		set { _name = value; OnPropertyChanged(); OnPropertyChanged(nameof(Displayear)); }
	}

	private string? _lastName;
	public string? LastName {
		get => _lastName;
		set { _lastName = value; OnPropertyChanged(); OnPropertyChanged(nameof(Displayear)); }
	}

	private string? _dni;
	public string? Dni {
		get => _dni;
		set { _dni = value; OnPropertyChanged(); }
	}

	private string? _provincia;
	public string? Provincia {
		get => _provincia;
		set { _provincia = value; OnPropertyChanged(); }
	}

	private string? _domicilio;
	public string? Domicilio {
		get => _domicilio;
		set { _domicilio = value; OnPropertyChanged(); }
	}

	private string? _localidad;
	public string? Localidad {
		get => _localidad;
		set { _localidad = value; OnPropertyChanged(); }
	}

	private string? _especialidad;
	public string? Especialidad {
		get => _especialidad;
		set { _especialidad = value; OnPropertyChanged(); OnPropertyChanged(nameof(Displayear)); }
	}

	private string? _telefono;
	public string? Telefono {
		get => _telefono;
		set { _telefono = value; OnPropertyChanged(); }
	}

	private bool? _guardia;
	public bool? Guardia {
		get => _guardia;
		set { _guardia = value; OnPropertyChanged(); }
	}

	private DateTime? _fechaIngreso;
	public DateTime? FechaIngreso {
		get => _fechaIngreso;
		set { _fechaIngreso = value; OnPropertyChanged(); }
	}

	private decimal? _sueldoMinimoGarantizado;
	public decimal? SueldoMinimoGarantizado {
		get => _sueldoMinimoGarantizado;
		set { _sueldoMinimoGarantizado = value; OnPropertyChanged(); }
	}

	[JsonIgnore]
	public string Displayear => $"{Id}: {Especialidad} - {Name} {LastName}";

	public static MedicoView NewEmpty() => new MedicoView(
		horarios: new ObservableCollection<HorarioMedicoView>(),
		id: null,
		name: null,
		lastName: null,
		dni: null,
		provincia: null,
		domicilio: null,
		localidad: null,
		especialidad: null,
		telefono: null,
		guardia: null,
		fechaIngreso: null,
		sueldoMinimoGarantizado: null
	);


	public Result<Medico2025> ToDomain() {
		// --- Value Objects base ---
		var nombreResult = NombreCompleto2025.Crear(this.Name, this.LastName);
		var especialidadResult = MedicoEspecialidad2025.Crear(this.Especialidad, "Clinica General");
		var dniResult = DniArgentino2025.Crear(this.Dni);
		var domicilioResult = DomicilioArgentino2025.Crear(
			LocalidadDeProvincia2025.Crear(this.Localidad, ProvinciaArgentina2025.Crear(this.Provincia)),
			this.Domicilio
		);
		var telefonoResult = ContactoTelefono2025.Crear(this.Telefono);
		var fechaIngresoResult = FechaIngreso2025.Crear(this.FechaIngreso);
		var sueldoResult = MedicoSueldoMinimo2025.Crear(this.SueldoMinimoGarantizado);
		Result<ListaHorarioMedicos2025> horariosResult = ListaHorarioMedicos2025.Crear(
			this.Horarios
				.SelectMany(dia =>
					dia.FranjasHora.Select(h => HorarioMedico2025.Crear(
						DiaSemana2025.Crear(dia.DiaName),
						HorarioHora2025.Crear(h.Desde),
						HorarioHora2025.Crear(h.Hasta)
					))
				)
				.ToList()
		);

		// --- Combinamos todo en el agregado ---
		return Medico2025.Crear(
			nombreResult,
			especialidadResult,
			dniResult,
			domicilioResult,
			telefonoResult,
			horariosResult,
			fechaIngresoResult,
			sueldoResult,
			this.Guardia ?? false
		);
	}





}