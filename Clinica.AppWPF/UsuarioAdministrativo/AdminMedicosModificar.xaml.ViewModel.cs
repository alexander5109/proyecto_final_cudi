using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.CommonViewModels;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioRecepcionista;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.AppWPF.CommonViewModels.CommonEnumsToViewModel;
using static Clinica.Shared.DbModels.DbModels;
using Clinica.Shared.ApiDtos;
using static Clinica.Shared.ApiDtos.HorarioDtos;

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

	public ObservableCollection<ViewModelHorarioAgrupado> HorariosAgrupados { get; } = [];





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
	public bool PuedeEditarHorario => true; // allow editing/adding/deleting horarios in the dialog



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
		_original.HaceGuardias != HaceGuardias ||
		_
	);

	// -----------------------------
	// METHODS
	// -----------------------------

	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		if (!PuedeGuardarCambios)
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information));

		return await ToDomain(fechaIngreso: DateTime.Now)
			.Bind(medico => EstaEditando
				? GuardarEdicionAsync(medico)
				: GuardarCreacionAsync(medico)
			);
	}
	private async Task<ResultWpf<UnitWpf>> GuardarEdicionAsync(Medico2025 medico) {
		if (Id is MedicoId idNotNull) {
			var agg = new Medico2025Agg(idNotNull, medico);
			// Build horarios DTOs from HorariosAgrupados
			List<HorarioDto> horarios = [];
			foreach (var grupo in HorariosAgrupados) {
				foreach (var h in grupo.Horarios) {
					horarios.Add(new HorarioDto {
						MedicoId = idNotNull,
						DiaSemana = h.DiaSemana,
						HoraDesde = h.HoraDesde.ToTimeSpan(),
						HoraHasta = h.HoraHasta.ToTimeSpan(),
						VigenteDesde = h.VigenteDesde,
						VigenteHasta = h.VigenteHasta // now non-nullable in viewmodel
					});
				}
			}

			return await App.Repositorio.UpdateMedicoWhereIdWithHorarios(idNotNull, medico, horarios);
		} else {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No se puede guardar, la entidad no tiene Id.", MessageBoxImage.Information));
		}
	}
	private async Task<ResultWpf<UnitWpf>> GuardarCreacionAsync(Medico2025 medico) {
		return (await App.Repositorio.InsertMedicoReturnId(medico))
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


	public async Task CargarHorariosAsync() {
		HorariosAgrupados.Clear();

		if (Id is not MedicoId idGood) {
			MessageBox.Show("why is medicoid null?");
			return;
		}
		IReadOnlyList<HorarioDbModel>? horarios = await App.Repositorio.SelectHorariosWhereMedicoId(idGood);
		if (horarios == null) {
			MessageBox.Show("why is horarios null?");
			return;
		}

		IOrderedEnumerable<IGrouping<DayOfWeek, HorarioDbModel>> grupos = horarios
			.GroupBy(h => h.DiaSemana)
			.OrderBy(g => g.Key);


		foreach (IGrouping<DayOfWeek, HorarioDbModel>? grupo in grupos) {
			HorariosAgrupados.Add(
				new ViewModelHorarioAgrupado(
					grupo.Key,
					[.. grupo]
				)
			);
		}
	}



	private ResultWpf<Medico2025> ToDomain(DateTime fechaIngreso) {
		return Medico2025.CrearResult(
			NombreCompleto2025.CrearResult(Nombre, Apellido),
			Especialidad2025.CrearResult(Especialidad.Codigo),
			DniArgentino2025.CrearResult(Dni),
			DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(
					Localidad,
					ProvinciaArgentina2025.CrearResultPorCodigo(Provincia.Codigo)),
				Domicilio
			),
			Telefono2025.CrearResult(Telefono),
			Email2025.CrearResult(Email),
			//ListaHorarioMedicos2025.CrearResult(horarios),
			FechaIngreso,
			HaceGuardias
		).ToWpf();
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




// ================================================================
// MINIVIEWMODELS
// ================================================================


public class ViewModelHorarioAgrupado(DayOfWeek dia, List<HorarioDbModel> horarios) {
	public DayOfWeek DiaSemana { get; } = dia;
	public string DiaSemanaNombre { get; } = dia.ATexto();
	//public string DiaSemanaNombre { get; } = CultureInfo.GetCultureInfo("es-AR").DateTimeFormat.DayNames[dia];
	public ObservableCollection<HorarioMedicoViewModel> Horarios { get; } = new ObservableCollection<HorarioMedicoViewModel>(
			horarios.Select(h => new HorarioMedicoViewModel(h))
		);
}

public class HorarioMedicoViewModel : INotifyPropertyChanged {
	public HorarioDbModel Model { get; private set; }

	private DayOfWeek _dia;
	public DayOfWeek DiaSemana {
		get => _dia;
		set { _dia = value; OnPropertyChanged(nameof(DiaSemana)); OnPropertyChanged(nameof(Desde)); OnPropertyChanged(nameof(Hasta)); }
	}

	private TimeOnly _horaDesde;
	public TimeOnly HoraDesde {
		get => _horaDesde;
		set { _horaDesde = value; OnPropertyChanged(nameof(HoraDesde)); OnPropertyChanged(nameof(Desde)); }
	}
	private TimeOnly _horaHasta;
	public TimeOnly HoraHasta {
		get => _horaHasta;
		set { _horaHasta = value; OnPropertyChanged(nameof(HoraHasta)); OnPropertyChanged(nameof(Hasta)); }
	}

	public DateTime VigenteDesde { get; set; }
	public DateTime VigenteHasta { get; set; } // Changed to non-nullable

	public string Desde => HoraDesde.ToString("hh\\:mm");
	public string Hasta => HoraHasta.ToString("hh\\:mm");

	public HorarioMedicoViewModel(HorarioDbModel h) {
		Model = h;
		_dia = h.DiaSemana;
		_horaDesde = TimeOnly.FromTimeSpan(h.HoraDesde);
		_horaHasta = TimeOnly.FromTimeSpan(h.HoraHasta);
		VigenteDesde = h.VigenteDesde;
		VigenteHasta = h.VigenteHasta ?? DateTime.MaxValue; // coalesce nullable
	}

	public HorarioMedicoViewModel(DayOfWeek dia, TimeOnly desde, TimeOnly hasta) {
		Model = new HorarioDbModel();
		_dia = dia;
		_horaDesde = desde;
		_horaHasta = hasta;
		VigenteDesde = DateTime.Today;
		VigenteHasta = DateTime.MaxValue;
	}

	public HorarioDto ToDto(MedicoId medicoId) => new HorarioDto {
		MedicoId = medicoId,
		DiaSemana = DiaSemana,
		HoraDesde = HoraDesde.ToTimeSpan(),
		HoraHasta = HoraHasta.ToTimeSpan(),
		VigenteDesde = VigenteDesde,
		VigenteHasta = VigenteHasta
	};

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new(name));
}
