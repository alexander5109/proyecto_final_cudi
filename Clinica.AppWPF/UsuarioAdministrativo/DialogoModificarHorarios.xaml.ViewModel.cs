using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
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
using Clinica.Shared.ApiDtos;
using static Clinica.AppWPF.CommonViewModels.CommonEnumsToViewModel;
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
		_ = CargarHorariosAsync(medico.Id);
	}



	// ================================================================
	// WINDOW.CONTEXTO
	// ================================================================


	public MedicoDbModel ActiveMedicoModel { get; private set; }
	public MedicoId MedicoId { get; private set; }
	public string? ActiveMedicoEspecialidad => ActiveMedicoModel?.EspecialidadCodigo.ToString();
	public string? ActiveMedicoNombreCompleto => $"{ActiveMedicoModel?.Nombre} {ActiveMedicoModel?.Apellido}";



	// ================================================================
	// WINDOW.COLLECTIONS
	// ================================================================

	public ObservableCollection<NodoDiaSemanaViewModel> HorariosAgrupados { get; } = [];

	private List<HorarioDto> _snapshotOriginal = [];


	// ================================================================
	// WINDOW.SELECTED ITEM
	// ================================================================


	private NodoDiaSemanaViewModel? _diaSeleccionado;
	public NodoDiaSemanaViewModel? DiaSeleccionado {
		get => _diaSeleccionado;
		private set {
			_diaSeleccionado = value;
			OnPropertyChanged(nameof(DiaSeleccionado));
			OnPropertyChanged(nameof(PuedeAgregar));
		}
	}

	private NodoFranjaHorariaViewModel? _horarioSeleccionado;
	public NodoFranjaHorariaViewModel? HorarioSeleccionado {
		get => _horarioSeleccionado;
		private set {
			_horarioSeleccionado = value;
			OnPropertyChanged(nameof(HorarioSeleccionado));
			OnPropertyChanged(nameof(PuedeEditarHorario));
			OnPropertyChanged(nameof(PuedeEliminar));
		}
	}



	// ================================================================
	// WINDOW.SELECTED FORM
	// ================================================================


	private DayOfWeek? _formDia;
	public DayOfWeek? FormDia {
		get => _formDia;
		set {
			if (_formDia != value) {
				_formDia = value;
				OnPropertyChanged(nameof(FormDia));
				OnPropertyChanged(nameof(PuedeAplicar));
			}
		}
	}


	private TimeOnly? _formHoraDesde;
	public TimeOnly? FormHoraDesde {
		get => _formHoraDesde;
		set {
			if (_formHoraDesde != value) {
				_formHoraDesde = value;
				OnPropertyChanged(nameof(FormHoraDesde));
				OnPropertyChanged(nameof(PuedeAplicar));
			}
		}
	}

	private TimeOnly? _formHoraHasta;
	public TimeOnly? FormHoraHasta {
		get => _formHoraHasta;
		set {
			if (_formHoraHasta != value) {
				_formHoraHasta = value;
				OnPropertyChanged(nameof(FormHoraHasta));
				OnPropertyChanged(nameof(PuedeAplicar));
			}
		}
	}



	private DateTime? _formVigenteDesde;
	public DateTime? FormVigenteDesde {
		get => _formVigenteDesde;
		set {
			if (_formVigenteDesde != value) {
				_formVigenteDesde = value;
				OnPropertyChanged(nameof(FormVigenteDesde));
			}
		}
	}

	private DateTime? _formVigenteHasta;
	public DateTime? FormVigenteHasta {
		get => _formVigenteHasta;
		set {
			if (_formVigenteHasta != value) {
				_formVigenteHasta = value;
				OnPropertyChanged(nameof(FormVigenteHasta));
			}
		}
	}


	// ================================================================
	// WINDOW.REGLAS
	// ================================================================

	public bool PuedeAgregar => DiaSeleccionado != null && HorarioSeleccionado == null;
	public bool PuedeEditarHorario => HorarioSeleccionado != null;
	public bool PuedeEliminar => HorarioSeleccionado != null;

	public bool PuedeGuardarCambios => TieneCambios;

	public bool PuedeAplicar => HorarioSeleccionado != null &&
		FormHoraDesde.HasValue &&
		FormHoraHasta.HasValue &&
		FormHoraDesde < FormHoraHasta;

	// -----------------------------
	// WINDOW.DETECTAR CAMBIOS
	// -----------------------------

	public bool TieneCambios =>
		!HorariosIguales(
			_snapshotOriginal,
			HorariosAgrupados
				.SelectMany(g => g.Horarios)
				.Select(h => h.ToDto(MedicoId))
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



	private void LimpiarFormulario() {
		FormDia = DiaSeleccionado?.DiaSemana;
		FormHoraDesde = null;
		FormHoraHasta = null;
		FormVigenteDesde = null;
		FormVigenteHasta = null;
	}

	private void CargarFormularioDesdeHorario(NodoFranjaHorariaViewModel h) {
		FormDia = h.DiaSemana;
		FormHoraDesde = h.HoraDesde;
		FormHoraHasta = h.HoraHasta;
		FormVigenteDesde = h.VigenteDesde;
		FormVigenteHasta = h.VigenteHasta;
	}



	// -----------------------------
	// WINDOW.METHODS.PUBLIC
	// -----------------------------

	public void OnTreeSelectionChanged(object? selected) {
		if (selected is NodoDiaSemanaViewModel dia) {
			DiaSeleccionado = dia;
			HorarioSeleccionado = null;
			LimpiarFormulario();
		} else if (selected is NodoFranjaHorariaViewModel horario) {
			HorarioSeleccionado = horario;
			DiaSeleccionado = HorariosAgrupados
				.First(g => g.DiaSemana == horario.DiaSemana);

			CargarFormularioDesdeHorario(horario);
		}
	}


	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		if (!PuedeGuardarCambios)
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information));

		//if (MedicoId is not MedicoId idGood)
		//	return new ResultWpf<UnitWpf>.Error(new ErrorInfo("El médico no tiene MedicoId.", MessageBoxImage.Error));

		List<HorarioDto> horarios = [];
		foreach (NodoDiaSemanaViewModel grupo in HorariosAgrupados) {
			foreach (NodoFranjaHorariaViewModel h in grupo.Horarios) {
				horarios.Add(new HorarioDto {
					MedicoId = MedicoId,
					DiaSemana = h.DiaSemana,
					HoraDesde = h.HoraDesde.ToTimeSpan(),
					HoraHasta = h.HoraHasta.ToTimeSpan(),
					VigenteDesde = h.VigenteDesde,
					VigenteHasta = h.VigenteHasta
				});
			}
		}

		// call repository method that updates only horarios for a medico
		return await App.Repositorio.UpdateHorariosWhereMedicoId(MedicoId, horarios);
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
		CargarFormularioDesdeHorario(nuevo);

		OnPropertyChanged(nameof(TieneCambios));
		OnPropertyChanged(nameof(PuedeGuardarCambios));
	}

	public void EliminarHorario() {
		if (HorarioSeleccionado is null) return;
		if (DiaSeleccionado is null) return;

		DiaSeleccionado.Horarios.Remove(HorarioSeleccionado);

		HorarioSeleccionado = null;
		LimpiarFormulario();

		OnPropertyChanged(nameof(TieneCambios));
		OnPropertyChanged(nameof(PuedeGuardarCambios));
	}

	public void AplicarCambios() {
		if (HorarioSeleccionado is null) return;

		HorarioSeleccionado.HoraDesde = FormHoraDesde!.Value;
		HorarioSeleccionado.HoraHasta = FormHoraHasta!.Value;
		HorarioSeleccionado.VigenteDesde = FormVigenteDesde!.Value;
		HorarioSeleccionado.VigenteHasta = FormVigenteHasta!.Value;

		OnPropertyChanged(nameof(TieneCambios));
		OnPropertyChanged(nameof(PuedeGuardarCambios));
	}



	public async Task CargarHorariosAsync(MedicoId idGood) {
		HorariosAgrupados.Clear();

        IReadOnlyList<HorarioDbModel> horarios = await App.Repositorio.SelectHorariosWhereMedicoId(idGood)
					   ?? [];

		Dictionary<DayOfWeek, List<HorarioDbModel>> dict = horarios
			.GroupBy(h => h.DiaSemana)
			.ToDictionary(g => g.Key, g => g.ToList());

		foreach (DayOfWeek dia in Enum.GetValues<DayOfWeek>()) {
			dict.TryGetValue(dia, out List<HorarioDbModel>? lista);

			HorariosAgrupados.Add(
				new NodoDiaSemanaViewModel(
					dia,
					lista ?? []
				)
			);
		}

		_snapshotOriginal = [.. HorariosAgrupados.SelectMany(g => g.Horarios).Select(h => h.ToDto(MedicoId))];
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


public class NodoDiaSemanaViewModel(DayOfWeek dia, List<HorarioDbModel> horarios) {

	// ================================================================
	// GROUPER_DIA.CONSTRUCTOR
	// ================================================================
	public DayOfWeek DiaSemana { get; } = dia;
	public string DiaSemanaNombre { get; } = dia.ATexto();
	public ObservableCollection<NodoFranjaHorariaViewModel> Horarios { get; } = new ObservableCollection<NodoFranjaHorariaViewModel>(
			horarios.Select(h => new NodoFranjaHorariaViewModel(h))
		);
}

public class NodoFranjaHorariaViewModel : INotifyPropertyChanged {

	// ================================================================
	// HORARIO_ITEM.CONSTRUCTOR
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
	// HORARIO_ITEM.PROPIEDADES
	// ================================================================

	private DayOfWeek _dia;
	public DayOfWeek DiaSemana {
		get => _dia;
		set {
			_dia = value;
			OnPropertyChanged(nameof(DiaSemana));
			OnPropertyChanged(nameof(Desde));
			OnPropertyChanged(nameof(Hasta));
		}
	}

	private TimeOnly _horaDesde;
	public TimeOnly HoraDesde {
		get => _horaDesde;
		set {
			_horaDesde = value;
			OnPropertyChanged(nameof(HoraDesde));
			OnPropertyChanged(nameof(Desde));
		}
	}
	private TimeOnly _horaHasta;
	public TimeOnly HoraHasta {
		get => _horaHasta;
		set {
			_horaHasta = value;
			OnPropertyChanged(nameof(HoraHasta));
			OnPropertyChanged(nameof(Hasta));
		}
	}

	public DateTime VigenteDesde { get; set; }
	public DateTime VigenteHasta { get; set; } // Changed to non-nullable

	public string Desde => HoraDesde.ToString("hh\\:mm");
	public string Hasta => HoraHasta.ToString("hh\\:mm");



	// ================================================================
	// HORARIO_ITEM.METHODS
	// ================================================================
	public HorarioDto ToDto(MedicoId medicoId) => new() {
		MedicoId = medicoId,
		DiaSemana = DiaSemana,
		HoraDesde = HoraDesde.ToTimeSpan(),
		HoraHasta = HoraHasta.ToTimeSpan(),
		VigenteDesde = VigenteDesde,
		VigenteHasta = VigenteHasta
	};

	// ================================================================
	// HORARIO_ITEM.INFRAESTRUCTURA
	// ================================================================
	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new(name));
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
