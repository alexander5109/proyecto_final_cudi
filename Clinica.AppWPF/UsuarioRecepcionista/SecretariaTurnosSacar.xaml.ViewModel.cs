using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.AppWPF.CommonViewModels.CommonEnumsToViewModel;
using static Clinica.Shared.DbModels.DbModels;
namespace Clinica.AppWPF.UsuarioRecepcionista;

public class SecretariaTurnosSacarViewModel : INotifyPropertyChanged {

	// ================================================================
	// CONSTRUCTORES Y CONTEXTO
	// ================================================================
	public SecretariaTurnosSacarViewModel(PacienteDbModel paciente) {
		SelectedPaciente = paciente;

		EspecialidadesDisponiblesItemsSource.Clear();
		foreach (EspecialidadViewModel? esp in Especialidad2025.Todas.Select(x => new EspecialidadViewModel(x)))
			EspecialidadesDisponiblesItemsSource.Add(esp);

		_ = LoadMedicosTodosAsync();

		DisponibilidadesView = CollectionViewSource.GetDefaultView(DisponibilidadesItemsSource);
		DisponibilidadesView?.SortDescriptions.Clear();
		// opcionalmente, podés definir un sort inicial
		// DisponibilidadesView.SortDescriptions.Add(new SortDescription("Fecha", ListSortDirection.Ascending));

	}

	public SecretariaTurnosSacarViewModel(PacienteDbModel paciente, EspecialidadCodigo especialidad) {
		SelectedPaciente = paciente;

		EspecialidadesDisponiblesItemsSource.Clear(); // <<---- importantísimo

		Result<Especialidad2025> espResult = Especialidad2025.CrearResult(especialidad);

		espResult.MatchAndDo(
			ok => {
				EspecialidadViewModel vm = new(ok);
				SelectedEspecialidad = vm;
				EspecialidadesDisponiblesItemsSource.Add(vm);
			},
			err => {
				MessageBox.Show($"El código de especialidad no existe <{especialidad}>");

				foreach (EspecialidadViewModel? esp in Especialidad2025.Todas.Select(x => new EspecialidadViewModel(x)))
					EspecialidadesDisponiblesItemsSource.Add(esp);
			});

		_ = LoadMedicosTodosAsync();

		DisponibilidadesView = CollectionViewSource.GetDefaultView(DisponibilidadesItemsSource);
		DisponibilidadesView?.SortDescriptions.Clear();
		// opcionalmente, podés definir un sort inicial
		// DisponibilidadesView.SortDescriptions.Add(new SortDescription("Fecha", ListSortDirection.Ascending));

	}






	// ================================================================
	// CONTEXTO DE TURNO
	// ================================================================


	public PacienteDbModel SelectedPaciente { get; private set; }
	public string? SelectedPacienteDomicilioCompleto => $"{SelectedPaciente?.Localidad}, {SelectedPaciente?.Domicilio}";
	public string? SelectedPacienteNombreCompleto => $"{SelectedPaciente?.Nombre} {SelectedPaciente?.Apellido}";




	// ================================================================
	// FUENTES DE DATOS (ItemsSource)
	// ================================================================

	public IReadOnlyList<MedicoSimpleViewModel>? MedicosTodos { get; private set; }
	public ObservableCollection<MedicoSimpleViewModel> MedicosEspecialistasItemsSource { get; } = [];

	private ICollectionView? _disponibilidadesView;
	public ICollectionView? DisponibilidadesView {
		get => _disponibilidadesView;
		private set {
			if (_disponibilidadesView == value) return;
			_disponibilidadesView = value;
			OnPropertyChanged(nameof(DisponibilidadesView));
		}
	}

	public ObservableCollection<EspecialidadViewModel> EspecialidadesDisponiblesItemsSource { get; } = [];

	public ObservableCollection<DiaDeSemanaViewModel> DiasSemanaItemsSource { get; } =
		[.. DiaDeSemanaViewModel.Todos];
	public ObservableCollection<DisponibilidadEspecialidadModelView> DisponibilidadesItemsSource { get; } = [];



	// ================================================================
	// REGLAS
	// ================================================================
	public bool ComboBoxMedicos_Enabled => MedicosEspecialistasItemsSource.Any();
	public bool ComboBoxDiasSemana_Enabled => SelectedMedico != null && SelectedMedico.DiasAtencion.Any();
	public bool BotonBuscar_Enabled => SelectedMedico != null && SelectedMedico.DiasAtencion.Any();
	public bool BotonAgendar_Enabled => DisponibilidadesItemsSource.Any() && SelectedDisponibilidad != null;

