using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.ViewModels;
using Clinica.DataPersistencia.Repositorios;
using Clinica.Dominio.Repositorios;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.AppWPF;

public partial class WindowGestionTurno : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));



	// Bindables
	public ObservableCollection<ViewModelEspecialidadMedica> EspecialidadesDisponibles { get; } = [];
	public ObservableCollection<ViewModelMedico> MedicosEspecialistas { get; } = [];


	public ObservableCollection<ViewModelDiaSemana> DiasSemana { get; } = [];


	public ObservableCollection<int> Horas { get; } = [];
	public ObservableCollection<ViewModelDisponibilidadEspecialidad> Disponibilidades { get; } = [];

	// Selecteds / filtros
	private int? _selectedEspecialidadCodigoInterno;
	public int? SelectedEspecialidadCodigoInterno {
		get => _selectedEspecialidadCodigoInterno;
		set {
			if (_selectedEspecialidadCodigoInterno == value) return;
			_selectedEspecialidadCodigoInterno = value;
			OnPropertyChanged(nameof(SelectedEspecialidadCodigoInterno));
			//MessageBox.Show($"[UI] Especialidad seleccionada: {_selectedEspecialidadCodigoInterno}");
			LoadMedicosPorEspecialidad(value);
			RefreshDisponibilidades();
		}
	}

	private int? _selectedMedicoId;
	public int? SelectedMedicoId {
		get => _selectedMedicoId;
		set {
			if (_selectedMedicoId == value) return;
			_selectedMedicoId = value;
			OnPropertyChanged(nameof(SelectedMedicoId));
			//MessageBox.Show($"[UI] Médico seleccionado: {_selectedMedicoId}");
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
			//MessageBox.Show($"[UI] Día seleccionado: {_selectedDiaValue}");
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
			//MessageBox.Show($"[UI] Hora seleccionada: {_selectedHora}");
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
        //var espList = new[] {
        //	new ViewModelEspecialidadMedica("esp-gastro", "Gastroenterólogo"),
        //	new ViewModelEspecialidadMedica("esp-psico", "Psicólogo"),
        //	new ViewModelEspecialidadMedica("esp-derma", "Dermatólogo")
        //};
		foreach (EspecialidadMedica2025 especialidad in EspecialidadMedica2025.TodasLasSoportadas) {
			//MessageBox.Show($"Especialidad cargada: {e.CodigoInterno} - {e.Titulo}", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
			EspecialidadesDisponibles.Add(new ViewModelEspecialidadMedica(especialidad.CodigoInterno.Valor, especialidad.Titulo) ); 
		}

		//// Dias de la semana (ViewModelDiaSemana)
		//var dias = new[] {
		//	new ViewModelDiaSemana(0, "Domingo"),
		//	new ViewModelDiaSemana(1, "Lunes"),
		//	new ViewModelDiaSemana(2, "Martes"),
		//	new ViewModelDiaSemana(3, "Miércoles"),
		//	new ViewModelDiaSemana(4, "Jueves"),
		//	new ViewModelDiaSemana(5, "Viernes"),
		//	new ViewModelDiaSemana(6, "Sábado")
		//};
		foreach (DiaSemana2025 dia in DiaSemana2025.Los7DiaSemana2025) { 
			DiasSemana.Add(new ViewModelDiaSemana((int)dia.Valor, dia.Valor.AEspañol()));
		}

		// Horas 8 a 19
		for (int h = 8; h <= 19; h++) Horas.Add(h);

		// Medicos ejemplo (no asociados aun)
		//List<ViewModelMedico> _medicosHard = 

		//new() {
		//	new ViewModelMedico("m1","Ana Perez (Gastro)"),
		//	new ViewModelMedico("m2","Luis Gomez (Gastro)"),
		//	new ViewModelMedico("m3","Marta Lopez (Psico)"),
		//	new ViewModelMedico("m4","Carla Ruiz (Derma)")
		//};

		// initial empty medicos list
		MedicosEspecialistas.Clear();

		// initial disponibilidades empty
		Disponibilidades.Clear();
	}

	//private List<ViewModelMedico> _medicosHard = App.BaseDeDatos.DictMedicos.Values.ToList();

	// Cargar médicos por especialidad (demo)
	private void LoadMedicosPorEspecialidad(int? especialidadCodigoInterno) {

		//MessageBox.Show($"[UI] LoadMedicosPorEspecialidad llamado {especialidadCodigoInterno}");
		MedicosEspecialistas.Clear();


		if (especialidadCodigoInterno is null) return;

		// En producción: Medico2025.SelectWhereEspecialidadCodigoInterno(especialidadCodigoInterno)
		// Demo mapping por CodigoInterno
		//IEnumerable<ViewModelMedico> found = Enumerable.Empty<ViewModelMedico>();




        //if (especialidadCodigoInterno == "esp-gastro")
        //	found = _medicosHard.Where(m => m.Displayear.Contains("Gastro"));
        //else if (especialidadCodigoInterno == "esp-psico")
        //	found = _medicosHard.Where(m => m.Displayear.Contains("Psico"));
        //else if (especialidadCodigoInterno == "esp-derma")
        //	found = _medicosHard.Where(m => m.Displayear.Contains("Derma"));

		foreach (ViewModelMedico medico in App.BaseDeDatos.ReadMedicosWhereEspecialidad((int)especialidadCodigoInterno)){
			//MessageBox.Show(medico.Displayear);
			MedicosEspecialistas.Add(medico);
		}
		// seleccionar primer medico si existe
		if (MedicosEspecialistas.Any()) {
			SelectedMedicoId = MedicosEspecialistas.First().Id;
			//MessageBox.Show($"[UI] Primer médico seleccionado automáticamente: {SelectedMedicoId}");
		}
	}

	// Refresh disponibilidades cada vez que cambia un filtro
	private void RefreshDisponibilidades() {
		//MessageBox.Show("[UI] RefreshDisponibilidades llamado");
		Disponibilidades.Clear();

		// Si no hay especialidad no hay disponibilidad
		if (SelectedEspecialidadCodigoInterno  == null) return;

		// Generar mock: para la especialidad elegida generamos slots próximos 5 días según filtros
		var hoy = DateTime.Today;
		var duracionMinutos = 40; // demo, ideal obtener de especialidad real
		for (int diaOffset = 0; diaOffset < 7; diaOffset++) {
			var fecha = hoy.AddDays(diaOffset);
			var dayOfWeek = (int)fecha.DayOfWeek;

			// si filtro dia habilitado y no coincide, saltar
			if (FiltroDiaEnabled && SelectedDiaValue.HasValue && SelectedDiaValue.Value != dayOfWeek) continue;

			// para cada médico disponible (si hay) generar slots
			var medicos = MedicosEspecialistas.Any() ? MedicosEspecialistas : App.BaseDeDatos.DictMedicos.Values.Where(m => m.EspecialidadCodigoInterno == SelectedEspecialidadCodigoInterno);

			foreach (var med in medicos) {
				// horas candidates 9..16 step duracion
				for (int hora = 9; hora <= 16; hora += Math.Max(1, duracionMinutos / 60)) {
					// si filtro hora activado y no coincide, saltar
					if (FiltroHoraEnabled && SelectedHora.HasValue && SelectedHora.Value != hora) continue;

					var horaStr = TimeSpan.FromHours(hora).ToString(@"hh\:mm");
					Disponibilidades.Add(new ViewModelDisponibilidadEspecialidad(fecha, horaStr, med.Displayear));
				}
			}
		}

		//MessageBox.Show($"[UI] Disponibilidades actualizadas: {Disponibilidades.Count}");
	}

	// Botones
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.VolverAHome();
	}

	private void ButtonReservar(object sender, RoutedEventArgs e) {
		// Para demo: tomar primer item seleccionado y mostrar info
		var seleccionado = txtDisponibilidades.SelectedItem as ViewModelDisponibilidadEspecialidad ?? Disponibilidades.FirstOrDefault();
		if (seleccionado is null) {
			//MessageBox.Show("No hay una disponibilidad seleccionada.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		// Aquí se llamaría a TurnoService/TurnoFactory para crear/guardar el turno utilizando los objetos de dominio.
		//MessageBox.Show($"[UI] Reservando turno: {seleccionado.Fecha:d} {seleccionado.Hora} - {seleccionado.Medico}");
		//MessageBox.Show($"Reservando turno: {seleccionado.Fecha:d} {seleccionado.Hora} - {seleccionado.Medico}", "Reservar", MessageBoxButton.OK, MessageBoxImage.Information);

		// Cerrar ventana como comportamiento por defecto demo
		this.Close();
	}
}