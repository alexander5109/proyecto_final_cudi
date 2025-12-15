using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.Shared.DbModels.DbModels;


namespace Clinica.AppWPF.UsuarioRecepcionista;

public sealed class SecretariaTurnosViewModel : INotifyPropertyChanged {
	// ================================================================
	// CONSTRUCTOR
	// ================================================================
	public SecretariaTurnosViewModel() {
		TurnosView = CollectionViewSource.GetDefaultView(_todosLosTurnos);
		TurnosView.Filter = FilterTurnos;
	}



	// ================================================================
	// COLECCIONES
	// ================================================================
	private List<TurnoViewModel> _todosLosTurnos = [];
	public ICollectionView TurnosView { get; private set; }
	public List<TurnoEstadoEnum> Estados { get; } = [.. Enum.GetValues<TurnoEstadoEnum>()];




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
	// METODOS DE DOMINIO
	// ================================================================
	public async Task<ResultWpf<UnitWpf>> MarcarAusenteAsync(string comentario, DateTime now) {
		// 1️⃣ Validaciones de estado
		if (SelectedTurno is null) {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo(
				"No hay un turno seleccionado para marcar como ausente.", MessageBoxImage.Information)
			);
		}

		if (comentario.Length < 10) {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo(
				"El comentario debe tener al menos 10 caracteres.", MessageBoxImage.Information)
			);
		}

		// 3️⃣ Llamada al repositorio
		return await App.Repositorio.MarcarTurnoComoAusente(
			SelectedTurno.Original.Id,
			now,
			comentario
		);
	}


	public async Task<ResultWpf<UnitWpf>> CancelarTurnoAsync(string comentario, DateTime now) {
		if (SelectedTurno is null) {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No hay turno seleccionado.", MessageBoxImage.Information));
		}
		if (comentario.Length < 10) {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("El comentario es muy corto.", MessageBoxImage.Information));
		}

		return await App.Repositorio.CancelarTurno(
			SelectedTurno.Original.Id,
			now,
			comentario
		);
	}

	public async Task<ResultWpf<UnitWpf>> ConfirmarAsistenciaAsync(DateTime fechaAsistencia) {
		// 1️⃣ Validaciones de estado (UI-agnósticas)
		if (SelectedTurno is null) {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo(
				"No hay un turno seleccionado para confirmar la asistencia.")
			);
		}

		// (opcional, pero prolijo)
		if (fechaAsistencia == default) {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo(
				"La fecha de asistencia es inválida.")
			);
		}

		// 2️⃣ Delegación al repositorio
		return await App.Repositorio.MarcarTurnoComoConcretado(
			SelectedTurno.Original.Id,
			fechaAsistencia,
			SelectedTurno.Original.OutcomeComentario
		);
	}



	// ================================================================
	// METODOS DE UI
	// ================================================================
	internal async Task RefrescarTurnosAsync() {
		await App.Repositorio.EnsureMedicosLoaded(); 
		await App.Repositorio.EnsurePacientesLoaded();
        List<TurnoDbModel> turnos = await App.Repositorio.SelectTurnos();
        IEnumerable<Task<TurnoViewModel>> turnoTasks = turnos.Select(async t => {
            TurnoViewModel vm = new(t);
			await vm.LoadRelacionesAsync();
			return vm;
		});
		_todosLosTurnos = (await Task.WhenAll(turnoTasks)).ToList();

		// Reasignamos la vista para que refleje la nueva lista
		TurnosView = CollectionViewSource.GetDefaultView(_todosLosTurnos);
		TurnosView.Filter = FilterTurnos;

		OnPropertyChanged(nameof(TurnosView));
		SelectedTurno = null;
		AplicarFiltros();
	}

	private void AplicarFiltros() {
		TurnosView.Refresh();
		if (SelectedTurno != null && !TurnosView.Cast<TurnoViewModel>().Contains(SelectedTurno))
			SelectedTurno = null;
	}
	// ================================================================
	// FILTROS
	// ================================================================

	private TurnoEstadoEnum? _estadoSeleccionado;
	public TurnoEstadoEnum? EstadoSeleccionado {
		get => _estadoSeleccionado;
		set {
			if (_estadoSeleccionado != value) {
				_estadoSeleccionado = value;
				OnPropertyChanged(nameof(EstadoSeleccionado));
				AplicarFiltros();
			}
		}
	}

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

	private string _filtroTurnosMedico = "";
	public string FiltroTurnosMedico {
		get => _filtroTurnosMedico;
		set {
			if (_filtroTurnosMedico != value) {
				_filtroTurnosMedico = value;
				OnPropertyChanged(nameof(FiltroTurnosMedico));
				AplicarFiltros();
			}
		}
	}


	private bool FilterTurnos(object obj) {
		if (obj is not TurnoViewModel t) return false;

		if (EstadoSeleccionado.HasValue && t.Original.OutcomeEstado != EstadoSeleccionado.Value)
			return false;

		if (!string.IsNullOrWhiteSpace(FiltroTurnosPaciente) &&
			!t.PacienteDisplayear.Contains(FiltroTurnosPaciente.Trim(), StringComparison.InvariantCultureIgnoreCase))
			return false;

		if (!string.IsNullOrWhiteSpace(FiltroTurnosMedico) &&
			!t.MedicoDisplayear.Contains(FiltroTurnosMedico.Trim(), StringComparison.InvariantCultureIgnoreCase))
			return false;

		return true;
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
	// INFRAESTRUCTURA
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));

}



// ================================================================
// VIEWMODELS PARA GRIDS
// ================================================================
public class TurnoViewModel(TurnoDbModel model) {
	public TurnoDbModel Original { get; } = model;

	internal async Task LoadRelacionesAsync() {
		await LoadPacienteRelacionadoAsync();
		await LoadMedicoRelacionadoAsync();
	}


	private PacienteDbModel? _pacienteRelacionado;
	public PacienteDbModel? PacienteRelacionado => _pacienteRelacionado;
	public string PacienteDisplayear => PacienteRelacionado is null ? "N/A" : $"{PacienteRelacionado.Dni}: {PacienteRelacionado.Nombre} {PacienteRelacionado.Apellido}";

	public async Task LoadPacienteRelacionadoAsync() {
		_pacienteRelacionado = await App.Repositorio.SelectPacienteWhereId(Original.PacienteId);
		OnPropertyChanged(nameof(PacienteRelacionado));
		OnPropertyChanged(nameof(PacienteDisplayear));
	}



	private MedicoDbModel? _medicoRelacionado;
	public MedicoDbModel? MedicoRelacionado => _medicoRelacionado;
	public string MedicoDisplayear => MedicoRelacionado is null ? "N/A" : $"{MedicoRelacionado.Nombre} {MedicoRelacionado.Apellido} {MedicoRelacionado.Dni}";

	public async Task LoadMedicoRelacionadoAsync() {
		_medicoRelacionado = await App.Repositorio.SelectMedicoWhereId(Original.MedicoId);
		OnPropertyChanged(nameof(MedicoRelacionado));
		OnPropertyChanged(nameof(MedicoDisplayear));
	}


	public string DiaSemana => Original.FechaHoraAsignadaDesde.DayOfWeek.ATexto();


	// ================================================================
	// INFRAESTRUCTURA
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));
}