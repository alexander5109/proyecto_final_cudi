using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.Shared.ApiDtos.HorarioDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public class DialogoModificarHorariosVM : INotifyPropertyChanged {

	// ================================================================
	// WINDOW.CONSTRUCTOR
	// ================================================================
	public DialogoModificarHorariosVM(MedicoDbModel medico) {
		ActiveMedicoModel = medico;
		MedicoId = medico.Id;
		_ = CargarHorariosAsync();
	}



	// ================================================================
	// WINDOW.COLLECTIONS
	// ================================================================

	public ObservableCollection<NodoDiaSemanaViewModel> HorariosAgrupados { get; } = [];

	private List<HorarioDto> _snapshotOriginal = [];



	// ================================================================
	// WINDOW.CONTEXTO
	// ================================================================


	public MedicoDbModel ActiveMedicoModel { get; private set; }
	public MedicoId MedicoId { get; private set; }
	public string? ActiveMedicoEspecialidad => ActiveMedicoModel?.EspecialidadCodigo.ToString();
	public string? ActiveMedicoNombreCompleto => $"{ActiveMedicoModel?.Nombre} {ActiveMedicoModel?.Apellido}";


	// ================================================================
	// WINDOW.SELECTED ITEM
	// ================================================================
	private void OnHorarioEditado() {
		OnPropertyChanged(nameof(TieneCambios));
		OnPropertyChanged(nameof(PuedeGuardarCambios));
		OnPropertyChanged(nameof(PuedeRestaurar));
	}


	private NodoDiaSemanaViewModel? _diaSeleccionado;
	public NodoDiaSemanaViewModel? DiaSeleccionado {
		get => _diaSeleccionado;
		private set {
			_diaSeleccionado = value;
			OnPropertyChanged(nameof(DiaSeleccionado));

			OnPropertyChanged(nameof(PuedeEliminar));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeAgregar));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}

	private NodoFranjaHorariaViewModel? _horarioSeleccionado;
	public NodoFranjaHorariaViewModel? HorarioSeleccionado {
		get => _horarioSeleccionado;
		private set {
			_horarioSeleccionado = value;
			OnPropertyChanged(nameof(HorarioSeleccionado));
			//OnPropertyChanged(nameof(PuedeEditarHorario));

			OnPropertyChanged(nameof(PuedeEliminar));
			OnPropertyChanged(nameof(TieneCambios));
			OnPropertyChanged(nameof(PuedeAgregar));
			OnPropertyChanged(nameof(PuedeGuardarCambios));
		}
	}


	// ================================================================
	// WINDOW.SELECTED FORM
	// ================================================================

	//private DayOfWeek? _formDia;
	//public DayOfWeek? FormDia {
	//	get => _formDia;
	//	set {
	//		if (_formDia != value) {
	//			_formDia = value;
	//			OnPropertyChanged(nameof(FormDia));
	//			//OnPropertyChanged(nameof(PuedeAplicar));
	//		}
	//	}
	//}


	//private TimeOnly? _formHoraDesde;
	//public TimeOnly? FormHoraDesde {
	//	get => _formHoraDesde;
	//	set {
	//		if (_formHoraDesde != value) {
	//			_formHoraDesde = value;
	//			OnPropertyChanged(nameof(FormHoraDesde));
	//			//OnPropertyChanged(nameof(PuedeAplicar));
	//		}
	//	}
	//}

	//private TimeOnly? _formHoraHasta;
	//public TimeOnly? FormHoraHasta {
	//	get => _formHoraHasta;
	//	set {
	//		if (_formHoraHasta != value) {
	//			_formHoraHasta = value;
	//			OnPropertyChanged(nameof(FormHoraHasta));
	//			//OnPropertyChanged(nameof(PuedeAplicar));
	//		}
	//	}
	//}



	//private DateTime? _formVigenteDesde;
	//public DateTime? FormVigenteDesde {
	//	get => _formVigenteDesde;
	//	set {
	//		if (_formVigenteDesde != value) {
	//			_formVigenteDesde = value;
	//			OnPropertyChanged(nameof(FormVigenteDesde));
	//		}
	//	}
	//}

	//private DateTime? _formVigenteHasta;
	//public DateTime? FormVigenteHasta {
	//	get => _formVigenteHasta;
	//	set {
	//		if (_formVigenteHasta != value) {
	//			_formVigenteHasta = value;
	//			OnPropertyChanged(nameof(FormVigenteHasta));
	//		}
	//	}
	//}


	// ================================================================
	// WINDOW.REGLAS
	// ================================================================
	public bool PuedeRestaurar => TieneCambios;
	public bool PuedeAgregar => DiaSeleccionado != null;
	//public bool PuedeEditarHorario => HorarioSeleccionado != null;
	public bool PuedeEliminar => HorarioSeleccionado != null;

	public bool PuedeGuardarCambios => TieneCambios;

	//public bool PuedeAplicar => HorarioSeleccionado != null &&
	//	FormHoraDesde.HasValue &&
	//	FormHoraHasta.HasValue &&
	//	FormHoraDesde < FormHoraHasta;

	// -----------------------------
	// WINDOW.DETECTAR CAMBIOS
	// -----------------------------

	public bool TieneCambios =>
		!HorariosIguales(
			_snapshotOriginal,
			HorariosAgrupados
				.SelectMany(g => g.Horarios)
				.Select(h => h.ToDto())
		);
	// -----------------------------
	// WINDOW.METHODS.PRIVATE
	// -----------------------------
	private static bool HorariosIguales(
		IEnumerable<HorarioDto> a,
		IEnumerable<HorarioDto> b) {
		HorarioDtoComparer comparer = new();

		return a.Count() == b.Count()
			&& !a.Except(b, comparer).Any()
			&& !b.Except(a, comparer).Any();
	}



	//private void LimpiarFormulario() {
	//FormDia = DiaSeleccionado?.DiaSemana;
	//FormHoraDesde = null;
	//FormHoraHasta = null;
	//FormVigenteDesde = null;
	//FormVigenteHasta = null;
	//}

	//private void CargarFormularioDesdeHorario(NodoFranjaHorariaViewModel h) {
	//FormDia = h.DiaSemana;
	//FormHoraDesde = h.HoraDesde;
	//FormHoraHasta = h.HoraHasta;
	//FormVigenteDesde = h.VigenteDesde;
	//FormVigenteHasta = h.VigenteHasta;
	//}



	// -----------------------------
	// WINDOW.METHODS.PUBLIC
	// -----------------------------
	public async Task RestaurarAsync() {
		if (!TieneCambios) return;

		if (MessageBox.Show(
			"¿Descartar todos los cambios?",
			"Restaurar horarios",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning
		) != MessageBoxResult.Yes)
			return;

		await CargarHorariosAsync();
	}


	private Result<HorariosMedicos2026Agg> ConstruirAgregadoDominio() {

		List<Result<HorarioFranja2026>> resultados = [.. HorariosAgrupados
			.SelectMany(d => d.Horarios)
			.Select(h => HorarioFranja2026.CrearResult(
				h.DiaSemana,
				h.HoraDesde,
				h.HoraHasta,
				DateOnly.FromDateTime(h.VigenteDesde),
				h.VigenteHasta == DateTime.MaxValue? null: DateOnly.FromDateTime(h.VigenteHasta)
			))
		];

		List<Result<HorarioFranja2026>> errores = [.. resultados.Where(r => r.IsError)];

		if (errores.Count != 0)
			return new Result<HorariosMedicos2026Agg>.Error(
				string.Join(
					"\n",
					errores.Select(e => e.UnwrapAsError())
				)
			);

		IReadOnlyCollection<HorarioFranja2026> franjasValidas = [.. resultados.Select(r => r.UnwrapAsOk())];

		return HorariosMedicos2026Agg.CrearResult(MedicoId, franjasValidas);
	}



	public void OnTreeSelectionChanged(object? selected) {
		if (selected is NodoDiaSemanaViewModel dia) {
			DiaSeleccionado = dia;
			HorarioSeleccionado = null;
			//LimpiarFormulario();
		} else if (selected is NodoFranjaHorariaViewModel horario) {
			HorarioSeleccionado = horario;
			DiaSeleccionado = HorariosAgrupados
				.First(g => g.DiaSemana == horario.DiaSemana);

			//CargarFormularioDesdeHorario(horario);
		}
	}


	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		if (!PuedeGuardarCambios)
			return new ResultWpf<UnitWpf>.Error(
				new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information)
			);

		ResultWpf<HorariosMedicos2026Agg> resultadoAgg = ConstruirAgregadoDominio().ToWpf(MessageBoxImage.Warning);
		if (!resultadoAgg.IsError) {
			if (MessageBox.Show(
				"¿Esta seguro de guardar los cambios realizados?",
				"Confirmar guardado",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question
			) != MessageBoxResult.Yes)
				return new ResultWpf<UnitWpf>.Ok(UnitWpf.Valor);
		}

		return await resultadoAgg.Bind(
			agregado => App.Repositorio.UpdateHorariosWhereMedicoId(agregado)
		);
	}


	public void AgregarHorario() {
		if (DiaSeleccionado is null) return;

		NodoFranjaHorariaViewModel nuevo = new(
			DiaSeleccionado.DiaSemana,
			ClinicaNegocio.Atencion.DesdeHs,
			ClinicaNegocio.Atencion.HastaHs,
			DateTime.Today,
			DateTime.Today.AddYears(10)
		);

		DiaSeleccionado.Horarios.Add(nuevo);

		HorarioSeleccionado = nuevo;
		//CargarFormularioDesdeHorario(nuevo);

		OnPropertyChanged(nameof(TieneCambios));
		OnPropertyChanged(nameof(PuedeGuardarCambios));
	}

	public void EliminarHorario() {
		if (HorarioSeleccionado is null) return;
		if (DiaSeleccionado is null) return;

		DiaSeleccionado.Horarios.Remove(HorarioSeleccionado);

		HorarioSeleccionado = null;
		//LimpiarFormulario();

		OnPropertyChanged(nameof(TieneCambios));
		OnPropertyChanged(nameof(PuedeGuardarCambios));
	}

	//public void AplicarCambios() {
	//	if (HorarioSeleccionado is null) return;

	//HorarioSeleccionado.HoraDesde = FormHoraDesde!.Value;
	//HorarioSeleccionado.HoraHasta = FormHoraHasta!.Value;
	//HorarioSeleccionado.VigenteDesde = FormVigenteDesde!.Value;
	//HorarioSeleccionado.VigenteHasta = FormVigenteHasta!.Value;

	//	OnPropertyChanged(nameof(TieneCambios));
	//	OnPropertyChanged(nameof(PuedeGuardarCambios));
	//}



	public async Task CargarHorariosAsync() {
		HorariosAgrupados.Clear();

		IReadOnlyList<HorarioDbModel> horarios = await App.Repositorio.SelectHorariosWhereMedicoId(MedicoId)
					   ?? [];

		Dictionary<DayOfWeek, List<HorarioDbModel>> dict = horarios
			.GroupBy(h => h.DiaSemana)
			.ToDictionary(g => g.Key, g => g.ToList());

		foreach (DayOfWeek dia in Enum.GetValues<DayOfWeek>()) {
			dict.TryGetValue(dia, out List<HorarioDbModel>? lista);

			HorariosAgrupados.Add(
				new NodoDiaSemanaViewModel(
					dia,
					lista ?? [],
					OnHorarioEditado
				)
			);
		}

		//foreach (var h in Horarios) {
		//	h.OnEdited = OnHorarioEditado;
		//}
		_snapshotOriginal = [.. HorariosAgrupados.SelectMany(g => g.Horarios).Select(h => h.ToDto())];
	}



	// ================================================================
	// WINDOW.INFRAESTRUCTURA
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));

}



