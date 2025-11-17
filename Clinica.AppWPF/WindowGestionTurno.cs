using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Clinica.AppWPF;

public partial class WindowGestionTurno : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	// View-model simple types (internos, auto-contenidos para demo)
	public record EspecialidadMedicaViewModel(string UId, string Titulo);
	public record MedicoSimpleViewModel(string Id, string Displayear);
	public record ModelViewDiaSemana(int Value, string NombreDia);
	public record DisponibilidadEspecialidadModelView(DateTime Fecha, string Hora, string Medico);


	// Bindables
	public ObservableCollection<EspecialidadMedicaViewModel> EspecialidadesDisponibles { get; } = [];
    public ObservableCollection<MedicoSimpleViewModel> MedicosEspecialistas { get; } = [];
    public ObservableCollection<ModelViewDiaSemana> DiasSemana { get; } = [];
	public ObservableCollection<int> Horas { get; } = [];
	public ObservableCollection<DisponibilidadEspecialidadModelView> Disponibilidades { get; } = [];

	// Selecteds / filtros
	private string? _selectedEspecialidadUId;
	public string? SelectedEspecialidadUId {
		get => _selectedEspecialidadUId;
		set {
			if (_selectedEspecialidadUId == value) return;
			_selectedEspecialidadUId = value;
			OnPropertyChanged(nameof(SelectedEspecialidadUId));
			Console.WriteLine($"[UI] Especialidad seleccionada: {_selectedEspecialidadUId}");
			LoadMedicosPorEspecialidad(value);
			RefreshDisponibilidades();
		}
	}

	private string? _selectedMedicoId;
	public string? SelectedMedicoId {
		get => _selectedMedicoId;
		set {
			if (_selectedMedicoId == value) return;
			_selectedMedicoId = value;
			OnPropertyChanged(nameof(SelectedMedicoId));
			Console.WriteLine($"[UI] Médico seleccionado: {_selectedMedicoId}");
			RefreshDisponibilidades();
		}
	}

	private int? _selectedDiaValue;
	public int? SelectedDiaValue {
		get => _selectedDiaValue;
		set {
			if (_selectedDiaValue == value) return;
			_selectedDiaValue = value;
			OnPropertyChanged(nameof(SelectedDiaValue));
			Console.WriteLine($"[UI] Día seleccionado: {_selectedDiaValue}");
			RefreshDisponibilidades();
		}
	}

	private int? _selectedHora;
	public int? SelectedHora {
		get => _selectedHora;
		set {
			if (_selectedHora == value) return;
			_selectedHora = value;
			OnPropertyChanged(nameof(SelectedHora));
			Console.WriteLine($"[UI] Hora seleccionada: {_selectedHora}");
			RefreshDisponibilidades();
		}
	}

	private bool _filtroDiaEnabled = false;
	public bool FiltroDiaEnabled {
		get => _filtroDiaEnabled;
		set { if (_filtroDiaEnabled == value) return; _filtroDiaEnabled = value; OnPropertyChanged(nameof(FiltroDiaEnabled)); RefreshDisponibilidades(); }
	}

	private bool _filtroHoraEnabled = false;
	public bool FiltroHoraEnabled {
		get => _filtroHoraEnabled;
		set { if (_filtroHoraEnabled == value) return; _filtroHoraEnabled = value; OnPropertyChanged(nameof(FiltroHoraEnabled)); RefreshDisponibilidades(); }
	}

	// Constructor
	public WindowGestionTurno() {
		InitializeComponent();
		DataContext = this;
		SeedData(); // llenar con ejemplos si no hay repos
	}

	// Llenado de datos (hardcode demo). En producción reemplazar con llamados a repositorios/App.BaseDeDatos.
	private void SeedData() {
		// Especialidades demo
		var espList = new[] {
			new EspecialidadMedicaViewModel("esp-gastro", "Gastroenterólogo"),
			new EspecialidadMedicaViewModel("esp-psico", "Psicólogo"),
			new EspecialidadMedicaViewModel("esp-derma", "Dermatólogo")
		};
		foreach (var e in espList) EspecialidadesDisponibles.Add(e);

		// Dias de la semana (ModelViewDiaSemana)
		var dias = new[] {
			new ModelViewDiaSemana(0, "Domingo"),
			new ModelViewDiaSemana(1, "Lunes"),
			new ModelViewDiaSemana(2, "Martes"),
			new ModelViewDiaSemana(3, "Miércoles"),
			new ModelViewDiaSemana(4, "Jueves"),
			new ModelViewDiaSemana(5, "Viernes"),
			new ModelViewDiaSemana(6, "Sábado")
		};
		foreach (var d in dias) DiasSemana.Add(d);

		// Horas 8 a 19
		for (int h = 8; h <= 19; h++) Horas.Add(h);

		// Medicos ejemplo (no asociados aun)
		_medicosHard = new List<MedicoSimpleViewModel> {
			new MedicoSimpleViewModel("m1","Ana Perez (Gastro)"),
			new MedicoSimpleViewModel("m2","Luis Gomez (Gastro)"),
			new MedicoSimpleViewModel("m3","Marta Lopez (Psico)"),
			new MedicoSimpleViewModel("m4","Carla Ruiz (Derma)")
		};

		// initial empty medicos list
		MedicosEspecialistas.Clear();

		// initial disponibilidades empty
		Disponibilidades.Clear();
	}

	private List<MedicoSimpleViewModel> _medicosHard = [];

	// Cargar médicos por especialidad (demo)
	private void LoadMedicosPorEspecialidad(string? especialidadUId) {
		MedicosEspecialistas.Clear();
		if (string.IsNullOrEmpty(especialidadUId)) return;

		// En producción: Medico2025.SelectWhereEspecialidadId(especialidadUId)
		// Demo mapping por UId
		IEnumerable<MedicoSimpleViewModel> found = Enumerable.Empty<MedicoSimpleViewModel>();
		if (especialidadUId == "esp-gastro")
			found = _medicosHard.Where(m => m.Displayear.Contains("Gastro"));
		else if (especialidadUId == "esp-psico")
			found = _medicosHard.Where(m => m.Displayear.Contains("Psico"));
		else if (especialidadUId == "esp-derma")
			found = _medicosHard.Where(m => m.Displayear.Contains("Derma"));

		foreach (var m in found) MedicosEspecialistas.Add(m);

		// seleccionar primer medico si existe
		if (MedicosEspecialistas.Any()) {
			SelectedMedicoId = MedicosEspecialistas.First().Id;
			Console.WriteLine($"[UI] Primer médico seleccionado automáticamente: {SelectedMedicoId}");
		}
	}

	// Refresh disponibilidades cada vez que cambia un filtro
	private void RefreshDisponibilidades() {
		Console.WriteLine("[UI] RefreshDisponibilidades llamado");
		Disponibilidades.Clear();

		// Si no hay especialidad no hay disponibilidad
		if (string.IsNullOrEmpty(SelectedEspecialidadUId)) {
			Console.WriteLine("[UI] No hay especialidad seleccionada -> no disponibilidades");
			return;
		}

		// Generar mock: para la especialidad elegida generamos slots próximos 5 días según filtros
		var hoy = DateTime.Today;
		var duracionMinutos = 40; // demo, ideal obtener de especialidad real
		for (int diaOffset = 0; diaOffset < 7; diaOffset++) {
			var fecha = hoy.AddDays(diaOffset);
			var dayOfWeek = (int)fecha.DayOfWeek;

			// si filtro dia habilitado y no coincide, saltar
			if (FiltroDiaEnabled && SelectedDiaValue.HasValue && SelectedDiaValue.Value != dayOfWeek) continue;

			// para cada médico disponible (si hay) generar slots
			var medicos = MedicosEspecialistas.Any() ? MedicosEspecialistas : _medicosHard.Where(m => m.Displayear.Contains(SelectedEspecialidadUId?.Split('-').Last() ?? ""));
			foreach (var med in medicos) {
				// horas candidates 9..16 step duracion
				for (int hora = 9; hora <= 16; hora += Math.Max(1, duracionMinutos / 60)) {
					// si filtro hora activado y no coincide, saltar
					if (FiltroHoraEnabled && SelectedHora.HasValue && SelectedHora.Value != hora) continue;

					var horaStr = TimeSpan.FromHours(hora).ToString(@"hh\:mm");
					Disponibilidades.Add(new DisponibilidadEspecialidadModelView(fecha, horaStr, med.Displayear));
				}
			}
		}

		Console.WriteLine($"[UI] Disponibilidades actualizadas: {Disponibilidades.Count}");
	}

	// Botones
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Close();
	}

	private void ButtonReservar(object sender, RoutedEventArgs e) {
		// Para demo: tomar primer item seleccionado y mostrar info
		var seleccionado = txtDisponibilidades.SelectedItem as DisponibilidadEspecialidadModelView ?? Disponibilidades.FirstOrDefault();
		if (seleccionado is null) {
			MessageBox.Show("No hay una disponibilidad seleccionada.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		// Aquí se llamaría a TurnoService/TurnoFactory para crear/guardar el turno utilizando los objetos de dominio.
		Console.WriteLine($"[UI] Reservando turno: {seleccionado.Fecha:d} {seleccionado.Hora} - {seleccionado.Medico}");
		MessageBox.Show($"Reservando turno: {seleccionado.Fecha:d} {seleccionado.Hora} - {seleccionado.Medico}", "Reservar", MessageBoxButton.OK, MessageBoxImage.Information);

		// Cerrar ventana como comportamiento por defecto demo
		this.Close();
	}
}