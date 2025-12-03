using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.ViewModels;

namespace Clinica.AppWPF;


public record EspecialidadMedicaDto(string UId, string Titulo);
public record MedicoSimpleDto(int Id, string Displayear);
public record DisponibilidadDto(DateTime Fecha, string Hora, string Medico);


public partial class WindowGestionTurno : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	//private readonly HorarioMedicoViewModel _agendaService;

	public ObservableCollection<EspecialidadMedicaViewModel> EspecialidadesDisponibles { get; } = [];
	public ObservableCollection<MedicoSimpleDto> MedicosEspecialistas { get; } = [];
	public ObservableCollection<DiaDeSemanaViewModel> DiasSemana { get; } = [];
	public ObservableCollection<int> Horas { get; } = [];
	public ObservableCollection<DisponibilidadDto> Disponibilidades { get; } = [];

	// Selecteds / filtros
	private string? _selectedEspecialidadUId;
	public string? SelectedEspecialidadUId {
		get => _selectedEspecialidadUId;
		set { if (_selectedEspecialidadUId == value) return; _selectedEspecialidadUId = value; OnPropertyChanged(nameof(SelectedEspecialidadUId)); OnEspecialidadChanged(); }
	}

	private int? _selectedMedicoId;
	public int? SelectedMedicoId { get => _selectedMedicoId; set { if (_selectedMedicoId == value) return; _selectedMedicoId = value; OnPropertyChanged(nameof(SelectedMedicoId)); RefreshDisponibilidades(); } }

	private int? _selectedDiaValue;
	public int? SelectedDiaValue { get => _selectedDiaValue; set { if (_selectedDiaValue == value) return; _selectedDiaValue = value; OnPropertyChanged(nameof(SelectedDiaValue)); RefreshDisponibilidades(); } }

	private int? _selectedHora;
	public int? SelectedHora { get => _selectedHora; set { if (_selectedHora == value) return; _selectedHora = value; OnPropertyChanged(nameof(SelectedHora)); RefreshDisponibilidades(); } }

	private bool _filtroDiaEnabled = false;
	public bool FiltroDiaEnabled { get => _filtroDiaEnabled; set { if (_filtroDiaEnabled == value) return; _filtroDiaEnabled = value; OnPropertyChanged(nameof(FiltroDiaEnabled)); RefreshDisponibilidades(); } }
	private bool _filtroHoraEnabled = false;
	public bool FiltroHoraEnabled { get => _filtroHoraEnabled; set { if (_filtroHoraEnabled == value) return; _filtroHoraEnabled = value; OnPropertyChanged(nameof(FiltroHoraEnabled)); RefreshDisponibilidades(); } }

	public WindowGestionTurno() {
		InitializeComponent();

        WindowGestionTurnoViewModel viewmodel = new();


		DataContext = this;
		//_agendaService = agendaService;
		LoadInitialData();
	}

	private void LoadInitialData() {
		EspecialidadesDisponibles.Clear();
		//foreach (var e in _agendaService.GetEspecialidades()) EspecialidadesDisponibles.Add(e);

		MedicosEspecialistas.Clear();
		//foreach (var m in _agendaService.GetMedicosByEspecialidad(EspecialidadesDisponibles.FirstOrDefault()?.UId ?? string.Empty)) MedicosEspecialistas.Add(m);

		DiasSemana.Clear();
		//foreach (var d in _agendaService.GetDiasSemana()) DiasSemana.Add(d);

		Horas.Clear();
		//foreach (var h in _agendaService.GetHoras()) HoursAdd(h);

		RefreshDisponibilidades();
	}

	private void HoursAdd(int h) { if (!Horas.Contains(h)) Horas.Add(h); }

	private void OnEspecialidadChanged() {
		MedicosEspecialistas.Clear();
		if (!string.IsNullOrEmpty(SelectedEspecialidadUId)) {
			//foreach (var m in _agendaService.GetMedicosByEspecialidad(SelectedEspecialidadUId)) MedicosEspecialistas.Add(m);
			SelectedMedicoId = MedicosEspecialistas.FirstOrDefault()?.Id;
		}
		RefreshDisponibilidades();
	}

	private void RefreshDisponibilidades() {
		Disponibilidades.Clear();
		//var datos = _agendaService.GetDisponibilidades(SelectedEspecialidadUId ?? string.Empty, SelectedMedicoId, FiltroDiaEnabled ? SelectedDiaValue : null, FiltroHoraEnabled ? SelectedHora : null);
		//foreach (var d in datos) Disponibilidades.Add(d);
	}

	private void ButtonCancelar(object sender, RoutedEventArgs e) => Close();
	private void ButtonReservar(object sender, RoutedEventArgs e) {
		DisponibilidadDto? seleccionado = (DisponibilidadDto?)txtDisponibilidades.SelectedItem ?? Disponibilidades.FirstOrDefault();
		if (seleccionado is null) { MessageBox.Show("No hay una disponibilidad seleccionada."); return; }
		MessageBox.Show($"Reservando: {seleccionado.Fecha:d} {seleccionado.Hora} - {seleccionado.Medico}");
		Close();
	}
}