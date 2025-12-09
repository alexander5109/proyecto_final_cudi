using System.ComponentModel;
using System.Windows;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.Dtos.ApiDtos;
using static Clinica.Shared.Dtos.DbModels;


namespace Clinica.AppWPF.UsuarioSecretaria;

public sealed class TurnoVM(Shared.Dtos.DbModels.TurnoDbModel model) {
    public TurnoId Id { get; } = model.Id;
    public string PacienteDisplayear { get; } = "(await model.SelectedPacienteId.RespectivoPaciente()).Nombre+Apellido";
    public string PacienteDni { get; } = "(await model.SelectedPacienteId.RespectivoPaciente()).Dni";
    public string MedicoDisplayear { get; } = "(await model.MedicoId.RespectivoMedico()).Nombre + Apellido";
    public EspecialidadCodigo EspecialidadCodigo { get; } = model.EspecialidadCodigo;
    public string FechaSolicitud { get; } = model.FechaHoraAsignadaDesde.AFechaArgentina();
    public string FechaAsignada { get; } = model.FechaHoraAsignadaDesde.AFechaArgentina();
    public string HoraAsignada { get; } = model.FechaHoraAsignadaHasta.ATexto();
    public TurnoEstadoCodigo OutcomeEstado { get; } = model.OutcomeEstado;
    public DateTime? OutcomeFecha { get; set; } = model.OutcomeFecha;
    public string? OutcomeComentario { get; set; } = model.OutcomeComentario;
}
public sealed class SecretariaGestionDeTurnosViewModel : INotifyPropertyChanged {
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
	private List<TurnoVM> _turnos = [];
	public List<TurnoVM> TurnosList {
		get => _turnos;
		set { _turnos = value; OnPropertyChanged(nameof(TurnosList)); }
	}

	private TurnoVM? _turnoSeleccionado;
	public TurnoVM? SelectedTurno {
		get => _turnoSeleccionado;
		set {
			if (_turnoSeleccionado != value) {
				_turnoSeleccionado = value;
				OnPropertyChanged(nameof(SelectedTurno));
				OnPropertyChanged(nameof(PuedeModificarTurnoSeleccionado));
				OnPropertyChanged(nameof(HayTurnoSeleccionado));
				OnPropertyChanged(nameof(ComentarioObligatorio));
			}
		}
	}

	public bool HayTurnoSeleccionado => SelectedTurno is not null;
	public bool PuedeModificarTurnoSeleccionado => SelectedTurno?.OutcomeEstado == TurnoEstadoCodigo.Programado;

	public bool ComentarioObligatorio { get; private set; }

	public void IndicarAccionRequiereComentario(bool requiere) {
		ComentarioObligatorio = requiere;
		OnPropertyChanged(nameof(ComentarioObligatorio));
	}


	private void OnPropertyChanged(string propertyName) =>
		PropertyChanged?.Invoke(this, new(propertyName));
}
