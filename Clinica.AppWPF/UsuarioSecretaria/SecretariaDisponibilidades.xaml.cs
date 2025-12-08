using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.AppWPF.UsuarioSecretaria.Comodidades;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;



public static class Comodidades {

	public record DisponibilidadEspecialidadModelView(string Fecha, string Hora, string Medico, DiaSemana2025 DiaSemana);
	public record EspecialidadViewModel(EspecialidadCodigo Codigo, string Displayear);
	public record MedicoSimpleViewModel(MedicoId Id, EspecialidadCodigo EspecialidadCodigo, string Displayear);
	//public record ModelViewDiaSemana(int Value, string NombreDia);


	public static async Task<MedicoDto> RespectivoMedico(this MedicoId id) {
        MedicoDto? instance = await App.Repositorio.SelectMedicoWhereId(id);
		if (instance is not null) return instance;
		string error = $"No existe el médico con ID {id.Valor}";
		MessageBox.Show(error);
		throw new InvalidOperationException(error);
	}
	public static async Task<PacienteApiDto> RespectivoPaciente(this PacienteId id) {
        PacienteApiDto? instance = await App.Repositorio.SelectPacienteWhereId(id);
		if (instance is not null) return instance;
		string error = $"No existe el médico con ID {id.Valor}";
		MessageBox.Show(error);
		throw new InvalidOperationException(error);
	}

	public static MedicoSimpleViewModel ToSimpleViewModel(this MedicoDto dto) {
		return new MedicoSimpleViewModel(
			Id: dto.Id,
			EspecialidadCodigo: dto.EspecialidadCodigo,
			Displayear: $"{dto.Nombre} {dto.Apellido}"
		);
	}

	public static EspecialidadViewModel ToSimpleViewModel(this Especialidad2025 instance) {
		return new EspecialidadViewModel(
			Codigo: instance.Codigo,
			Displayear: $"{instance.Titulo} --- (Duración consulta: {instance.DuracionConsultaMinutos})"
		);
	}

	async public static Task<DisponibilidadEspecialidadModelView> ToSimpleViewModel(this Disponibilidad2025 domainValue) {
		MedicoDto medico = await domainValue.MedicoId.RespectivoMedico();
		return new DisponibilidadEspecialidadModelView(
			Fecha: domainValue.FechaHoraDesde.AFechaArgentina(),
			Hora: domainValue.FechaHoraDesde.AHorasArgentina(),
			Medico: $"{medico.Nombre}{medico.Apellido}",
			DiaSemana: domainValue.DiaSemana
		);
	}


}


//DiaSemana2025
public partial class SecretariaDisponibilidades : Window, INotifyPropertyChanged {
	//public SecretariaDisponibilidades() {
	//	InitializeComponent();
	//	DataContext = this;
	//	CargarHoras();
	//	MedicosEspecialistasItemsSource.Clear();
	//	DisponibilidadesItemsSource.Clear();
	//	Loaded += WindowGestionTurno_Loaded;
	//}
	public SecretariaDisponibilidades(PacienteId pacienteId) {
		InitializeComponent();
		DataContext = this;
		//SelectedPaciente = paciente;

        //_ = LoadPacienteAsync(pacienteId);
		UpdatePacienteUI(pacienteId);
		Loaded += WindowGestionTurno_Loaded;
		CargarHoras();
	}

	private async void WindowGestionTurno_Loaded(object sender, RoutedEventArgs e) {
        List<MedicoDto> medicos = await App.Repositorio.SelectMedicos();
		MedicosTodos = [.. medicos.Select(x => x.ToSimpleViewModel())];
		OnPropertyChanged(nameof(MedicosTodos));
	}




	//------------------------------//
	private void UpdatePacienteUI(PacienteId paciente) {

		//Task<PacienteApiDto>? paciente = await pacienteId.RespectivoPaciente();

		//txtPacienteDni.Text = paciente.Dni;
		//txtPacienteNombre.Text = paciente.Nombre;
		//txtPacienteApellido.Text = paciente.Apellido;
		//txtPacienteEmail.Text = paciente.Email;
		//txtPacienteTelefono.Text = paciente.Telefono;
		//buttonModificarPaciente.IsEnabled = paciente != null;
	}

	public required PacienteApiDto SelectedPaciente { get; set; }