// ================================================================
// MINIVIEWMODELS
// ================================================================

public class NodoDiaSemanaViewModel(DayOfWeek dia, List<HorarioDbModel> horarios, Action onHorarioEditado) {

	// ================================================================
	// NODO_DIA.PROPS
	// ================================================================
	public DayOfWeek DiaSemana { get; } = dia;
	public string DiaSemanaNombre { get; } = dia.ATexto();
	public ObservableCollection<NodoFranjaHorariaViewModel> Horarios { get; } = new ObservableCollection<NodoFranjaHorariaViewModel>(
			horarios.Select(h => {
                NodoFranjaHorariaViewModel vm = new NodoFranjaHorariaViewModel(h) {
					OnEdited = onHorarioEditado
				};
				return vm;
			})
		);
}

public class NodoFranjaHorariaViewModel : INotifyPropertyChanged {

	// ================================================================
	// NODO_FRANJA.CONSTRUCTOR
	// ================================================================
	public NodoFranjaHorariaViewModel(HorarioDbModel h) {
		Model = h;
		_dia = h.DiaSemana;
		_horaDesde = TimeOnly.FromTimeSpan(h.HoraDesde);
		_horaHasta = TimeOnly.FromTimeSpan(h.HoraHasta);
		VigenteDesde = h.VigenteDesde;
		VigenteHasta = h.VigenteHasta ?? DateTime.MaxValue; // coalesce nullable
	}

