using System.Collections.ObjectModel;
using System.ComponentModel;
using Clinica.AppWPF.Infrastructure.IRepositorios;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioMedico;





public sealed class MedicoAtencionDelDiaVM : INotifyPropertyChanged {



	// ================================================================
	// CONSTRUCTOIR
	// ================================================================


	private MedicoId CurrentMedicoId;




	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	public event PropertyChangedEventHandler? PropertyChanged;
	public ObservableCollection<AtencionPreviaVM> AtencionesViewModelList { get; } = [];

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

	private async void OnSelectedPacienteChanged() {
		await CargarAtencionesDePacienteSeleccionado();
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
	private List<TurnoDbModel> _todosLosTurnos = []; // Copia completa para filtrar
	public ObservableCollection<TurnoDeHoyVM> TurnosList { get; } = [];



	// ================================================================
	// METHODS
	// ================================================================

	internal async Task RefrescarMisTurnosAsync() {
		List<TurnoDbModel> turnos = await App.Repositorio.Turnos.SelectTurnosWhereMedicoId(CurrentMedicoId);
		await App.Repositorio.Pacientes.RefreshCache();
		await App.Repositorio.Medicos.RefreshCache();

		_todosLosTurnos = turnos;
		TurnosList.Clear();


		foreach (TurnoDbModel t in turnos.OrderBy(t => t.FechaHoraAsignadaDesde)) {
			//PacienteDbModel? paciente = await App.Repositorio.Pacientes.SelectPacienteWhereId(t.PacienteId); // cache de pacientes
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
				FechaHora: atencion.FechaHora.ToString("%f"), //i intend 2025-12-31 17:30 format here
				MedicoNombreApellido: selectedMedicoNombreCompleto!,
				Observaciones: atencion.Observaciones
			));
		}
	}

	private string CalcularEdad(DateTime fechaNacimiento) =>
		(DateTime.Today.Year - fechaNacimiento.Year - (DateTime.Today < fechaNacimiento.AddYears(DateTime.Today.Year - fechaNacimiento.Year) ? 1 : 0)).ToString();


	// ================================================================
	// METHODS
	// ================================================================

	public bool HayPacienteSeleccionado => SelectedPaciente is not null;

	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}


public record TurnoDeHoyVM(string Hora, string PacienteNombreApellido, string PacienteEdad, string FueAtendido);

public record AtencionPreviaVM(string FechaHora, string MedicoNombreApellido, string Observaciones);