using System.ComponentModel;
using System.Windows;
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
	// CONSTRUCTORES
	// ================================================================
	public SecretariaPacientesModificarViewModel() {
		_original = new PacienteEdicionSnapshot(
			Id: null,  // o puede dejarse como 'null' sin especificar el nombre
			Dni: "",
			Nombre: "",
			Apellido: "",
			FechaIngreso: DateTime.Today,
			Domicilio: "",
			Localidad: "",
			Provincia: null,
			Telefono: "",
			Email: "",
			FechaNacimiento: DateTime.Today
		);
	}
	public SecretariaPacientesModificarViewModel(PacienteDbModel original) {
		_original = new PacienteEdicionSnapshot(
			Id: original.Id,
			Dni: original.Dni,
			Nombre: original.Nombre,
			Apellido: original.Apellido,
			FechaIngreso: original.FechaIngreso,
			Domicilio: original.Domicilio,
			Localidad: original.Localidad,
			Provincia: original.ProvinciaCodigo,
			Telefono: original.Telefono,
			Email: original.Email,
			FechaNacimiento: original.FechaNacimiento
		);
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

	private readonly PacienteEdicionSnapshot _original;
	public IReadOnlyList<ProvinciaVmItem> Provincias { get; } = [.. ProvinciaArgentina2025.Todas().Select(p => p.ToViewModel())];


	// ================================================================
	// REGLAS
	// ================================================================

	private bool EstaCreando => Id is null;
	private bool EstaEditando => Id is not null;


	//directamente ligados a la ui
	public bool PuedeEliminar => EstaEditando;
	public bool PuedeGuardarCambios => TieneCambios;
	// -----------------------------
	// PROPERTIES
	// -----------------------------

	public PacienteId? Id { get; private set; }

	private string _dni = "";
	public string Dni {
		get => _dni;
		set {
			_dni = value;
			OnPropertyChanged(nameof(Dni));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private string _nombre = "";
	public string Nombre {
		get => _nombre;
		set {
			_nombre = value;
			OnPropertyChanged(nameof(Nombre));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private string _apellido = "";
	public string Apellido {
		get => _apellido;
		set {
			_apellido = value;
			OnPropertyChanged(nameof(Apellido));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private DateTime _fechaIngreso = DateTime.Today;

	public DateTime FechaIngreso {
		get => _fechaIngreso;
		set {
			_fechaIngreso = value;
			OnPropertyChanged(nameof(FechaIngreso));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private string _domicilio = "";
	public string Domicilio {
		get => _domicilio;
		set {
			_domicilio = value;
			OnPropertyChanged(nameof(Domicilio));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private string _localidad = "";
	public string Localidad {
		get => _localidad;
		set {
			_localidad = value;
			OnPropertyChanged(nameof(Localidad));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private ProvinciaVmItem? _provincia;
	public ProvinciaVmItem? Provincia {
		get => _provincia;
		set {
			_provincia = value;
			OnPropertyChanged(nameof(Provincia));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private string _telefono = "";
	public string Telefono {
		get => _telefono;
		set {
			_telefono = value;
			OnPropertyChanged(nameof(Telefono));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private string _email = "";
	public string Email {
		get => _email;
		set {
			_email = value;
			OnPropertyChanged(nameof(Email));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private DateTime _fechaNacimiento = DateTime.Today;
	public DateTime FechaNacimiento {
		get => _fechaNacimiento;
		set {
			_fechaNacimiento = value;
			OnPropertyChanged(nameof(FechaNacimiento));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}


	// -----------------------------
	// DETECTAR CAMBIOS
	// -----------------------------

	public bool TieneCambios => (
		_original.Dni != Dni ||
		_original.Nombre != Nombre ||
		_original.Apellido != Apellido ||
		_original.Domicilio != Domicilio ||
		_original.Localidad != Localidad ||
		_original.Provincia != Provincia?.Codigo ||
		_original.Telefono != Telefono ||
		_original.Email != Email ||
		_original.FechaNacimiento != FechaNacimiento
	);


	// -----------------------------
	// METHODS
	// -----------------------------

	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		if (!PuedeGuardarCambios)
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information));

		return await ToDomain(fechaIngreso: DateTime.Now)
			.Bind(paciente => EstaEditando
				? GuardarEdicionAsync(paciente)
				: GuardarCreacionAsync(paciente)
			);
	}
	private async Task<ResultWpf<UnitWpf>> GuardarEdicionAsync(Paciente2025 paciente) {
		if (Id is PacienteId idNotNull) {
			var agg = new Paciente2025Agg(idNotNull, paciente);
			return await App.Repositorio.UpdatePacienteWhereId(agg);
		} else {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No se puede guardar, la entidad no tiene Id.", MessageBoxImage.Information));
		}
	}
	private async Task<ResultWpf<UnitWpf>> GuardarCreacionAsync(Paciente2025 paciente) {
		return (await App.Repositorio.InsertPacienteReturnId(paciente))
			.MatchTo<PacienteId, UnitWpf>(
				ok => {
					Id = ok;
					OnPropertyChanged(nameof(Id));
					OnPropertyChanged(nameof(EstaCreando));
					OnPropertyChanged(nameof(EstaEditando));
					OnPropertyChanged(nameof(PuedeEliminar));
					return new ResultWpf<UnitWpf>.Ok(UnitWpf.Valor);
				},
				error => new ResultWpf<UnitWpf>.Error(error)
			);
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
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));




}

// ================================================================
// SNAPSHOTS
// ================================================================

internal record PacienteEdicionSnapshot(
	PacienteId? Id,
	string Dni,
	string Nombre,
	string Apellido,
	DateTime FechaIngreso,
	string Domicilio,
	string Localidad,
	ProvinciaCodigo? Provincia,
	string Telefono,
	string Email,
	DateTime FechaNacimiento
);