	public NodoFranjaHorariaViewModel(DayOfWeek dia, TimeOnly desde, TimeOnly hasta, DateTime vigenteDesde, DateTime vigenteHasta) {
		Model = new HorarioDbModel();
		_dia = dia;
		_horaDesde = desde;
		_horaHasta = hasta;
		VigenteDesde = vigenteDesde;
		VigenteHasta = vigenteHasta;
	}
	public HorarioDbModel Model { get; private set; }




	// ================================================================
	// NODO_FRANJA.PROPS
	// ================================================================

	private DayOfWeek _dia;
	public DayOfWeek DiaSemana {
		get => _dia;
		set {
			_dia = value;
			OnPropertyChanged(nameof(DiaSemana));
			OnPropertyChanged(nameof(HoraDesde));
			OnPropertyChanged(nameof(HoraHasta));
			NotifyEdit();
		}
	}

	private TimeOnly _horaDesde;
	public TimeOnly HoraDesde {
		get => _horaDesde;
		set {
			_horaDesde = value;
			OnPropertyChanged(nameof(HoraDesde));
			NotifyEdit();
		}
	}
	private TimeOnly _horaHasta;
	public TimeOnly HoraHasta {
		get => _horaHasta;
		set {
			_horaHasta = value;
			OnPropertyChanged(nameof(HoraHasta));
			NotifyEdit();
		}
	}


