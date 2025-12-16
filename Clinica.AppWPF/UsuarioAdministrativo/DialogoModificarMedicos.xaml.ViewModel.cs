using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.CommonViewModels;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.AppWPF.CommonViewModels.CommonEnumsToViewModel;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public class DialogoMedicoModificarVM : INotifyPropertyChanged {

	// ================================================================
	// CONSTRUCTORES
	// ================================================================
	public DialogoMedicoModificarVM() {
		_original = new MedicoEdicionSnapshot(
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
			EspecialidadCodigo: default,
			HaceGuardias: default
		);

	}
	public DialogoMedicoModificarVM(MedicoDbModel original) {
		_original = new MedicoEdicionSnapshot(
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
			EspecialidadCodigo: original.EspecialidadCodigo,
			HaceGuardias: original.HaceGuardias
		);
		Id = original.Id;
		Dni = original.Dni;
		Nombre = original.Nombre;
		Apellido = original.Apellido;
		FechaIngreso = original.FechaIngreso;
		Email = original.Email;
		Telefono = original.Telefono;
		Especialidad = original.EspecialidadCodigo.ToViewModel();
		Domicilio = original.Domicilio;
		Localidad = original.Localidad;
		Provincia = original.ProvinciaCodigo.ToViewModel();


	}




	// ================================================================
	// COLLECTIONS
	// ================================================================

	// No administrar horarios en este VM; horarios están en ventana separada


	// ================================================================
	// READ_ONLIES
	// ================================================================

	private readonly MedicoEdicionSnapshot _original;
	public IReadOnlyList<ProvinciaVmItem> Provincias { get; } = [.. ProvinciaArgentina2025.Todas().Select(p => p.ToViewModel())];

	public ObservableCollection<EspecialidadViewModel> EspecialidadesDisponiblesItemsSource { get; } = [.. Especialidad2025.Todas.Select(x => x.ToViewModel())];

	// ================================================================
	// REGLAS
	// ================================================================

	private bool EstaCreando => Id is null;
	private bool EstaEditando => Id is not null;
	public bool PuedeEliminar => EstaEditando;
	public bool PuedeGuardarCambios => TieneCambios;


	// -----------------------------
	// PROPERTIES
	// -----------------------------

	public MedicoId? Id { get; private set; }

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


	private EspecialidadViewModel? _especialidad;
	public EspecialidadViewModel? Especialidad {
		get => _especialidad;
		set {
			_especialidad = value;
			OnPropertyChanged(nameof(Especialidad));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}


	private bool _haceGuardias;
	public bool HaceGuardias {
		get => _haceGuardias;
		set {
			_haceGuardias = value;
			OnPropertyChanged(nameof(HaceGuardias));
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
		_original.EspecialidadCodigo != Especialidad?.Codigo ||
		_original.HaceGuardias != HaceGuardias
	);

	// -----------------------------
	// METHODS
	// -----------------------------

	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		if (!PuedeGuardarCambios)
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information));

		return await this.ToDomain()
			.Bind(medico => EstaEditando
				? GuardarEdicionAsync(medico)
				: GuardarCreacionAsync(medico)
			);
	}
	private async Task<ResultWpf<UnitWpf>> GuardarEdicionAsync(Medico2025 medico) {
		if (Id is MedicoId idNotNull) {
            Medico2025Agg agg = new(idNotNull, medico);
			return await App.Repositorio.Medicos.UpdateMedicoWhereId(agg);
		} else {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No se puede guardar, la entidad no tiene MedicoId.", MessageBoxImage.Information));
		}
	}
	private async Task<ResultWpf<UnitWpf>> GuardarCreacionAsync(Medico2025 medico) {
		return (await App.Repositorio.Medicos.InsertMedicoReturnId(medico))
			.MatchTo(
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


	private ResultWpf<Medico2025> ToDomain() {
		return Medico2025.CrearResult(
			NombreCompleto2025.CrearResult(Nombre, Apellido),
			Especialidad2025.CrearResult(Especialidad?.Codigo),
			DniArgentino2025.CrearResult(Dni),
			DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(
					Localidad,
					ProvinciaArgentina2025.CrearResultPorCodigo(Provincia?.Codigo)),
				Domicilio
			),
			Telefono2025.CrearResult(Telefono),
			Email2025.CrearResult(Email),
			FechaIngreso,
			HaceGuardias
		).ToWpf(MessageBoxImage.Information);
	}


	// ================================================================
	// INFRAESTRUCTURA
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));

}





// ================================================================
// SNAPSHOTS
// ================================================================

internal record MedicoEdicionSnapshot(
	MedicoId? Id,
	EspecialidadEnum EspecialidadCodigo,
	string Dni,
	string Nombre,
	string Apellido,
	DateTime FechaIngreso,
	string Domicilio,
	string Localidad,
	ProvinciaEnum? Provincia,
	string Telefono,
	string Email,
	bool HaceGuardias
);
