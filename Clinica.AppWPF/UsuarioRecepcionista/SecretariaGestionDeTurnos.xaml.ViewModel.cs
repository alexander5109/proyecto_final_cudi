using System.ComponentModel;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.Shared.DbModels.DbModels;


namespace Clinica.AppWPF.UsuarioRecepcionista;

public sealed class TurnoViewModel(TurnoDbModel model) {

	public TurnoId Id { get; } = model.Id;
	public PacienteDbModel? PacienteRelacionado => RepoCache.DictPacientes.GetValueOrDefault(model.PacienteId);
	public MedicoDbModel? MedicoRelacionado => RepoCache.DictMedicos.GetValueOrDefault(model.MedicoId);
	public string PacienteDisplayear => PacienteRelacionado is null ? "N/A" : $"{PacienteRelacionado.Dni}: {PacienteRelacionado.Nombre} {PacienteRelacionado.Apellido}";
	public string MedicoDisplayear => MedicoRelacionado is null ? "N/A" : $"{MedicoRelacionado.Nombre} {MedicoRelacionado.Apellido} {MedicoRelacionado.Dni}";
	public EspecialidadCodigo EspecialidadCodigo { get; } = model.EspecialidadCodigo;
	public string FechaSolicitud { get; } = model.FechaHoraAsignadaDesde.ATextoDia();
	public string FechaAsignada { get; } = model.FechaHoraAsignadaDesde.ATextoDia();
	public string HoraAsignada { get; } = model.FechaHoraAsignadaHasta.ATextoHoras();
	public TurnoEstadoCodigo OutcomeEstado { get; } = model.OutcomeEstado;
	public DateTime? OutcomeFecha { get; set; } = model.OutcomeFecha;
	public string? OutcomeComentario { get; set; } = model.OutcomeComentario;

