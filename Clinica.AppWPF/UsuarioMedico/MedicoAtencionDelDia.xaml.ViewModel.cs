using System.Collections.ObjectModel;
using System.ComponentModel;
using Clinica.AppWPF.Infrastructure.IRepositorios;
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
				OnPropertyChanged(nameof(SelectedPacienteDomicilioCompleto));
				OnPropertyChanged(nameof(SelectedPacienteNombreCompleto));
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
	public ObservableCollection<TurnoDbModel> TurnosList { get; } = [];




	internal async Task RefrescarMisTurnosAsync() {
		List<TurnoDbModel> turnos = await App.Repositorio.Turnos.SelectTurnos();

		_todosLosTurnos = turnos;
	}


	public string? SelectedPacienteDomicilioCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Localidad}, {SelectedPaciente?.Domicilio}";
	public string? SelectedPacienteNombreCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Nombre} {SelectedPaciente?.Apellido}";

	public bool HayPacienteSeleccionado => SelectedPaciente is not null;

	private async Task CargarAtencionesDePacienteSeleccionado() {
		AtencionesViewModelList.Clear();

		if (SelectedPaciente is null) {
			return;
		}
		IReadOnlyList<AtencionDbModel>? Atenciones = await App.Repositorio.Atenciones.SelectAtencionesWherePacienteId(SelectedPaciente.Id);

		if (Atenciones is null || Atenciones.Count == 0)
			return;

		foreach (AtencionDbModel h in Atenciones) {
			AtencionesViewModelList.Add(
				new AtencionPreviaVM(
					Hora: "",
					PacienteNombreApellido: "",
					PacienteEdad: "",
					FueAtendido: ""
				)
			);
		}
	}


	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}


public record TurnoDeHoyVM(string Hora, string PacienteNombreApellido, string PacienteEdad, string FueAtendido);

public record AtencionPreviaVM(string Hora, string PacienteNombreApellido, string PacienteEdad, string FueAtendido);