using System.ComponentModel;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.DbModels.DbModels;
using static CommonEnumsToViewModel;



namespace Clinica.AppWPF.UsuarioRecepcionista;


public class SecretariaPacientesModificarViewModel : INotifyPropertyChanged {

	// ================================================================
	// CTOR
	// ================================================================
	public SecretariaPacientesModificarViewModel(PacienteDbModel original) {
		Original = original;
		Id = original.Id;
		Dni = original.Dni;
		Nombre = original.Nombre;
		Apellido = original.Apellido;
		FechaIngreso = original.FechaIngreso;
		Email = original.Email;
		Telefono = original.Telefono;
		FechaNacimiento = original.FechaNacimiento;
		Domicilio = original.Domicilio;
		Localidad = original.Localidad;
		Provincia = original.ProvinciaCodigo.ToViewModel();
	}

	// ================================================================
	// EXTRAS
	// ================================================================

	private readonly PacienteDbModel Original;
	public IReadOnlyList<ProvinciaVmItem> Provincias { get; } = [.. ProvinciaArgentina2025.Todas().Select(p => p.ToViewModel())];


	// ================================================================
	// REGLAS
	// ================================================================

	public bool EstaCreando => Id is null;
	public bool EstaEditando => Id is not null;
	public bool PuedeGuardarCambios => true;



	// -----------------------------
	// PROPERTIES
	// -----------------------------

	private PacienteId? _id;
	public PacienteId? Id {
		get => _id;
		set {
			_id = value;
			OnPropertyChanged(nameof(Id));
			OnPropertyChanged(nameof(EstaCreando));
			OnPropertyChanged(nameof(EstaEditando));
		}
	}

	private string _dni = "";
	public string Dni {
		get => _dni;
		set { _dni = value; OnPropertyChanged(nameof(Dni)); }
	}

	private string _nombre = "";
	public string Nombre {
		get => _nombre;
		set { _nombre = value; OnPropertyChanged(nameof(Nombre)); }
	}

	private string _apellido = "";
	public string Apellido {
		get => _apellido;
		set { _apellido = value; OnPropertyChanged(nameof(Apellido)); }
	}

	private DateTime _fechaIngreso = DateTime.Today;

	public DateTime FechaIngreso {
		get => _fechaIngreso;
		set {
			_fechaIngreso = value;
			OnPropertyChanged(nameof(FechaIngreso));
		}
	}

	private string _domicilio = "";
	public string Domicilio {
		get => _domicilio;
		set { _domicilio = value; OnPropertyChanged(nameof(Domicilio)); }
	}

	private string _localidad = "";
	public string Localidad {
		get => _localidad;
		set { _localidad = value; OnPropertyChanged(nameof(Localidad)); }
	}

	private ProvinciaVmItem? _provincia;
	public ProvinciaVmItem? Provincia {
		get => _provincia; set {
			_provincia = value;
			OnPropertyChanged(nameof(Provincia));
		}
	}

	private string _telefono = "";
	public string Telefono {
		get => _telefono;
		set { _telefono = value; OnPropertyChanged(nameof(Telefono)); }
	}

	private string _email = "";
	public string Email {
		get => _email;
		set { _email = value; OnPropertyChanged(nameof(Email)); }
	}

	private DateTime _fechaNacimiento = DateTime.Today;
	public DateTime FechaNacimiento {
		get => _fechaNacimiento;
		set { _fechaNacimiento = value; OnPropertyChanged(nameof(FechaNacimiento)); }
	}


	// -----------------------------
	// METHODS
	// -----------------------------
	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		return await ToDomain(fechaIngreso: DateTime.Now).Bind(async paciente => {
			if (Id is PacienteId idExistente) {
				Paciente2025Agg agg = new(idExistente, paciente);
				return await App.Repositorio.UpdatePacienteWhereId(agg);
			} else {
				//MessageBox.Show(paciente.FechaIngreso);
				//MessageBox.Show(paciente.FechaNacimiento);

				return (await App.Repositorio.InsertPacienteReturnId(paciente))
					.MatchTo<PacienteId, UnitWpf>(
						ok => {
							Id = ok;
							return new ResultWpf<UnitWpf>.Ok(UnitWpf.Valor);
						},
						error => new ResultWpf<UnitWpf>.Error(error)
					);
			}
		});
	}

	private ResultWpf<Paciente2025> ToDomain(DateTime fechaIngreso) {
		return Paciente2025.CrearResult(
				NombreCompleto2025.CrearResult(Nombre, Apellido),
				DniArgentino2025.CrearResult(Dni),
				Telefono2025.CrearResult(Telefono),
				Email2025.CrearResult(Email),
				DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(Localidad, ProvinciaArgentina2025.CrearResultPorCodigo(Provincia?.Codigo)),
				Domicilio
			),
			FechaDeNacimiento2025.CrearResult(FechaNacimiento),
			fechaIngreso
		).ToWpf();
	}




	// ================================================================
	// UTILS
	// ================================================================(propertyName));

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));

}