	public TurnoDbModel Original => model;
	//public readonly TurnoDbModel Original = model;
}
public sealed class RecepcionistaGestionDeTurnosViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;

	// ==== PACIENTES ====
	private List<PacienteDbModel> _pacientes = [];
	public List<PacienteDbModel> PacientesList {
		get => _pacientes;
		set { _pacientes = value; OnPropertyChanged(nameof(PacientesList)); }
	}




	// FILTRO ESPECIALIDAD
	public List<EspecialidadCodigo> Especialidades { get; } =
		Enum.GetValues<EspecialidadCodigo>().ToList();

	private EspecialidadCodigo? _especialidadSeleccionada;
	public EspecialidadCodigo? EspecialidadSeleccionada {
		get => _especialidadSeleccionada;
		set { _especialidadSeleccionada = value; OnPropertyChanged(nameof(EspecialidadSeleccionada)); AplicarFiltros(); }
	}

	// FILTRO POR DNI
	private string _filtroDniPaciente = "";
	public string FiltroDniPaciente {
		get => _filtroDniPaciente;
		set { _filtroDniPaciente = value; OnPropertyChanged(nameof(FiltroDniPaciente)); AplicarFiltros(); }
	}


	// ==================== FILTRO POR PACIENTE (texto) ====================
	private string _filtroTurnosPaciente = "";
	public string FiltroTurnosPaciente {
		get => _filtroTurnosPaciente;
		set {
			if (_filtroTurnosPaciente != value) {
				_filtroTurnosPaciente = value;
				OnPropertyChanged(nameof(FiltroTurnosPaciente));
				AplicarFiltros();
			}
		}
	}

	// ==================== FILTRO POR ESTADO ====================
	public List<TurnoEstadoCodigo> Estados { get; } =
		Enum.GetValues<TurnoEstadoCodigo>().ToList();

	private TurnoEstadoCodigo? _estadoSeleccionado;
	public TurnoEstadoCodigo? EstadoSeleccionado {
		get => _estadoSeleccionado;
		set {
			if (_estadoSeleccionado != value) {
				_estadoSeleccionado = value;
				OnPropertyChanged(nameof(EstadoSeleccionado));
				AplicarFiltros();
			}
		}
	}





	private void AplicarFiltros() {
		IEnumerable<TurnoViewModel> query = _turnosOriginal;

		// Filtro estado
		if (EstadoSeleccionado is not null)
			query = query.Where(t => t.OutcomeEstado == EstadoSeleccionado);

		// Filtro DNI
		if (!string.IsNullOrWhiteSpace(FiltroDniPaciente))
			query = query.Where(t => t.PacienteDisplayear.Contains(FiltroDniPaciente));

		// Filtro especialidad
		if (EspecialidadSeleccionada is not null)
			query = query.Where(t => t.EspecialidadCodigo == EspecialidadSeleccionada);

		TurnosList = query.ToList();


	}









	public bool ModificarPacienteCommand => SelectedPaciente is not null;

	private PacienteDbModel? _selectedPaciente;
	public PacienteDbModel? SelectedPaciente {
		get => _selectedPaciente;
		set {
			if (_selectedPaciente != value) {
				_selectedPaciente = value;
				OnPropertyChanged(nameof(SelectedPaciente));
				OnPropertyChanged(nameof(HayPacienteSeleccionado));
			}
		}
	}

	public bool HayPacienteSeleccionado => SelectedPaciente is not null;


	private List<TurnoViewModel> _turnosOriginal = [];

	public List<TurnoViewModel> TurnosList {
		get => _turnos;
		set {
			_turnosOriginal = value;
			_turnos = value;
			OnPropertyChanged(nameof(TurnosList));
		}
	}


	// ==== TURNOS ====
	private List<TurnoViewModel> _turnos = [];

	private TurnoViewModel? _turnoSeleccionado;
	public TurnoViewModel? SelectedTurno {
		get => _turnoSeleccionado;
		set {
			if (_turnoSeleccionado != value) {
				_turnoSeleccionado = value;
				OnPropertyChanged(nameof(SelectedTurno));
				OnPropertyChanged(nameof(PuedeCancelarTurno));
				OnPropertyChanged(nameof(PuedeConfirmarTurno));
				OnPropertyChanged(nameof(PuedeMarcarComoAusente));


				OnPropertyChanged(nameof(HayTurnoSeleccionado));
				OnPropertyChanged(nameof(ComentarioObligatorio));
			}
		}
	}

	public bool HayTurnoSeleccionado => SelectedTurno is not null;



	public bool PuedeMarcarComoAusente =>
		SelectedTurno != null
		&& TurnoPolicyRaw.PuedeMarcarComoAusente(
			estado: SelectedTurno.OutcomeEstado,
			fechaAsignadaHasta: SelectedTurno.Original.FechaHoraAsignadaHasta,
			tieneOutcome: SelectedTurno.OutcomeFecha is not null,
			ahora: DateTime.Now
		);
	public bool PuedeConfirmarTurno =>
		SelectedTurno != null
		&& TurnoPolicyRaw.PuedeConfirmar(
			estado: SelectedTurno.OutcomeEstado,
			fechaAsignadaDesde: SelectedTurno.Original.FechaHoraAsignadaDesde,
			tieneOutcome: SelectedTurno.OutcomeFecha is not null,
			ahora: DateTime.Now
		);
	public bool PuedeCancelarTurno =>
		SelectedTurno != null
		&& TurnoPolicyRaw.PuedeCancelar(
			estado: SelectedTurno.OutcomeEstado,
			fechaAsignadaDesde: SelectedTurno.Original.FechaHoraAsignadaDesde,
			tieneOutcome: SelectedTurno.OutcomeFecha is not null,
			ahora: DateTime.Now
		);

	public bool ComentarioObligatorio { get; private set; }

	public void IndicarAccionRequiereComentario(bool requiere) {
		ComentarioObligatorio = requiere;
		OnPropertyChanged(nameof(ComentarioObligatorio));
	}


	private void OnPropertyChanged(string propertyName) =>
		PropertyChanged?.Invoke(this, new(propertyName));
}
