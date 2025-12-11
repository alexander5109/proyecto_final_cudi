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
	public string PacienteDisplayear => PacienteRelacionado is null ? "N/A" : $"{PacienteRelacionado.Nombre} {PacienteRelacionado.Apellido} {PacienteRelacionado.Dni}";
	public string MedicoDisplayear => MedicoRelacionado is null ? "N/A" : $"{MedicoRelacionado.Nombre} {MedicoRelacionado.Apellido} {MedicoRelacionado.Dni}";
	public EspecialidadCodigo EspecialidadCodigo { get; } = model.EspecialidadCodigo;
	public string FechaSolicitud { get; } = model.FechaHoraAsignadaDesde.ATextoDia();
	public string FechaAsignada { get; } = model.FechaHoraAsignadaDesde.ATextoDia();
	public string HoraAsignada { get; } = model.FechaHoraAsignadaHasta.ATextoHoras();
	public TurnoEstadoCodigo OutcomeEstado { get; } = model.OutcomeEstado;
	public DateTime? OutcomeFecha { get; set; } = model.OutcomeFecha;
	public string? OutcomeComentario { get; set; } = model.OutcomeComentario;

	public readonly TurnoDbModel Original = model;
}
public sealed class RecepcionistaGestionDeTurnosViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;

	// ==== PACIENTES ====
	private List<PacienteDbModel> _pacientes = [];
	public List<PacienteDbModel> PacientesList {
		get => _pacientes;
		set { _pacientes = value; OnPropertyChanged(nameof(PacientesList)); }
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


	// ==== TURNOS ====
	private List<TurnoViewModel> _turnos = [];
	public List<TurnoViewModel> TurnosList {
		get => _turnos;
		set { _turnos = value; OnPropertyChanged(nameof(TurnosList)); }
	}

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
