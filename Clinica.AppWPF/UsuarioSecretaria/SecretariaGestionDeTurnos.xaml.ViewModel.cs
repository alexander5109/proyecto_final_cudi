using System.ComponentModel;
using System.Windows;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.Dtos.ApiDtos;
using static Clinica.Shared.Dtos.DbModels;


namespace Clinica.AppWPF.UsuarioSecretaria;

public sealed class TurnoVM {
	public TurnoId Id { get; }
	public string PacienteDisplayear { get; }
	public string PacienteDni { get; }
	public string MedicoDisplayear { get; }
	public EspecialidadCodigo EspecialidadCodigo { get; }
	public string FechaSolicitud { get; }
	public string FechaAsignada { get; }
	public string HoraAsignada { get; }
	public TurnoOutcomeEstadoCodigo2025 OutcomeEstado { get; }
	public DateTime? OutcomeFecha { get; set; }
	public string? OutcomeComentario { get; set; }
	public TurnoVM(TurnoDbModel model) {
		Id = model.Id;
		PacienteDisplayear = "(await model.SelectedPacienteId.RespectivoPaciente()).Nombre+Apellido";
		PacienteDni = "(await model.SelectedPacienteId.RespectivoPaciente()).Dni";
		MedicoDisplayear = "(await model.MedicoId.RespectivoMedico()).Nombre + Apellido";
		FechaSolicitud = model.FechaHoraAsignadaDesde.AFechaArgentina();
		FechaAsignada = model.FechaHoraAsignadaDesde.AFechaArgentina();
		HoraAsignada = model.FechaHoraAsignadaHasta.AHorasArgentina();
		EspecialidadCodigo = model.EspecialidadCodigo;
		OutcomeEstado = model.OutcomeEstado;
		//MessageBox.Show($"{EspecialidadCodigo} {OutcomeEstado.ToString()}");
		OutcomeFecha = model.OutcomeFecha;
		OutcomeComentario = model.OutcomeComentario;
	}
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
	public bool PuedeModificarTurnoSeleccionado => SelectedTurno?.OutcomeEstado == TurnoOutcomeEstadoCodigo2025.Programado;

	public bool ComentarioObligatorio { get; private set; }

	public void IndicarAccionRequiereComentario(bool requiere) {
		ComentarioObligatorio = requiere;
		OnPropertyChanged(nameof(ComentarioObligatorio));
	}


	private void OnPropertyChanged(string propertyName) =>
		PropertyChanged?.Invoke(this, new(propertyName));
}
