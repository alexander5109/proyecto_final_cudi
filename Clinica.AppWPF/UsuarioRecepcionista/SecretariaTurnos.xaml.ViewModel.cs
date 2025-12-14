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
	public List<TurnoEstadoCodigo> Estados { get; } = [.. Enum.GetValues<TurnoEstadoCodigo>()];




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
		await App.Repositorio.EnsureMedicosLoaded(); //just to generate the dictionaries for the views.
		await App.Repositorio.EnsurePacientesLoaded();


		// VOY A SACAR LOS TRY EXCEPT DE TODOS LOS LLAMADOS AL REPO.
		// EL REPOSITORIO DEBERIA UTILIZAR EL SISTEMA ResultWpf. me falta implementarlo todavia. 
		//try { 
		// await App.Repositorio.SelectTurnos();
		//} catch (Exception ex) {
		//	MessageBox.Show("Error cargando turnos: " + ex.Message);
		//	return;
		//}

		List<TurnoDbModel> turnos = await App.Repositorio.SelectTurnos();
		_todosLosTurnos = [.. turnos.Select(
			async t => {
				var vm = new TurnoViewModel(t);
				await vm.LoadRelacionesAsync();
				return vm;
			}
		).Select(
			t => t.Result
		)];
		SelectedTurno = null;

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

		// 🔹 Filtro por medico
		if (!string.IsNullOrWhiteSpace(FiltroTurnosMedico)) {
			var txt = FiltroTurnosMedico.Trim();

			origen = origen.Where(t =>
				t.MedicoDisplayear.Contains(
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
	// FILTROS
	// ================================================================

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



	// ================================================================
	// INFRAESTRUCTURA
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));
}