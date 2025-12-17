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
using Clinica.Dominio.TiposExtensiones;
using static Clinica.AppWPF.CommonViewModels.CommonEnumsToViewModel;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public class DialogoUsuarioModificarVM : INotifyPropertyChanged {

	// ================================================================
	// CONSTRUCTORES
	// ================================================================
	public DialogoUsuarioModificarVM() {
		_original = new UsuarioEdicionSnapshot();
		_medicosDisponibles = new ObservableCollection<MedicoVinculadoViewModel>();
		MedicoVinculadoId = null; // inicializamos el SelectedValue
	}


	public DialogoUsuarioModificarVM(UsuarioDbModel original) {
		_original = new UsuarioEdicionSnapshot(
			Id: original.Id,
			UserName: original.UserName,
			Nombre: original.Nombre,
			Apellido: original.Apellido,
			Telefono: original.Telefono,
			Email: original.Email,
			EnumRole: original.EnumRole,
			MedicoVinculadoId: original.MedicoRelacionadoId
		);

		Id = original.Id;
		UserName = original.UserName;
		Nombre = original.Nombre;
		Apellido = original.Apellido;
		Telefono = original.Telefono;
		Email = original.Email;
		EnumRole = original.EnumRole;
		MedicoVinculadoId = original.MedicoRelacionadoId; // importante
		NuevaPassword = null;

		_medicosDisponibles = new ObservableCollection<MedicoVinculadoViewModel>();
	}



	// ================================================================
	// READ_ONLIES
	// ================================================================

	private readonly UsuarioEdicionSnapshot _original;
	public IReadOnlyList<UsuarioRoleEnum> EnumRoles { get; } = Enum.GetValues<UsuarioRoleEnum>();

	public ObservableCollection<EspecialidadViewModel> EspecialidadesDisponiblesItemsSource { get; } = [.. Especialidad2025.Todas.Select(x => x.ToViewModel())];
	private ObservableCollection<MedicoVinculadoViewModel> _medicosDisponibles = new();
	public ObservableCollection<MedicoVinculadoViewModel> MedicosDisponibles {
		get => _medicosDisponibles;
		set { _medicosDisponibles = value; OnPropertyChanged(nameof(MedicosDisponibles)); }
	}


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

	public UsuarioId2025? Id { get; private set; }

	private string _userName = "";
	public string UserName {
		get => _userName;
		set {
			_userName = value;
			OnPropertyChanged(nameof(UserName));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private string? _nuevaPassword;
	public string? NuevaPassword {
		get => _nuevaPassword;
		set {
			_nuevaPassword = value;
			OnPropertyChanged(nameof(NuevaPassword));
			OnPropertyChanged(nameof(TieneCambios));
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

	private UsuarioRoleEnum? _enumRole;
	public UsuarioRoleEnum? EnumRole {
		get => _enumRole;
		set {
			_enumRole = value;
			OnPropertyChanged(nameof(EnumRole));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}


	private MedicoId2025? _medicoVinculadoId;
	public MedicoId2025? MedicoVinculadoId {
		get => _medicoVinculadoId;
		set { _medicoVinculadoId = value; OnPropertyChanged(nameof(MedicoVinculadoId)); }
	}


	//private MedicoDbModel? _medicoRelacionado;
	//public MedicoDbModel? MedicoVinculado {
	//	get => _medicoRelacionado;
	//	set {
	//		_medicoRelacionado = value;
	//		OnPropertyChanged(nameof(MedicoVinculado));
	//		OnPropertyChanged(nameof(TieneCambios));
	//		OnPropertyChanged(nameof(PuedeGuardarCambios));
	//	}
	//}

	// -----------------------------
	// DETECTAR CAMBIOS
	// -----------------------------

	public bool TieneCambios => (
		_original.UserName != UserName ||
		!string.IsNullOrWhiteSpace(NuevaPassword) || // 👈 clave
		_original.Nombre != Nombre ||
		_original.Apellido != Apellido ||
		_original.Telefono != Telefono ||
		_original.Email != Email ||
		_original.EnumRole != EnumRole
	);


	// -----------------------------
	// METHODS.PERSISTENCIA
	// -----------------------------
	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		if (!PuedeGuardarCambios)
			return new ResultWpf<UnitWpf>.Error(
				new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information)
			);
		return EstaEditando
			? await GuardarEdicionAsync()
			: await GuardarCreacionAsync();
	}
	private async Task<ResultWpf<UnitWpf>> GuardarEdicionAsync() {
		if (Id is not UsuarioId2025 id)
			return new ResultWpf<UnitWpf>.Error(
				new ErrorInfo(
					"No se puede guardar, la entidad no tiene UsuarioId2025.",
					MessageBoxImage.Information
				)
			);

		return await ToEdicionDomain()
			.Bind(edicion => {
                Usuario2025EdicionAgg agg = Usuario2025EdicionAgg.Crear(id, edicion);
				return App.Repositorio.Usuarios.UpdateUsuarioWhereId(agg);
			});
	}



	private async Task<ResultWpf<UnitWpf>> GuardarCreacionAsync() {
		return await ToCreacionDomain()
			.Bind(async usuario => {
				var result = await App.Repositorio.Usuarios.InsertUsuarioReturnId(usuario);

				return result.MatchTo(
					ok => {
						Id = ok;
						OnPropertyChanged(nameof(Id));
						return new ResultWpf<UnitWpf>.Ok(UnitWpf.Valor);
					},
					error => new ResultWpf<UnitWpf>.Error(error)
				);
			});
	}




	// ================================================================
	// METHODS.VALIDACION
	// ================================================================
	private ResultWpf<Usuario2025Edicion> ToEdicionDomain() {
		//MessageBox.Show($"ToEdicionDomain: {UserName}, {Nombre}, {Apellido}");
		return Usuario2025Edicion.CrearResult(
			UserName2025.CrearResult(UserName),
			NombreCompleto2025.CrearResult(Nombre, Apellido),
			CrearNuevaPasswordSiCorresponde(),
			EnumRole.CrearResult(),
			Email2025.CrearResult(Email),
			Telefono2025.CrearResult(Telefono),
			MedicoVinculadoId

		).ToWpf(MessageBoxImage.Information);
	}
	private ResultWpf<Usuario2025> ToCreacionDomain() {
		//MessageBox.Show($"ToCreacionDomain: {UserName}, {Nombre}, {Apellido}, {EnumRole}");
		return Usuario2025.CrearResult(
			UserName2025.CrearResult(UserName),
			NombreCompleto2025.CrearResult(Nombre, Apellido),
			ContraseñaHasheada2025.CrearResultFromRaw(NuevaPassword!), // obligatoria
			EnumRole.CrearResult(),
			Email2025.CrearResult(Email),
			Telefono2025.CrearResult(Telefono),
			MedicoVinculadoId
		).ToWpf(MessageBoxImage.Information);
	}

	private Result<ContraseñaHasheada2025?> CrearNuevaPasswordSiCorresponde() {
		if (string.IsNullOrWhiteSpace(NuevaPassword))
			return new Result<ContraseñaHasheada2025?>.Ok(null);

		return ContraseñaHasheada2025
			.CrearResultFromRaw(NuevaPassword)
			.Map(p => (ContraseñaHasheada2025?)p);
	}



	// ================================================================
	// INFRAESTRUCTURA
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));

	internal async Task RefrescarMedicosAsync() {
		await App.Repositorio.Medicos.RefreshCache();
		MedicosDisponibles.Clear();

		List<MedicoDbModel> medicosDisponibles = await App.Repositorio.Medicos.SelectMedicos();

		foreach (var medico in medicosDisponibles) {
			MedicosDisponibles.Add(new MedicoVinculadoViewModel(
				$"{medico.Nombre} {medico.Apellido} {medico.EspecialidadCodigo} ", medico.Id));
		}

		// Opcional: si el Id actual no está en la lista, lo limpiamos
		if (MedicoVinculadoId.HasValue &&
			!MedicosDisponibles.Any(m => m.Id == MedicoVinculadoId.Value)) {
			MedicoVinculadoId = null;
		}

		OnPropertyChanged(nameof(MedicosDisponibles));
	}






	// ================================================================
	// SNAPSHOTS
	// ================================================================

	public record MedicoVinculadoViewModel(string NombreCompleto, MedicoId2025 Id);

	public record UsuarioEdicionSnapshot(
		UsuarioId2025 Id,
		string UserName,
		//string PasswordHash,
		string Nombre,
		string Apellido,
		string Telefono,
		string Email,
		UsuarioRoleEnum EnumRole,
		MedicoId2025? MedicoVinculadoId
	) {
		public UsuarioEdicionSnapshot() : this(default, "", "", "", "", "", default, null) { }
	}
}