	private DateTime _vigenteDesde;
	public DateTime VigenteDesde {
		get => _vigenteDesde;
		set {
			_vigenteDesde = value;
			OnPropertyChanged(nameof(VigenteDesde));
			NotifyEdit();
		}
	}
	private DateTime _vigenteHasta;
	public DateTime VigenteHasta {
		get => _vigenteHasta;
		set {
			_vigenteHasta = value;
			OnPropertyChanged(nameof(VigenteHasta));
			NotifyEdit();
		}
	}

	// ================================================================
	// NODO_FRANJA.INFRAESTRUCTURA
	// ================================================================
	public Action? OnEdited { get; set; }
	private void NotifyEdit() => OnEdited?.Invoke();

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new(name));



	// ================================================================
	// NODO_FRANJA.METHODS
	// ================================================================
	public HorarioDto ToDto() => new() {
		//MedicoId = medicoId,
		DiaSemana = DiaSemana,
		HoraDesde = HoraDesde.ToTimeSpan(),
		HoraHasta = HoraHasta.ToTimeSpan(),
		VigenteDesde = VigenteDesde,
		VigenteHasta = VigenteHasta
	};
}




public sealed class HorarioDtoComparer : IEqualityComparer<HorarioDto> {
	public bool Equals(HorarioDto? x, HorarioDto? y) {
		if (ReferenceEquals(x, y)) return true;
		if (x is null || y is null) return false;

		return
			x.DiaSemana == y.DiaSemana &&
			x.HoraDesde == y.HoraDesde &&
			x.HoraHasta == y.HoraHasta &&
			x.VigenteDesde.Date == y.VigenteDesde.Date &&
			NullableEquals(x.VigenteHasta, y.VigenteHasta);
	}

	public int GetHashCode(HorarioDto obj) {
		unchecked {
			int hash = 17;
			hash = hash * 23 + obj.DiaSemana.GetHashCode();
			hash = hash * 23 + obj.HoraDesde.GetHashCode();
			hash = hash * 23 + obj.HoraHasta.GetHashCode();
			hash = hash * 23 + obj.VigenteDesde.Date.GetHashCode();
			hash = hash * 23 + (obj.VigenteHasta?.Date.GetHashCode() ?? 0);
			return hash;
		}
	}

	private static bool NullableEquals(DateTime? a, DateTime? b)
		=> a?.Date == b?.Date;
}


public class TimeOnly24hConverter : IValueConverter {
	private const string Format = "HH:mm";

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
		if (value is TimeOnly time)
			return time.ToString(Format);

		return string.Empty;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
		if (value is string text &&
			TimeOnly.TryParseExact(text, Format, culture, DateTimeStyles.None, out var time)) {
			return time;
		}

		return Binding.DoNothing;
	}
}