using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using static Clinica.Shared.DbModels.DbModels;


namespace Clinica.AppWPF.UsuarioRecepcionista;

public sealed class SecretariaTurnosViewModel : INotifyPropertyChanged {

	// ================================================================
	// COLECCIONES
	// ================================================================

	private List<TurnoViewModel> _todosLosTurnos = [];   // Original immutable list
	public ObservableCollection<TurnoViewModel> TurnosList { get; } = [];


	// ================================================================
	// METODOS
	// ================================================================

	internal async Task RefrescarTurnosAsync() {
		try {
            List<TurnoDbModel> turnos = await App.Repositorio.SelectTurnos();
			_todosLosTurnos = turnos.Select(t => new TurnoViewModel(t)).ToList();
			SelectedTurno = null;
		} catch (Exception ex) {
			MessageBox.Show("Error cargando turnos: " + ex.Message);
			return;
		}
		//EstadoSeleccionado = TurnoEstadoCodigo.Programado; //nah, es molesto
		AplicarFiltros();
	}



	private void AplicarFiltros() {
        TurnoViewModel? seleccionado = SelectedTurno;

		TurnosList.Clear();

		IEnumerable<TurnoViewModel> origen = _todosLosTurnos;

		// 🔹 Filtro por estado
		if (EstadoSeleccionado.HasValue) {
			origen = origen.Where(turno => turno.Original.OutcomeEstado == EstadoSeleccionado.Value);
		}

		// 🔹 Filtro por paciente
		if (!string.IsNullOrWhiteSpace(FiltroTurnosPaciente)) {
			var txt = FiltroTurnosPaciente.Trim();

			origen = origen.Where(t =>
				t.PacienteDisplayear.Contains(
					txt,
					StringComparison.InvariantCultureIgnoreCase
				)
			);
		}

		foreach (TurnoViewModel turno in origen)
			TurnosList.Add(turno);

		// 🔹 Restaurar selección si sigue visible
		if (seleccionado != null && TurnosList.Contains(seleccionado))
			SelectedTurno = seleccionado;
		else
			SelectedTurno = null;
	}


	// ================================================================
	// ITEM SELECCIONADO
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
	// REGLAS
	// ================================================================

	public bool PuedeMarcarComoAusente =>
		SelectedTurno != null &&
		TurnoPolicyRaw.PuedeMarcarComoAusente(
			SelectedTurno.Original.OutcomeEstado,
			SelectedTurno.Original.FechaHoraAsignadaHasta,
			SelectedTurno.Original.OutcomeFecha is not null,
			DateTime.Now
		);

	public bool PuedeConfirmarTurno =>
		SelectedTurno != null &&
		TurnoPolicyRaw.PuedeConfirmar(
			SelectedTurno.Original.OutcomeEstado,
			SelectedTurno.Original.FechaHoraAsignadaDesde,
			SelectedTurno.Original.OutcomeFecha is not null,
			DateTime.Now
		);

	public bool PuedeCancelarTurno =>
		SelectedTurno != null &&
		TurnoPolicyRaw.PuedeCancelar(
			SelectedTurno.Original.OutcomeEstado,
			SelectedTurno.Original.FechaHoraAsignadaDesde,
			SelectedTurno.Original.OutcomeFecha is not null,
			DateTime.Now
		);

	public bool HayTurnoSeleccionado => SelectedTurno is not null;

	// ================================================================
	// UTILS
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));
}



// ================================================================
// VIEWMODELS PARA GRIDS
// ================================================================
public sealed class TurnoViewModel(TurnoDbModel model) {
	public PacienteDbModel? PacienteRelacionado => RepoCache.DictPacientes.GetValueOrDefault(model.PacienteId);
	public MedicoDbModel? MedicoRelacionado => RepoCache.DictMedicos.GetValueOrDefault(model.MedicoId);
	public string PacienteDisplayear => PacienteRelacionado is null ? "N/A" : $"{PacienteRelacionado.Dni}: {PacienteRelacionado.Nombre} {PacienteRelacionado.Apellido}";
	public string MedicoDisplayear => MedicoRelacionado is null ? "N/A" : $"{MedicoRelacionado.Nombre} {MedicoRelacionado.Apellido} {MedicoRelacionado.Dni}";
	public TurnoDbModel Original => model;
}