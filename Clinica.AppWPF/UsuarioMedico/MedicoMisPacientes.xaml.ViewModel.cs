using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioMedico;

public sealed class MedicoMisPacientesViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;


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
			}
		}
	}

	private string _filtroPacientesTexto = string.Empty;
	public string FiltroPacientesTexto {
		get => _filtroPacientesTexto;
		set {
			if (_filtroPacientesTexto != value) {
				_filtroPacientesTexto = value;
				OnPropertyChanged(nameof(FiltroPacientesTexto));
				AplicarFiltros(); // cada vez que cambia el texto, aplicamos el filtro
			}
		}
	}

	public ObservableCollection<PacienteDbModel> PacientesList { get; } = [];


	public bool ModificarPacienteCommand => SelectedPaciente is not null;



	private List<PacienteDbModel> _todosLosPacientes = []; // copia completa para filtrar
	internal async Task RefrescarPacientesAsync() {
		try {
			List<PacienteDbModel> pacientes = await App.Repositorio.Pacientes.SelectPacientes();
			_todosLosPacientes = pacientes;
		} catch (Exception ex) {
			MessageBox.Show("Error cargando pacientes: " + ex.Message);
		}
		AplicarFiltros();
	}



	private void AplicarFiltros() {
		PacientesList.Clear();

		IEnumerable<PacienteDbModel> origen;

		if (string.IsNullOrWhiteSpace(FiltroPacientesTexto)) {
			origen = _todosLosPacientes;
		} else {
			var texto = FiltroPacientesTexto.Trim();

			origen = _todosLosPacientes.Where(x =>
					(x.Nombre?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
					(x.Apellido?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
					(x.Dni?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false)
			);
		}

		foreach (PacienteDbModel instance in origen)
			PacientesList.Add(instance);
	}


	public string? SelectedPacienteDomicilioCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Localidad}, {SelectedPaciente?.Domicilio}";
	public string? SelectedPacienteNombreCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Nombre} {SelectedPaciente?.Apellido}";

	public bool HayPacienteSeleccionado => SelectedPaciente is not null;

	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
}