	public string InformeReservaDeTurno {
		get {
			if (SelectedDisponibilidad is null || SelectedEspecialidad is null) return "";
			return $"Se va a reservar un turno para {SelectedPacienteNombreCompleto} " +
				   $"para consulta de {SelectedEspecialidad.NombreEspecialidad} " +
				   $"con el profesional {SelectedDisponibilidad.MedicoDisplayear} " +
				   $"el día {SelectedDisponibilidad.Fecha} a las {SelectedDisponibilidad.Hora}. Confirmar?";
		}
	}

	// -----------------------------
	// METHODS
	// -----------------------------


	public async Task RefreshDisponibilidadesAsync() {
		if (SelectedEspecialidad is not EspecialidadViewModel esp) return;

		DisponibilidadesItemsSource.Clear();
		SelectedDisponibilidad = null;

		int duracionMin = SelectedEspecialidad?.Duracion ?? 0;
		DateTime desde = DateTime.SpecifyKind(
			SelectedFecha < DateTime.Now ? DateTime.Now : SelectedFecha,
			DateTimeKind.Local
		).AddMinutes(duracionMin);

		List<Disponibilidad2025> lista = await App.Repositorio.SelectDisponibilidades(
			especialidad: esp.Codigo,
			cuantos: 40,
			apartirDeCuando: desde
		);

		foreach (Disponibilidad2025 d in lista)
			DisponibilidadesItemsSource.Add(new(d));

		DisponibilidadesView?.Refresh();
	}


	private void ActualizarDiasSemana() {
		DiasSemanaItemsSource.Clear();
		if (SelectedMedico is null) return;

		foreach (DayOfWeek dia in SelectedMedico.DiasAtencion) {
			DiasSemanaItemsSource.Add(new DiaDeSemanaViewModel(dia, dia.ATexto()));
		}

		SelectedDiaValue = DiasSemanaItemsSource.FirstOrDefault();
	}


	public async Task LoadMedicosPorEspecialidadAsync(EspecialidadCodigo? esp) {
		MedicosEspecialistasItemsSource.Clear();
		if (esp is not EspecialidadCodigo codigo) return;

		if (MedicosTodos == null)
			await LoadMedicosTodosAsync();

		foreach (MedicoSimpleViewModel? m in MedicosTodos!.Where(x => x.EspecialidadCodigo == codigo))
			MedicosEspecialistasItemsSource.Add(m);

		if (MedicosEspecialistasItemsSource.Any()) {
			SelectedMedico = MedicosEspecialistasItemsSource.First();
		} else {
			SelectedMedico = null;
		}
		OnPropertyChanged(nameof(ComboBoxMedicos_Enabled));
	}
	public async Task LoadMedicosTodosAsync() {
		List<MedicoDbModel> medicos = await App.Repositorio.SelectMedicos();

		IEnumerable<Task<MedicoSimpleViewModel>> tasks = medicos.Select(async medico => {
			IReadOnlyList<DayOfWeek>? dias =
				await App.Repositorio.SelectDiasDeAtencionWhereMedicoId(medico.Id)
				?? [];

			return new MedicoSimpleViewModel(
				Id: medico.Id,
				EspecialidadCodigo: medico.EspecialidadCodigo,
				Displayear: $"{medico.Nombre} {medico.Apellido}",
				DiasAtencion: dias
			);
		});

		MedicosTodos = [.. (await Task.WhenAll(tasks))];

		OnPropertyChanged(nameof(MedicosTodos));
		OnPropertyChanged(nameof(ComboBoxMedicos_Enabled));
	}


	// ================================================================
	// SELECTEDITEMS
	// ================================================================


	private EspecialidadViewModel? _selectedEspecialidad;
	public EspecialidadViewModel? SelectedEspecialidad {
		get => _selectedEspecialidad;
		set {
			if (_selectedEspecialidad == value) return;
			_selectedEspecialidad = value;

			OnPropertyChanged(nameof(SelectedEspecialidad));
			OnPropertyChanged(nameof(SelectedMedico));
			OnPropertyChanged(nameof(BotonBuscar_Enabled));
			OnPropertyChanged(nameof(ComboBoxMedicos_Enabled));

			_ = LoadMedicosPorEspecialidadAsync(value?.Codigo);
		}
	}