	//------------------------------//














	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));










	//-------------------------------------------------------------------------------//
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






	//-------------------------------------------------------------------------------//
	//-------------- Elegir dia minimo que prefiere la cita:

	private DateTime _preferedFechaValue = DateTime.Today;
	public DateTime PreferedFechaValue {
		get => _preferedFechaValue;
		set {
			if (_preferedFechaValue == value) return;
			_preferedFechaValue = value;
			//MessageBox.Show(value.ToString());
			OnPropertyChanged(nameof(PreferedFechaValue));
		}
	}
	//-------------- Elegir hora minima que prefiere la cita:
	private void CargarHoras() {
		HorasItemsSource.Clear();

		TimeOnly inicio = ClinicaNegocio.Atencion.DesdeHs;
		TimeOnly fin = ClinicaNegocio.Atencion.HastaHs;

		for (TimeOnly t = inicio; t <= fin; t = t.AddMinutes(30))
			HorasItemsSource.Add(t);

		// Seleccionar por defecto la primera opción
		if (HorasItemsSource.Count > 0)
			SelectedHoraValue = HorasItemsSource[0];
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

	private bool _filtroHoraEnabled = false;
	public bool FiltroHoraEnabled {
		get => _filtroHoraEnabled;
		set {
			if (_filtroHoraEnabled == value)
				return; _filtroHoraEnabled = value;
			if (value == false)
				SelectedHoraValue = ClinicaNegocio.Atencion.DesdeHs;
			OnPropertyChanged(nameof(FiltroHoraEnabled));
		}
	}


	//-------------- Elegir dia semana que prefiere la cita:
	public ObservableCollection<DiaSemana2025> DiasSemanaItemsSource { get; } = [.. DiaSemana2025.Todos];

	private DiaSemana2025? _selectedDiaValue;
	public DiaSemana2025? SelectedDiaValue {
		get => _selectedDiaValue;
		set {
			if (_selectedDiaValue == value) return;
			_selectedDiaValue = value;
			//MessageBox.Show(value.ToString());
			OnPropertyChanged(nameof(SelectedDiaValue));
		}
	}

	private bool _filtroDiaEnabled = false;
	public bool FiltroDiaEnabled {
		get => _filtroDiaEnabled;
		set {
			if (_filtroDiaEnabled == value)
				return;
			if (value == false)
				SelectedDiaValue = null;
			_filtroDiaEnabled = value;
			OnPropertyChanged(nameof(FiltroDiaEnabled));
		}
	}

	//-------------------------------------------------------------------------------//



	//-------------- Items yieldados y el seleccionado:
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









	//-------------------------------------------------------------------------------//
	public ObservableCollection<EspecialidadViewModel> EspecialidadesDisponiblesItemsSource { get; } = [.. Especialidad2025.Todas.Select(x => x.ToSimpleViewModel())];
	private EspecialidadCodigo? _selectedEspecialidadCodigo;
	public EspecialidadCodigo? SelectedEspecialidadCodigo {
		get => _selectedEspecialidadCodigo;
		set {
			if (_selectedEspecialidadCodigo == value) return;
			_selectedEspecialidadCodigo = value;
			buttonConsultar.IsEnabled = value != null;
			OnPropertyChanged(nameof(SelectedEspecialidadCodigo));
			LoadMedicosPorEspecialidad(value);
		}
	}
	//-------------------------------------------------------------------------------//









	//----------------------------------------------PRIVATE PROCEDURES----------------------------------------------//

	private void ButtonConsultar(object sender, RoutedEventArgs e) {
		SelectedDisponibilidad = null;
		if (SelectedEspecialidadCodigo is EspecialidadCodigo gud) {
			RefreshDisponibilidades(gud, PreferedFechaValue + SelectedHoraValue.ToTimeSpan());
		}
	}

	private async void LoadMedicosPorEspecialidad(EspecialidadCodigo? especialidadCodigo) {
		MedicosEspecialistasItemsSource.Clear();
		if (especialidadCodigo is not EspecialidadCodigo especialidadGud) return;

		//MessageBox.Show("Cargando medicos especialistas...", "Cargando", MessageBoxButton.OK, MessageBoxImage.Information);
		IEnumerable<MedicoSimpleViewModel> found = [];
		if (MedicosTodos is null) {
            List<MedicoDto> medicos = await App.Repositorio.SelectMedicos();
			MedicosTodos = [.. medicos.Select(x => x.ToSimpleViewModel())];
		}
		foreach (MedicoSimpleViewModel? item in MedicosTodos.Where(m => m.EspecialidadCodigo == especialidadCodigo)) {
			MedicosEspecialistasItemsSource.Add(item);
			//MessageBox.Show("{item}", "Medico encontrado", MessageBoxButton.OK, MessageBoxImage.Information);
		}
		if (MedicosEspecialistasItemsSource.Any()) {
			SelectedMedicoId = MedicosEspecialistasItemsSource.First().Id;
		}
	}

	async private void RefreshDisponibilidades(EspecialidadCodigo especialidad, DateTime turnosAPartirDeFechaYHora) {
		DisponibilidadesItemsSource.Clear();
		List<Disponibilidad2025> disponibilidades = await App.Repositorio.SelectDisponibilidades(especialidad, 15, turnosAPartirDeFechaYHora);

		foreach (Disponibilidad2025 dispo in disponibilidades) {
			DisponibilidadesItemsSource.Add(await dispo.ToSimpleViewModel());
		}
		buttonReservar.IsEnabled = SelectedDisponibilidad != null;
	}



	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (SelectedPaciente != null) {
			this.AbrirComoDialogo<SecretariaPacienteFormulario>(SelectedPaciente.Id);
		}
	}
	// Botones
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		this.Close();
	}

	private void ButtonReservar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (SelectedDisponibilidad is not DisponibilidadEspecialidadModelView seleccionada) {
			MessageBox.Show("No hay una disponibilidad seleccionada.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
		} else {
			MessageBox.Show($"Reservando turno: {seleccionada.Fecha:d} {seleccionada.Hora} - {seleccionada.Medico}", "Reservar", MessageBoxButton.OK, MessageBoxImage.Information);
			this.Close();
		}
	}

	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
}