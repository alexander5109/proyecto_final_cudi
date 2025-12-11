using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

//----------------------------------internal Viewmodel for SecretariaFormularioTurno-------------------------
internal class MyViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string name)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));



	public ObservableCollection<EspecialidadViewModel> EspecialidadesDisponiblesItemsSource { get; }
		= [];

	public MyViewModel(PacienteDbModel paciente) {
		SelectedPaciente = paciente;

		EspecialidadesDisponiblesItemsSource.Clear();
		foreach (var esp in Especialidad2025.Todas.Select(x => x.ToSimpleViewModel()))
			EspecialidadesDisponiblesItemsSource.Add(esp);

		_ = LoadMedicosTodosAsync();
		LoadHoras();
	}

	public MyViewModel(PacienteDbModel paciente, EspecialidadCodigo especialidad) {
		SelectedPaciente = paciente;

		EspecialidadesDisponiblesItemsSource.Clear(); // <<---- importantísimo

		var espResult = Especialidad2025.CrearResult(especialidad);

		espResult.MatchAndDo(
			ok => {
				var vm = ok.ToSimpleViewModel();
				SelectedEspecialidad = vm;
				EspecialidadesDisponiblesItemsSource.Add(vm);
			},
			err => {
				MessageBox.Show($"El código de especialidad no existe <{especialidad}>");

				foreach (var esp in Especialidad2025.Todas.Select(x => x.ToSimpleViewModel()))
					EspecialidadesDisponiblesItemsSource.Add(esp);
			});

		_ = LoadMedicosTodosAsync();
		LoadHoras();
	}



	private MedicoSimpleViewModel? _selectedMedico;
	public MedicoSimpleViewModel? SelectedMedico {
		get => _selectedMedico;
		set {
			if (_selectedMedico == value) return;
			_selectedMedico = value;
			OnPropertyChanged(nameof(SelectedMedico));
			OnPropertyChanged(nameof(HayMedicoSeleccionado));
			OnPropertyChanged(nameof(BotonBuscarDisponibilidadEnabled)); // <--- importante
			OnPropertyChanged(nameof(ComboBoxDiasSemanaEnabled));
			ActualizarDiasSemana();
			//FiltroDiaEnabled = SelectedMedico != null;

			//MessageBox.Show(SelectedMedico?.ToString());

		}
	}
	public string InformeReservaDeTurno {
		get {
			if (SelectedDisponibilidad is null || SelectedEspecialidad is null) return "";
			return $"Se va a reservar un turno para {SelectedPacienteNombreCompleto} " +
				   $"para consulta de {SelectedEspecialidad.NombreEspecialidad} " +
				   $"con el profesional {SelectedDisponibilidad.MedicoDisplayear} " +
				   $"el día {SelectedDisponibilidad.Fecha} a las {SelectedDisponibilidad.Hora}. Confirmar?";
		}
	}


	public bool HayMedicoSeleccionado => SelectedMedico != null;
	public bool ComboBoxDiasSemanaEnabled => FiltroDiaEnabled && SelectedMedico != null && SelectedMedico.DiasAtencion.Any();
	public bool AgendarTurnoEnabled => DisponibilidadesItemsSource.Any() && SelectedDisponibilidad != null;


	public DisponibilidadEspecialidadModelView? SelectedDisponibilidad {
		get => _selectedDisponibilidad;
		set {
			if (_selectedDisponibilidad == value) return;
			_selectedDisponibilidad = value;
			OnPropertyChanged(nameof(SelectedDisponibilidad));
			OnPropertyChanged(nameof(BotonBuscarDisponibilidadEnabled));
			OnPropertyChanged(nameof(ComboBoxDiasSemanaEnabled));
			OnPropertyChanged(nameof(InformeReservaDeTurno));
			OnPropertyChanged(nameof(AgendarTurnoEnabled));
		}
	}






	public PacienteDbModel SelectedPaciente { get; private set; }

	public string? SelectedPacienteDomicilioCompleto => $"{SelectedPaciente?.Localidad}, {SelectedPaciente?.Domicilio}";
	public string? SelectedPacienteNombreCompleto => $"{SelectedPaciente?.Nombre} {SelectedPaciente?.Apellido}";

	public IReadOnlyList<MedicoSimpleViewModel>? MedicosTodos { get; private set; }
	public ObservableCollection<MedicoSimpleViewModel> MedicosEspecialistasItemsSource { get; } = [];



	public bool ComboBoxMedicosEnabled => MedicosEspecialistasItemsSource.Any();

	private void ActualizarDiasSemana() {
		DiasSemanaItemsSource.Clear();
		if (SelectedMedico is null) return;

		foreach (var dia in SelectedMedico.DiasAtencion) {
			DiasSemanaItemsSource.Add(new DiaDeSemanaViewModel(dia, dia.ATexto()));
		}

		SelectedDiaValue = DiasSemanaItemsSource.FirstOrDefault();
	}


	public async Task LoadMedicosTodosAsync() {
		List<MedicoDbModel> medicos = await App.Repositorio.SelectMedicos();
		MedicosTodos = [.. medicos.Select(m => m.ToSimpleViewModel())];
		OnPropertyChanged(nameof(MedicosTodos));
		OnPropertyChanged(nameof(ComboBoxMedicosEnabled));
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
		OnPropertyChanged(nameof(ComboBoxMedicosEnabled));

	}

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

	private void LoadHoras() {
		HorasItemsSource.Clear();

		TimeOnly inicio = ClinicaNegocio.Atencion.DesdeHs;
		TimeOnly fin = ClinicaNegocio.Atencion.HastaHs;

		for (TimeOnly t = inicio; t <= fin; t = t.AddMinutes(30))
			HorasItemsSource.Add(t);

		if (HorasItemsSource.Any())
			SelectedHoraValue = HorasItemsSource[0];
	}




	public ObservableCollection<DiaDeSemanaViewModel> DiasSemanaItemsSource { get; } =
		[.. DiaDeSemanaViewModel.Todos];

	private DiaDeSemanaViewModel? _selectedDiaValue;
	public DiaDeSemanaViewModel? SelectedDiaValue {
		get => _selectedDiaValue;
		set {
			if (_selectedDiaValue == value) return;
			_selectedDiaValue = value;
			OnPropertyChanged(nameof(SelectedDiaValue));
			OnPropertyChanged(nameof(BotonBuscarDisponibilidadEnabled));
			OnPropertyChanged(nameof(AgendarTurnoEnabled));
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


	private EspecialidadViewModel? _selectedEspecialidad;
	public EspecialidadViewModel? SelectedEspecialidad {
		get => _selectedEspecialidad;
		set {
			if (_selectedEspecialidad == value) return;
			_selectedEspecialidad = value;

			OnPropertyChanged(nameof(SelectedEspecialidad));
			OnPropertyChanged(nameof(SelectedMedico));
			OnPropertyChanged(nameof(BotonBuscarDisponibilidadEnabled));
			OnPropertyChanged(nameof(ComboBoxMedicosEnabled));

			_ = LoadMedicosPorEspecialidadAsync(value?.Codigo);
		}
	}

	public ObservableCollection<DisponibilidadEspecialidadModelView> DisponibilidadesItemsSource { get; } = [];

	private DisponibilidadEspecialidadModelView? _selectedDisponibilidad;

	public bool BotonBuscarDisponibilidadEnabled => SelectedMedico != null && SelectedMedico.DiasAtencion.Any();

	public async Task RefreshDisponibilidadesAsync() {
		if (SelectedEspecialidad is not EspecialidadViewModel esp) return;

		DisponibilidadesItemsSource.Clear();

		DateTime desde = PreferedFechaValue + SelectedHoraValue.ToTimeSpan();

		List<Disponibilidad2025> lista = await App.Repositorio.SelectDisponibilidades(
			especialidad: esp.Codigo,
			cuantos: 15,
			apartirDeCuando: desde
		);

		foreach (Disponibilidad2025 d in lista)
			DisponibilidadesItemsSource.Add(await d.ToSimpleViewModel());
	}
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



public record MedicoSimpleViewModel(MedicoId Id, EspecialidadCodigo EspecialidadCodigo, string Displayear, IReadOnlyList<DayOfWeek> DiasAtencion);
public record DisponibilidadEspecialidadModelView(
	string Fecha,
	string Hora,
	string MedicoDisplayear,
	DiaDeSemanaViewModel DiaSemana,
	Disponibilidad2025 Original
);
public record EspecialidadViewModel(EspecialidadCodigo Codigo, string NombreEspecialidad, int Duracion);

internal static class ExtensionesLocales {

	internal static MedicoSimpleViewModel ToSimpleViewModel(this MedicoDbModel model) {
		List<DayOfWeek> dias = [];
		if (!string.IsNullOrWhiteSpace(model.HorariosJson)) {
			var horarios = System.Text.Json.JsonSerializer.Deserialize<List<HorarioDto>>(model.HorariosJson);
			dias = horarios is null ? [] : [.. horarios.Select(h => h.DiaSemana).Distinct()];
		}

		return new MedicoSimpleViewModel(
			Id: model.Id,
			EspecialidadCodigo: model.EspecialidadCodigo,
			Displayear: $"{model.Nombre} {model.Apellido}",
			DiasAtencion: dias
		);
	}

	private record HorarioDto(int Id, int MedicoId, DayOfWeek DiaSemana, TimeOnly HoraDesde, TimeOnly HoraHasta);

	internal static EspecialidadViewModel ToSimpleViewModel(this Especialidad2025 instance) {
		return new EspecialidadViewModel(
			Codigo: instance.Codigo,
			NombreEspecialidad: instance.Titulo,
			//NombreEspecialidad: $"{instance.Titulo} --- (Duración consulta: {instance.DuracionConsultaMinutos})"
			Duracion: instance.DuracionConsultaMinutos
		);
	}

	async internal static Task<DisponibilidadEspecialidadModelView> ToSimpleViewModel(this Disponibilidad2025 domainValue) {
		MedicoDbModel? medico = RepoCache.DictMedicos.GetValueOrDefault(domainValue.MedicoId);
		return new DisponibilidadEspecialidadModelView(
			Fecha: domainValue.FechaHoraDesde.ATextoDia(),
			Hora: domainValue.FechaHoraDesde.ATextoHoras(),
			MedicoDisplayear: $"{medico?.Nombre}{medico?.Apellido}",
			DiaSemana: new DiaDeSemanaViewModel(domainValue.FechaHoraDesde.DayOfWeek, domainValue.FechaHoraDesde.DayOfWeek.ATexto()),
			Original: domainValue
		);
	}








}


