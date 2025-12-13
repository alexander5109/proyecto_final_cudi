using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public sealed class SecretariaPacientesViewModel : INotifyPropertyChanged {

	// ================================================================
	// COLECCIONES
	// ================================================================

	private List<PacienteDbModel> _todosLosPacientes = [];  // Original immutable list
	public ObservableCollection<PacienteDbModel> PacientesList { get; } = [];

	// ================================================================
	// METODOS
	// ================================================================
	internal async Task RefrescarPacientesAsync() {
		try {
			List<PacienteDbModel> pacientes = await App.Repositorio.SelectPacientes();
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




	// ================================================================
	// ITEM SELECCIONADO
	// ================================================================

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



	// ================================================================
	// FILTER: PACIENTE (search in PacienteDisplayear)
	// ================================================================

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


	// ================================================================
	// REGLAS
	// ================================================================

	public string? SelectedPacienteDomicilioCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Localidad}, {SelectedPaciente?.Domicilio}";
	public string? SelectedPacienteNombreCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Nombre} {SelectedPaciente?.Apellido}";
	public bool HayPacienteSeleccionado => SelectedPaciente is not null;



	// ================================================================
	// UTILS
	// ================================================================(propertyName));
	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));
}


// ================================================================
// VIEWMODELS PARA SUB-COLLECTIONS
// ================================================================