	private MedicoSimpleViewModel? _selectedMedico;
	public MedicoSimpleViewModel? SelectedMedico {
		get => _selectedMedico;
		set {
			if (_selectedMedico == value) return;
			_selectedMedico = value;
			OnPropertyChanged(nameof(SelectedMedico));
			//OnPropertyChanged(nameof(ComboBoxMedicos_Enabled)); // <--- importante
			OnPropertyChanged(nameof(ComboBoxDiasSemana_Enabled));
			OnPropertyChanged(nameof(BotonBuscar_Enabled)); // <--- importante
			OnPropertyChanged(nameof(BotonAgendar_Enabled));
			ActualizarDiasSemana();
			//CheckBoxDiaSemana_Enabled = SelectedMedico != null;

			//MessageBox.Show(SelectedMedico?.ToString());

		}
	}


	private DiaDeSemanaViewModel? _selectedDiaValue;
	public DiaDeSemanaViewModel? SelectedDiaValue {
		get => _selectedDiaValue;
		set {
			if (_selectedDiaValue == value) return;
			_selectedDiaValue = value;
			OnPropertyChanged(nameof(SelectedDiaValue));
			OnPropertyChanged(nameof(BotonBuscar_Enabled));
			OnPropertyChanged(nameof(BotonAgendar_Enabled));
		}
	}


	private DateTime _selectedFecha = DateTime.Now;
	public DateTime SelectedFecha {
		get => _selectedFecha;
		set {
			if (_selectedFecha == value) return;
			_selectedFecha = value;
			OnPropertyChanged(nameof(SelectedFecha));
			OnPropertyChanged(nameof(BotonBuscar_Enabled));
		}
	}



	private DisponibilidadEspecialidadModelView? _selectedDisponibilidad;
	public DisponibilidadEspecialidadModelView? SelectedDisponibilidad {
		get => _selectedDisponibilidad;
		set {
			if (_selectedDisponibilidad == value) return;
			_selectedDisponibilidad = value;
			OnPropertyChanged(nameof(SelectedDisponibilidad));
			OnPropertyChanged(nameof(BotonBuscar_Enabled));
			OnPropertyChanged(nameof(ComboBoxDiasSemana_Enabled));
			OnPropertyChanged(nameof(InformeReservaDeTurno));
			OnPropertyChanged(nameof(BotonAgendar_Enabled));
		}
	}


	// ================================================================
	// INFRAESTRUCTURA
	// ================================================================


	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string name)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));



}



public record DiaDeSemanaViewModel(DayOfWeek Value, string DiaNombre) {
	public static readonly List<DiaDeSemanaViewModel> Todos = [
		new(DayOfWeek.Monday,    "Lunes"),
		new(DayOfWeek.Tuesday,   "Martes"),
		new(DayOfWeek.Wednesday, "Miércoles"),
		new(DayOfWeek.Thursday,  "Jueves"),
		new(DayOfWeek.Friday,    "Viernes"),
		new(DayOfWeek.Saturday,  "Sábado"),
		new(DayOfWeek.Sunday,    "Domingo")
	];
};



public record DisponibilidadEspecialidadModelView(
	string Fecha,
	string Hora,
	string MedicoDisplayear,
	DiaDeSemanaViewModel DiaSemana,
	Disponibilidad2025 Original
) {
	internal DisponibilidadEspecialidadModelView(Disponibilidad2025 domainValue)
		: this(
			Fecha: domainValue.FechaHoraDesde.ATextoDia(),
			Hora: domainValue.FechaHoraDesde.ATextoHoras(),
			MedicoDisplayear: BuildMedicoDisplay(domainValue.MedicoId),
			DiaSemana: new DiaDeSemanaViewModel(
				domainValue.FechaHoraDesde.DayOfWeek,
				domainValue.FechaHoraDesde.DayOfWeek.ATexto()
			),
			Original: domainValue
		) {
	}

	private static string BuildMedicoDisplay(MedicoId medicoId) {
		var medico = RepoCache.DictMedicos.GetValueOrDefault(medicoId);
		return medico is null ? "Médico desconocido" : $"{medico.Nombre} {medico.Apellido}";
	}
}



public record MedicoSimpleViewModel(
	MedicoId Id,
	EspecialidadCodigo EspecialidadCodigo,
	string Displayear,
	IReadOnlyList<DayOfWeek> DiasAtencion
) {
	internal MedicoSimpleViewModel(MedicoDbModel medico, IReadOnlyList<DayOfWeek> dias)
		: this(
			Id: medico.Id,
			EspecialidadCodigo: medico.EspecialidadCodigo,
			Displayear: $"{medico.Nombre} {medico.Apellido}",
			DiasAtencion: dias
		) {
	}
}