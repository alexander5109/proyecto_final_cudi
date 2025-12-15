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
	// CONSTRUCTORES Y CONTEXTO
	// ================================================================
	public DialogoModificarHorariosVM(MedicoDbModel medico) {
		ActiveMedicoModel = medico;
		MedicoId = medico.Id;

		_ = CargarHorariosAsync(medico.Id);
	}


	// ================================================================
	// COLLECTIONS
	// ================================================================

	public ObservableCollection<ViewModelHorarioAgrupado> HorariosAgrupados { get; } = [];

	private List<HorarioDto> _snapshotOriginal = [];


	// ================================================================
	// SELECTED ITEM
	// ================================================================

	public void OnTreeSelectionChanged(object? selected) {
		if (selected is ViewModelHorarioAgrupado dia) {
			DiaSeleccionado = dia;
			HorarioSeleccionado = null;
			LimpiarFormulario();
		} else if (selected is HorarioMedicoViewModel horario) {
			HorarioSeleccionado = horario;
			DiaSeleccionado = HorariosAgrupados
				.First(g => g.DiaSemana == horario.DiaSemana);

			CargarFormularioDesdeHorario(horario);
		}
	}


	private ViewModelHorarioAgrupado? _diaSeleccionado;
	public ViewModelHorarioAgrupado? DiaSeleccionado {
		get => _diaSeleccionado;
		private set {
			_diaSeleccionado = value;
			OnPropertyChanged(nameof(DiaSeleccionado));
			OnPropertyChanged(nameof(PuedeAgregar));
		}
	}

	private HorarioMedicoViewModel? _horarioSeleccionado;
	public HorarioMedicoViewModel? HorarioSeleccionado {
		get => _horarioSeleccionado;
		private set {
			_horarioSeleccionado = value;
			OnPropertyChanged(nameof(HorarioSeleccionado));
			OnPropertyChanged(nameof(PuedeEditarHorario));
			OnPropertyChanged(nameof(PuedeEliminar));
		}
	}



	// ================================================================
	// REGLAS
	// ================================================================
	public bool PuedeAgregar => DiaSeleccionado != null && HorarioSeleccionado == null;
	public bool PuedeEditarHorario => HorarioSeleccionado != null;
	public bool PuedeEliminar => HorarioSeleccionado != null;

	public bool PuedeGuardarCambios => TieneCambios;



	// ================================================================
	// CONTEXTO DE TURNO
	// ================================================================


	public MedicoDbModel ActiveMedicoModel { get; private set; }
	public string? ActiveMedicoEspecialidad => ActiveMedicoModel?.EspecialidadCodigo.ToString();
	public string? ActiveMedicoNombreCompleto => $"{ActiveMedicoModel?.Nombre} {ActiveMedicoModel?.Apellido}";

	public MedicoId MedicoId { get; private set; }



	// -----------------------------
	// WINDOW.DETECTAR CAMBIOS
	// -----------------------------

	public bool TieneCambios { get; private set; }

	// -----------------------------
	// WINDOW.METHODS
	// -----------------------------
	public void AplicarCambios() {
		if (HorarioSeleccionado is null) return;

		HorarioSeleccionado.HoraDesde = FormHoraDesde!.Value;
		HorarioSeleccionado.HoraHasta = FormHoraHasta!.Value;
		HorarioSeleccionado.VigenteDesde = FormVigenteDesde!.Value;
		HorarioSeleccionado.VigenteHasta = FormVigenteHasta!.Value;

		TieneCambios = true;
	}


	private void LimpiarFormulario() {
		FormDia = DiaSeleccionado?.DiaSemana;
		FormHoraDesde = null;
		FormHoraHasta = null;
		FormVigenteDesde = null;
		FormVigenteHasta = null;
		OnPropertyChanged(string.Empty);
	}

	private void CargarFormularioDesdeHorario(HorarioMedicoViewModel h) {
		FormDia = h.DiaSemana;
		FormHoraDesde = h.HoraDesde;
		FormHoraHasta = h.HoraHasta;
		FormVigenteDesde = h.VigenteDesde;
		FormVigenteHasta = h.VigenteHasta;
		OnPropertyChanged(string.Empty);
	}



	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		if (!PuedeGuardarCambios)
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information));

		//if (MedicoId is not MedicoId idGood)
		//	return new ResultWpf<UnitWpf>.Error(new ErrorInfo("El médico no tiene MedicoId.", MessageBoxImage.Error));

		List<HorarioDto> horarios = [];
		foreach (var grupo in HorariosAgrupados) {
			foreach (var h in grupo.Horarios) {
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


	public async Task CargarHorariosAsync(MedicoId idGood) {
		HorariosAgrupados.Clear();

		var horarios = await App.Repositorio.SelectHorariosWhereMedicoId(idGood)
					   ?? [];

		var dict = horarios
			.GroupBy(h => h.DiaSemana)
			.ToDictionary(g => g.Key, g => g.ToList());

		foreach (DayOfWeek dia in Enum.GetValues<DayOfWeek>()) {
			dict.TryGetValue(dia, out var lista);

			HorariosAgrupados.Add(
				new ViewModelHorarioAgrupado(
					dia,
					lista ?? []
				)
			);
		}

		_snapshotOriginal = HorariosAgrupados
	.SelectMany(g => g.Horarios)
	.Select(h => h.ToDto(MedicoId!))
	.ToList();

	}


	// ================================================================
	// WINDOW.PROPIEDADES
	// ================================================================

	public DayOfWeek? FormDia { get; set; }
	public TimeOnly? FormHoraDesde { get; set; }
	public TimeOnly? FormHoraHasta { get; set; }
	public DateTime? FormVigenteDesde { get; set; }
	public DateTime? FormVigenteHasta { get; set; }


	// ================================================================
	// WINDOW.INFRAESTRUCTURA
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));

}



// ================================================================
// MINIVIEWMODELS
// ================================================================


public class ViewModelHorarioAgrupado(DayOfWeek dia, List<HorarioDbModel> horarios) {
	public DayOfWeek DiaSemana { get; } = dia;
	public string DiaSemanaNombre { get; } = dia.ATexto();
	public ObservableCollection<HorarioMedicoViewModel> Horarios { get; } = new ObservableCollection<HorarioMedicoViewModel>(
			horarios.Select(h => new HorarioMedicoViewModel(h))
		);
}

public class HorarioMedicoViewModel : INotifyPropertyChanged {

	// ================================================================
	// HORARIO_ITEM.CONSTRUCTOR
	// ================================================================
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
	public HorarioDbModel Model { get; private set; }




	// ================================================================
	// HORARIO_ITEM.PROPIEDADES
	// ================================================================

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
