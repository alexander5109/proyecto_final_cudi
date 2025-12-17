using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.AppWPF.UsuarioMedico;





public sealed class MedicoAtencionDelDiaVM(MedicoId2025 CurrentMedicoId) : INotifyPropertyChanged {
	// ==========================================================
	// BOTONES: SELECTED ITEMS
	// ==========================================================.




	private DateTime _selectedFecha = DateTime.Now;
	public DateTime SelectedFecha {
		get => _selectedFecha;
		set {
			if (_selectedFecha == value) return;
			_selectedFecha = value;
			OnPropertyChanged(nameof(SelectedFecha));
		}
	}


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


	private TurnoDeHoyVM? _selectedTurno;
	public TurnoDeHoyVM? SelectedTurno {
		get => _selectedTurno;
		set {
			if (_selectedTurno != value) {
				_selectedTurno = value;
				OnPropertyChanged(nameof(SelectedTurno));
				ActualizarPacienteDesdeTurno();
			}
		}
	}
	private string? _diagnosticoText;
	public string? Observaciones {
		get => _diagnosticoText;
		set {
			if (_diagnosticoText != value) {
				_diagnosticoText = value;
				OnPropertyChanged(nameof(Observaciones));
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
		if (SelectedTurno == null) {
			SelectedPaciente = null;
			return;
		}

		SelectedPaciente = App.Repositorio.Pacientes.GetFromCachePacienteWhereId(SelectedTurno.Model.PacienteId);
		OnPropertyChanged(nameof(PuedeDiagnosticar));
	}


	private string CalcularEdad(DateTime fechaNacimiento) =>
		(DateTime.Today.Year - fechaNacimiento.Year - (DateTime.Today < fechaNacimiento.AddYears(DateTime.Today.Year - fechaNacimiento.Year) ? 1 : 0)).ToString();

	// ================================================================
	// METHODS.ASYNC
	// ================================================================

	public async Task ConfirmarDiagnosticoAsync() {
		MessageBox.Show($"|SelectedPaciente?.Nombre: {SelectedPaciente?.Nombre} \n| SelectedTurno?.Hora: {SelectedTurno?.Model.FechaHoraAsignadaDesde}| SelectedTurno?.EsperandoAtencion: {SelectedTurno?.EsperandoAtencion}| ObservacionActual: {Observaciones}|");
		if (SelectedPaciente == null || SelectedTurno == null || string.IsNullOrWhiteSpace(Observaciones)) {
			return;
		}
		// Solo permitir si el turno está concretado
		if (!SelectedTurno.EsperandoAtencion) return;

		ResultWpf<UnitWpf> result = await App.Repositorio.Atenciones.AgendarAtencionConDiagnostico(new AtencionDto(
			SelectedTurno.Model.Id.Valor,
			SelectedPaciente.Id.Valor,
			CurrentMedicoId.Valor,
			Observaciones
			)
		);
		result.MatchAndDo(
			async caseok => {
				Observaciones = null;
				MessageBox.Show("Llegamos a result.isok?");
				await CargarAtencionesDePacienteSeleccionado();
				await RefrescarMisTurnosAsync();

			},
			casError => { casError.ShowMessageBox(); }

			);
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


		foreach (TurnoDbModel turnoDbModel in turnos.OrderBy(t => t.FechaHoraAsignadaDesde)) {
			PacienteDbModel? paciente = App.Repositorio.Pacientes.GetFromCachePacienteWhereId(turnoDbModel.PacienteId);
			if (paciente == null) {
				MessageBox.Show("Por que este paciente id es null?");
				continue;
			}

			bool tieneAtencion = (await App.Repositorio.Atenciones.SelectAtencionesWherePacienteId(paciente.Id))
									.Any(a => a.TurnoId == turnoDbModel.Id);

			TurnosList.Add(new TurnoDeHoyVM(
				Model: turnoDbModel,
				PacienteNombreApellido: $"{paciente.Nombre} {paciente.Apellido}",
				PacienteEdad: CalcularEdad(paciente.FechaNacimiento),
				EsperandoAtencion: turnoDbModel.OutcomeEstado == TurnoEstadoEnum.Concretado,
				FueAtendido: tieneAtencion ? "✔" : ""
			));
		}
	}


	private async Task CargarAtencionesDePacienteSeleccionado() {
		AtencionesViewModelList.Clear();

		if (SelectedPaciente is null) return;

		List<AtencionDbModel>? atenciones = await App.Repositorio.Atenciones.SelectAtencionesWherePacienteId(SelectedPaciente.Id);

		MessageBox.Show($"Atenciones de pacienteconid [ID: {SelectedPaciente.Id.Valor}]: [Count: {atenciones.Count}]");
		if (atenciones is null || atenciones.Count == 0) return;


		foreach (AtencionDbModel atencion in atenciones.OrderByDescending(x => x.Fecha)) {
			//TurnoDbModel? turno = _todosLosTurnos.FirstOrDefault(t => t.Id == atencion.TurnoId);
			//string horaStr = turno?.FechaHoraAsignadaDesde.ToString("HH:mm") ?? "";


			string selectedMedicoNombreCompleto = App.Repositorio.Medicos.GetFromCacheMedicoDisplayWhereId(atencion.MedicoId);

			AtencionesViewModelList.Add(new AtencionPreviaVM(
				Fecha: atencion.Fecha.ToString("yyyy-MM-dd HH:mm"),
				MedicoNombreApellido: selectedMedicoNombreCompleto!,
				Observaciones: atencion.Observaciones
			));
		}
	}


	// ================================================================
	// REGLAS
	// ================================================================
	public bool PuedeDiagnosticar => SelectedTurno != null && SelectedTurno.EsperandoAtencion;

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

public record TurnoDeHoyVM(
	TurnoDbModel Model,
	//TurnoId2025 Id,
	//PacienteId2025 PacienteId,
	//string Hora,
	string PacienteNombreApellido,
	string PacienteEdad,
	bool EsperandoAtencion,
	string FueAtendido
);

public record AtencionPreviaVM(
	string Fecha, 
	string MedicoNombreApellido, 
	string Observaciones
);