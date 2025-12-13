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
	public string FechaSolicitud { get; } = model.FechaDeCreacion.ATextoDiaYHoras();
	public string FechaAsignada { get; } = model.FechaHoraAsignadaDesde.ATextoDiaYHoras();
	public string HoraAsignada { get; } = model.FechaHoraAsignadaHasta.ATextoHoras();
	public TurnoEstadoCodigo OutcomeEstado { get; } = model.OutcomeEstado;
	public DateTime? OutcomeFecha { get; set; } = model.OutcomeFecha;
	public string? OutcomeComentario { get; set; } = model.OutcomeComentario;

	public TurnoDbModel Original => model;
	//public readonly TurnoDbModel Original = model;
}
public sealed class SecretariaTurnosViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;

	// ================================================================
	// TURNOS
	// ================================================================

	private List<TurnoViewModel> _turnosOriginal = [];   // Original immutable list
	private List<TurnoViewModel> _turnos = [];           // Filtered list for the UI

	public List<TurnoViewModel> TurnosList {
		get => _turnos;
		private set {
			_turnos = value;
			OnPropertyChanged(nameof(TurnosList));
		}
	}

	public void CargarTurnos(List<TurnoViewModel> turnos) {
		_turnosOriginal = turnos;
		_turnos = turnos;

		// DEFAULT FILTER: only "Programado"
		EstadoSeleccionado = TurnoEstadoCodigo.Programado;
		AplicarFiltros();
	}

	// ================================================================
	// SELECTED TURNO
	// ================================================================

	private TurnoViewModel? _turnoSeleccionado;
	public TurnoViewModel? SelectedTurno {
		get => _turnoSeleccionado;
		set {
			if (_turnoSeleccionado != value) {
				_turnoSeleccionado = value;
				OnPropertyChanged(nameof(SelectedTurno));
				OnPropertyChanged(nameof(HayTurnoSeleccionado));
				OnPropertyChanged(nameof(PuedeCancelarTurno));
				OnPropertyChanged(nameof(PuedeConfirmarTurno));
				OnPropertyChanged(nameof(PuedeMarcarComoAusente));
			}
		}
	}

	public bool HayTurnoSeleccionado => SelectedTurno is not null;


	// ================================================================
	// FILTER: PACIENTE (search in PacienteDisplayear)
	// ================================================================

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

	// ================================================================
	// FILTER: ESTADO
	// ================================================================

	public List<TurnoEstadoCodigo> Estados { get; }
		= Enum.GetValues<TurnoEstadoCodigo>().ToList();

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

	// ================================================================
	// APPLY FILTERS (ONLY TWO)
	// ================================================================

	private void AplicarFiltros() {
		IEnumerable<TurnoViewModel> query = _turnosOriginal;

		// Filtro por estado
		if (EstadoSeleccionado is not null)
			query = query.Where(t => t.OutcomeEstado == EstadoSeleccionado);

		// Filtro por paciente: searches in PacienteDisplayear
		if (!string.IsNullOrWhiteSpace(FiltroTurnosPaciente)) {
			string txt = FiltroTurnosPaciente.Trim().ToLowerInvariant();
			query = query.Where(t =>
				t.PacienteDisplayear.Contains(txt, StringComparison.InvariantCultureIgnoreCase)
            );
		}

		TurnosList = query.ToList();
	}

	// ================================================================
	// POLICIES
	// ================================================================

	public bool PuedeMarcarComoAusente =>
		SelectedTurno != null &&
		TurnoPolicyRaw.PuedeMarcarComoAusente(
			SelectedTurno.OutcomeEstado,
			SelectedTurno.Original.FechaHoraAsignadaHasta,
			SelectedTurno.OutcomeFecha is not null,
			DateTime.Now
		);

	public bool PuedeConfirmarTurno =>
		SelectedTurno != null &&
		TurnoPolicyRaw.PuedeConfirmar(
			SelectedTurno.OutcomeEstado,
			SelectedTurno.Original.FechaHoraAsignadaDesde,
			SelectedTurno.OutcomeFecha is not null,
			DateTime.Now
		);

	public bool PuedeCancelarTurno =>
		SelectedTurno != null &&
		TurnoPolicyRaw.PuedeCancelar(
			SelectedTurno.OutcomeEstado,
			SelectedTurno.Original.FechaHoraAsignadaDesde,
			SelectedTurno.OutcomeFecha is not null,
			DateTime.Now
		);

	// ================================================================
	// UTILS
	// ================================================================

	private void OnPropertyChanged(string prop) =>
		PropertyChanged?.Invoke(this, new(prop));
}
