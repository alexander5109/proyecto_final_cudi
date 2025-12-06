using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;

//public record DisponibilidadDto(DateTime Fecha, string Hora, string Medico);

public partial class SecretariaGestorTurnos : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string name)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	public ObservableCollection<Especialidad2025> EspecialidadesDisponibles { get; } = [];
	public ObservableCollection<MedicoDto> MedicosEspecialistas { get; } = [];
	public ObservableCollection<DiaSemana2025> DiasSemana { get; } = [];
	public ObservableCollection<int> Horas { get; } = [];
	public ObservableCollection<Disponibilidad2025> Disponibilidades { get; } = [];

	// ---------------------------
	// Selecteds / filtros
	// ---------------------------

	private EspecialidadCodigo? _selectedEspecialidadUId;
	public EspecialidadCodigo? SelectedEspecialidadUId {
		get => _selectedEspecialidadUId;
		set {
			if (_selectedEspecialidadUId == value) return;
			_selectedEspecialidadUId = value;
			OnPropertyChanged(nameof(SelectedEspecialidadUId));
			OnEspecialidadChanged();
		}
	}

	private MedicoId? _selectedMedicoId;
	public MedicoId? SelectedMedicoId {
		get => _selectedMedicoId;
		set {
			if (_selectedMedicoId == value) return;
			_selectedMedicoId = value;
			OnPropertyChanged(nameof(SelectedMedicoId));
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
			RefreshDisponibilidades();
		}
	}

	public bool FiltroDiaEnabled { get; set; }
	public bool FiltroHoraEnabled { get; set; }

	// ---------------------------
	// Constructor
	// ---------------------------

	public SecretariaGestorTurnos() {
		InitializeComponent();
		DataContext = this;
		LoadInitialData();
	}

	// ---------------------------
	// Carga inicial
	// ---------------------------

	private async void LoadInitialData() {
		// Especialidades
		EspecialidadesDisponibles.Clear();
		foreach (var e in Especialidad2025.Todas)
			EspecialidadesDisponibles.Add(e);

		// Médicos por especialidad inicial
		MedicosEspecialistas.Clear();
		var codigo = SelectedEspecialidadUId ?? EspecialidadesDisponibles.First().Codigo;

		foreach (var m in await App.BaseDeDatos.SelectMedicosWhereEspecialidadCodigo(codigo))
			MedicosEspecialistas.Add(m);

		// Días
		DiasSemana.Clear();
		foreach (var d in DiaSemana2025.Todos)
			DiasSemana.Add(d);

		// Horas (si tenés un método real para obtenerlas, lo pongo acá)
		// por ahora lo dejo vacío hasta que definamos de dónde vienen:
		// HoursAdd(8); HoursAdd(9); etc.

		RefreshDisponibilidades();
	}

	private void HoursAdd(int h) {
		if (!Horas.Contains(h)) Horas.Add(h);
	}

	// ---------------------------
	// Cambio de especialidad
	// ---------------------------

	private async void OnEspecialidadChanged() {
		if (SelectedEspecialidadUId is null)
			return;

		MedicosEspecialistas.Clear();

		var items =
			await App.BaseDeDatos.SelectMedicosWhereEspecialidadCodigo(SelectedEspecialidadUId.Value);

		foreach (var m in items)
			MedicosEspecialistas.Add(m);

		SelectedMedicoId = MedicosEspecialistas.FirstOrDefault()?.Id;

		RefreshDisponibilidades();
	}

	// ---------------------------
	// Carga de disponibilidades desde API
	// ---------------------------

	private async void RefreshDisponibilidades() {
		Disponibilidades.Clear();

		if (SelectedEspecialidadUId is null)
			return;

		// ¿Cuántas disponibilidades pedir? Suponemos 20 para ejemplo
		int cuantos = 20;

		var items = await App.BaseDeDatos.SelectDisponibilidades(
			SelectedEspecialidadUId.Value,
			cuantos,
			DateTime.Now
		);

		foreach (var d in items)
			Disponibilidades.Add(d);
	}

	// ---------------------------
	// Botones
	// ---------------------------

	private void ButtonCancelar(object sender, RoutedEventArgs e) => this.VolverAHome();

	private void ButtonReservar(object sender, RoutedEventArgs e) {
		Disponibilidad2025? seleccionado =
			(Disponibilidad2025?)txtDisponibilidades.SelectedItem ??
			Disponibilidades.FirstOrDefault();

		if (seleccionado is null) {
			MessageBox.Show("No hay una disponibilidad seleccionada.");
			return;
		}

		MessageBox.Show($"Reservando: {seleccionado?.FechaHoraDesde:d} {seleccionado?.FechaHoraHasta} - {seleccionado?.MedicoId}");
		Close();
	}
}
