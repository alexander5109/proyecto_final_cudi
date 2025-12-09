using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.ViewModels;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.AppWPF.Infrastructure.Comodidades;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.AppWPF.UsuarioSecretaria;

public class SecretariaDisponibilidadesViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string name)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	public SecretariaDisponibilidadesViewModel(PacienteId pacienteId) {
		_ = LoadPaciente(pacienteId);
		_ = LoadMedicosTodosAsync();
		LoadHoras();
	}

	// -----------------------------------------------------------------------
	// PACIENTE
	// -----------------------------------------------------------------------
	public PacienteDbModel? SelectedPaciente { get; private set; }

	private async Task LoadPaciente(PacienteId id) {
		SelectedPaciente = RepoCache.DictPacientes.GetValueOrDefault(id);
		OnPropertyChanged(nameof(SelectedPaciente));
	}

	// -----------------------------------------------------------------------
	// MEDICOS
	// -----------------------------------------------------------------------
	public IReadOnlyList<MedicoSimpleViewModel>? MedicosTodos { get; private set; }
	public ObservableCollection<MedicoSimpleViewModel> MedicosEspecialistasItemsSource { get; } = [];

	private MedicoId? _selectedMedicoId;
	public MedicoId? SelectedMedicoId {
		get => _selectedMedicoId;
		set {
			if (_selectedMedicoId == value) return;
			_selectedMedicoId = value;
			OnPropertyChanged(nameof(SelectedMedicoId));
		}
	}

	public async Task LoadMedicosTodosAsync() {
		var medicos = await App.Repositorio.SelectMedicos();
		MedicosTodos = medicos.Select(m => m.ToSimpleViewModel()).ToList();
		OnPropertyChanged(nameof(MedicosTodos));
	}

	public async Task LoadMedicosPorEspecialidadAsync(EspecialidadCodigo? esp) {
		MedicosEspecialistasItemsSource.Clear();
		if (esp is not EspecialidadCodigo codigo) return;

		if (MedicosTodos == null)
			await LoadMedicosTodosAsync();

		foreach (var m in MedicosTodos!.Where(x => x.EspecialidadCodigo == codigo))
			MedicosEspecialistasItemsSource.Add(m);

		if (MedicosEspecialistasItemsSource.Any())
			SelectedMedicoId = MedicosEspecialistasItemsSource.First().Id;
	}

	// -----------------------------------------------------------------------
	// FECHAS / HORAS
	// -----------------------------------------------------------------------
	private DateTime _preferedFechaValue = DateTime.Today;
	public DateTime PreferedFechaValue {
		get => _preferedFechaValue;
		set {
			if (_preferedFechaValue == value) return;
			_preferedFechaValue = value;
			OnPropertyChanged(nameof(PreferedFechaValue));
		}
	}

	public ObservableCollection<TimeOnly> HorasItemsSource { get; } = [];

	private TimeOnly _selectedHoraValue = ClinicaNegocio.Atencion.DesdeHs;
	public TimeOnly SelectedHoraValue {
		get => _selectedHoraValue;
		set {
			if (_selectedHoraValue == value) return;
			_selectedHoraValue = value;
			OnPropertyChanged(nameof(SelectedHoraValue));
		}
	}

	private bool _filtroHoraEnabled;
	public bool FiltroHoraEnabled {
		get => _filtroHoraEnabled;
		set {
			if (_filtroHoraEnabled == value) return;
			_filtroHoraEnabled = value;
			if (!value)
				SelectedHoraValue = ClinicaNegocio.Atencion.DesdeHs;
			OnPropertyChanged(nameof(FiltroHoraEnabled));
		}
	}

	private void LoadHoras() {
		HorasItemsSource.Clear();

		var inicio = ClinicaNegocio.Atencion.DesdeHs;
		var fin = ClinicaNegocio.Atencion.HastaHs;

		for (var t = inicio; t <= fin; t = t.AddMinutes(30))
			HorasItemsSource.Add(t);

		if (HorasItemsSource.Any())
			SelectedHoraValue = HorasItemsSource[0];
	}




	// -----------------------------------------------------------------------
	// DÍA DE SEMANA
	// -----------------------------------------------------------------------
	public ObservableCollection<DiaDeSemanaViewModel> DiasSemanaItemsSource { get; } =
		[.. DiaDeSemanaViewModel.Todos];

	private DiaDeSemanaViewModel? _selectedDiaValue;
	public DiaDeSemanaViewModel? SelectedDiaValue {
		get => _selectedDiaValue;
		set {
			if (_selectedDiaValue == value) return;
			_selectedDiaValue = value;
			OnPropertyChanged(nameof(SelectedDiaValue));
		}
	}

	private bool _filtroDiaEnabled;
	public bool FiltroDiaEnabled {
		get => _filtroDiaEnabled;
		set {
			if (_filtroDiaEnabled == value) return;
			if (!value)
				SelectedDiaValue = null;
			_filtroDiaEnabled = value;
			OnPropertyChanged(nameof(FiltroDiaEnabled));
		}
	}

	// -----------------------------------------------------------------------
	// ESPECIALIDAD
	// -----------------------------------------------------------------------
	public ObservableCollection<EspecialidadViewModel> EspecialidadesDisponiblesItemsSource { get; } =
		[.. Especialidad2025.Todas.Select(x => x.ToSimpleViewModel())];

	private EspecialidadCodigo? _selectedEspecialidadCodigo;
	public EspecialidadCodigo? SelectedEspecialidadCodigo {
		get => _selectedEspecialidadCodigo;
		set {
			if (_selectedEspecialidadCodigo == value) return;
			_selectedEspecialidadCodigo = value;

			OnPropertyChanged(nameof(SelectedEspecialidadCodigo));

			_ = LoadMedicosPorEspecialidadAsync(value);
		}
	}

	// -----------------------------------------------------------------------
	// DISPONIBILIDADES
	// -----------------------------------------------------------------------
	public ObservableCollection<DisponibilidadEspecialidadModelView> DisponibilidadesItemsSource { get; } = [];

	private DisponibilidadEspecialidadModelView? _selectedDisponibilidad;
	public DisponibilidadEspecialidadModelView? SelectedDisponibilidad {
		get => _selectedDisponibilidad;
		set {
			if (_selectedDisponibilidad == value) return;
			_selectedDisponibilidad = value;
			OnPropertyChanged(nameof(SelectedDisponibilidad));
		}
	}

	public async Task RefreshDisponibilidadesAsync() {
		if (SelectedEspecialidadCodigo is not EspecialidadCodigo esp) return;

		DisponibilidadesItemsSource.Clear();

		DateTime desde = PreferedFechaValue + SelectedHoraValue.ToTimeSpan();

		var lista = await App.Repositorio.SelectDisponibilidades(
			especialidad: esp,
			cuantos: 15,
			apartirDeCuando: desde
		);

		foreach (var d in lista)
			DisponibilidadesItemsSource.Add(await d.ToSimpleViewModel());
	}
}
