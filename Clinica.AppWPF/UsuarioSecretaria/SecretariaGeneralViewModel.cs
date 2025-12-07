using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Clinica.AppWPF.UsuarioSecretaria;
using Clinica.Dominio.Entidades;

public class SecretariaGeneralViewModel : INotifyPropertyChanged {
	// -----------------------------
	// PACIENTES
	// -----------------------------
	public ObservableCollection<PacienteVM> PacientesList { get; }
		= new();

	private PacienteVM? _selectedPaciente;
	public PacienteVM? SelectedPaciente {
		get => _selectedPaciente;
		set {
			_selectedPaciente = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(PuedeModificarPaciente));
			OnPropertyChanged(nameof(PuedeReservarTurno));
		}
	}

	public string FiltroPacientesTexto {
		get => _filtroPacientesTexto;
		set { _filtroPacientesTexto = value; OnPropertyChanged(); FiltrarPacientes(); }
	}
	private string _filtroPacientesTexto = string.Empty;

	public bool PuedeModificarPaciente => SelectedPaciente is not null;
	public bool PuedeReservarTurno => SelectedPaciente is not null;

	// -----------------------------
	// TURNOS
	// -----------------------------
	public ObservableCollection<TurnoVM> TurnosList { get; }
		= new();

	private TurnoVM? _selectedTurno;
	public TurnoVM? SelectedTurno {
		get => _selectedTurno;
		set {
			_selectedTurno = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(HayTurnoSeleccionado));
		}
	}

	public bool HayTurnoSeleccionado => SelectedTurno is not null;

	public DateOnly? FiltroFecha { get; set; }
	public string FiltroTurnosPaciente { get; set; } = "";
	public string FiltroTurnosMedico { get; set; } = "";
	public string EstadoSeleccionado { get; set; } = "";
	public string EspecialidadSeleccionada { get; set; } = "";
	public string DiaSemanaSeleccionado { get; set; } = "";

	public IEnumerable<string> Estados { get; } = Enum.GetNames(typeof(EstadoTurno));
	public IEnumerable<string> Especialidades { get; } = new[] { "CARD", "DERM", "CLIN" };
	public IEnumerable<string> DiasSemana { get; } = Enum.GetNames(typeof(DayOfWeek));

	// Aquí van los Commands (confirmar asistencia, cancelar, etc.)

	// -----------------------------
	// Métodos de filtrado
	// -----------------------------
	private void FiltrarPacientes() {
		var cvs = (CollectionView)CollectionViewSource.GetDefaultView(PacientesList);
		cvs.Filter = obj => {
			if (obj is not PacienteVM p) return false;
			if (string.IsNullOrWhiteSpace(FiltroPacientesTexto)) return true;
			return (p.Nombre + " " + p.Apellido + " " + p.Dni)
				.Contains(FiltroPacientesTexto, StringComparison.OrdinalIgnoreCase);
		};
		cvs.Refresh();
	}

	protected void OnPropertyChanged([CallerMemberName] string? prop = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

	public event PropertyChangedEventHandler? PropertyChanged;
}
