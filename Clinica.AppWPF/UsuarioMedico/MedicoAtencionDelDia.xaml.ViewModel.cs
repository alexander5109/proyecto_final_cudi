using System.Collections.ObjectModel;
using System.ComponentModel;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.AppWPF.UsuarioMedico;





public sealed class MedicoAtencionDelDiaVM(MedicoId2025 CurrentMedicoId) : INotifyPropertyChanged {
	// ==========================================================
	// BOTONES: SELECTED ITEMS
	// ==========================================================

	private PacienteDbModel? _selectedPaciente;
	public PacienteDbModel? SelectedPaciente {
		get => _selectedPaciente;
		set {
			if (_selectedPaciente != value) {
				_selectedPaciente = value;
				OnPropertyChanged(nameof(SelectedPaciente));
				OnPropertyChanged(nameof(HayPacienteSeleccionado));
				OnSelectedPacienteChanged();
			}
		}
	}


	private TurnoDeHoyVM? _turnoSeleccionado;
	public TurnoDeHoyVM? TurnoSeleccionado {
		get => _turnoSeleccionado;
		set {
			if (_turnoSeleccionado != value) {
				_turnoSeleccionado = value;
				OnPropertyChanged(nameof(TurnoSeleccionado));
				ActualizarPacienteDesdeTurno();
			}
		}
	}
	private string? _diagnosticoText;
	public string? DiagnosticoText {
		get => _diagnosticoText;
		set {
			if (_diagnosticoText != value) {
				_diagnosticoText = value;
				OnPropertyChanged(nameof(DiagnosticoText));
			}
		}
	}

	// ================================================================
	// COLECCIONES
	// ================================================================
	
	public ObservableCollection<AtencionPreviaVM> AtencionesViewModelList { get; } = [];
	private List<TurnoDbModel> _todosLosTurnos = []; // Copia completa para filtrar
	public ObservableCollection<TurnoDeHoyVM> TurnosList { get; } = [];



	// ================================================================
	// METHODS
	// ================================================================

	private void ActualizarPacienteDesdeTurno() {
		if (_turnoSeleccionado == null) {
			SelectedPaciente = null;
			return;
		}

		// Buscar paciente en cache según nombre/ID
		//SelectedPaciente = App.Repositorio.Pacientes.GetFromCachePacienteWhereId(_todosLosTurnos
		//	.First(t => t.FechaHoraAsignadaDesde.ToString("HH:mm") == _turnoSeleccionado.Hora
		//				&& t.PacienteId2025 == ??? // deberías tener PacienteId2025 en TurnoDeHoyVM o mapearlo

		//	).PacienteId2025);
	}

	private string CalcularEdad(DateTime fechaNacimiento) =>
		(DateTime.Today.Year - fechaNacimiento.Year - (DateTime.Today < fechaNacimiento.AddYears(DateTime.Today.Year - fechaNacimiento.Year) ? 1 : 0)).ToString();

	// ================================================================
	// METHODS.ASYNC
	// ================================================================

	public async Task ConfirmarDiagnosticoAsync() {
		if (SelectedPaciente == null || string.IsNullOrWhiteSpace(DiagnosticoText)) return;

		var turno = _todosLosTurnos.FirstOrDefault(t =>
			t.PacienteId == SelectedPaciente.Id &&
			t.OutcomeEstado != TurnoEstadoEnum.Concretado);

		if (turno == null) return;

		var result = await App.Repositorio.Atenciones.AgendarAtencionConDiagnostico(turno.Id, SelectedPaciente.Id);

		if (result.IsOk) {
			DiagnosticoText = null;
			await CargarAtencionesDePacienteSeleccionado();
			await RefrescarMisTurnosAsync();
		}
	}



	private async void OnSelectedPacienteChanged() {
		await CargarAtencionesDePacienteSeleccionado();
	}


	internal async Task RefrescarMisTurnosAsync() {
		List<TurnoDbModel> turnos = await App.Repositorio.Turnos.SelectTurnosWhereMedicoId(CurrentMedicoId);
		await App.Repositorio.Pacientes.RefreshCache();
		await App.Repositorio.Medicos.RefreshCache();

		_todosLosTurnos = turnos;
		TurnosList.Clear();


		foreach (TurnoDbModel t in turnos.OrderBy(t => t.FechaHoraAsignadaDesde)) {
			//PacienteDbModel? paciente = await App.Repositorio.Pacientes.SelectPacienteWhereId(t.PacienteId2025); // cache de pacientes
			PacienteDbModel? paciente = App.Repositorio.Pacientes.GetFromCachePacienteWhereId(t.PacienteId);
			if (paciente == null) continue;

			TurnosList.Add(new TurnoDeHoyVM(
				Hora: t.FechaHoraAsignadaDesde.ToString("HH:mm"),
				PacienteNombreApellido: $"{paciente.Nombre} {paciente.Apellido}",
				PacienteEdad: CalcularEdad(paciente.FechaNacimiento),
				FueAtendido: t.OutcomeEstado == TurnoEstadoEnum.Concretado ? "✔" : ""
			));
		}
	}


	private async Task CargarAtencionesDePacienteSeleccionado() {
		AtencionesViewModelList.Clear();

		if (SelectedPaciente is null) return;

        List<AtencionDbModel>? atenciones = await App.Repositorio.Atenciones.SelectAtencionesWherePacienteId(SelectedPaciente.Id);

		if (atenciones is null || atenciones.Count == 0) return;


		foreach (AtencionDbModel atencion in atenciones.OrderByDescending(x => x.FechaHora)) {
            TurnoDbModel? turno = _todosLosTurnos.FirstOrDefault(t => t.Id == atencion.TurnoId);
            string horaStr = turno?.FechaHoraAsignadaDesde.ToString("HH:mm") ?? "";


			string selectedMedicoNombreCompleto = App.Repositorio.Medicos.GetFromCacheMedicoDisplayWhereId(atencion.MedicoId);

			AtencionesViewModelList.Add(new AtencionPreviaVM(
				FechaHora: atencion.FechaHora.ToString("yyyy-MM-dd HH:mm"),
				MedicoNombreApellido: selectedMedicoNombreCompleto!,
				Observaciones: atencion.Observaciones
			));
		}
	}


	// ================================================================
	// REGLAS
	// ================================================================

	public bool HayPacienteSeleccionado => SelectedPaciente is not null;
	
	
	// ================================================================
	// INFRASTRUCTURE
	// ================================================================
	
	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	public event PropertyChangedEventHandler? PropertyChanged;
}



// ================================================================
// MINIVIEWMODELS
// ================================================================

public record TurnoDeHoyVM(string Hora, string PacienteNombreApellido, string PacienteEdad, string FueAtendido);

public record AtencionPreviaVM(string FechaHora, string MedicoNombreApellido, string Observaciones);