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

	}


	public DialogoUsuarioModificarVM(UsuarioDbModel original) {
		_original = new UsuarioEdicionSnapshot(
			Id: original.Id,
			UserName: original.UserName,
			PasswordHash: original.PasswordHash,
			Nombre: original.Nombre,
			Apellido: original.Apellido,
			Telefono: original.Telefono,
			Email: original.Email,
			EnumRole: original.EnumRole
		);
		Id = original.Id;
		UserName = original.UserName;
		PasswordHash = original.PasswordHash;
		Nombre = original.Nombre;
		Apellido = original.Apellido;
		Telefono = original.Telefono;
		Email = original.Email;
		EnumRole = original.EnumRole;
	}


	// ================================================================
	// READ_ONLIES
	// ================================================================

	private readonly UsuarioEdicionSnapshot _original;
	public IReadOnlyList<UsuarioRoleEnum> EnumRoles { get; } = Enum.GetValues<UsuarioRoleEnum>();

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

	public UsuarioId? Id { get; private set; }

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

	private string _passwordHash = "";
	public string PasswordHash {
		get => _passwordHash;
		set {
			_passwordHash = value;
			OnPropertyChanged(nameof(PasswordHash));
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

	// -----------------------------
	// DETECTAR CAMBIOS
	// -----------------------------

	public bool TieneCambios => (
		_original.UserName != UserName ||
		_original.PasswordHash != PasswordHash ||
		_original.Nombre != Nombre ||
		_original.Apellido != Apellido ||
		_original.Telefono != Telefono ||
		_original.Email != Email ||
		_original.EnumRole != EnumRole
	);

	// -----------------------------
	// METHODS
	// -----------------------------

	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		if (!PuedeGuardarCambios)
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information));

		return await this.ToDomain()
			.Bind(usuario => EstaEditando
				? GuardarEdicionAsync(usuario)
				: GuardarCreacionAsync(usuario)
			);
	}
	private async Task<ResultWpf<UnitWpf>> GuardarEdicionAsync(Usuario2025 usuario) {
		if (Id is UsuarioId idNotNull) {
			Usuario2025Agg agg = new(idNotNull, usuario);
			return await App.Repositorio.UpdateUsuarioWhereId(agg);
		} else {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No se puede guardar, la entidad no tiene UsuarioId.", MessageBoxImage.Information));
		}
	}
	private async Task<ResultWpf<UnitWpf>> GuardarCreacionAsync(Usuario2025 usuario) {
		return (await App.Repositorio.InsertUsuarioReturnId(usuario))
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


	private ResultWpf<Usuario2025> ToDomain() {
		return Usuario2025.CrearResult(
			UserName2025.CrearResult(UserName),
			NombreCompleto2025.CrearResult(Nombre, Apellido),
			ContraseñaHasheada2025.CrearResult(PasswordHash),
			EnumRole.CrearResult(),
			Email2025.CrearResult(Email),
			Telefono2025.CrearResult(Telefono)
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

internal record UsuarioEdicionSnapshot(
	UsuarioId Id,
	string UserName,
	string PasswordHash,
	string Nombre,
	string Apellido,
	string Telefono,
	string Email,
	UsuarioRoleEnum EnumRole
) {
	public UsuarioEdicionSnapshot() : this(default, "", "", "", "", "", "", default) { }
}

