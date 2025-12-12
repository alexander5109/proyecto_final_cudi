using System.ComponentModel;
using System.Windows;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public sealed class SecretariaPacientesViewModel : INotifyPropertyChanged {
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
				FiltrarPacientes(); // cada vez que cambia el texto, aplicamos el filtro
			}
		}
	}

	private List<PacienteDbModel> _pacientes = [];
	public List<PacienteDbModel> PacientesList {
		get => _pacientes;
		set { _pacientes = value; OnPropertyChanged(nameof(PacientesList)); }
	}
	public bool ModificarPacienteCommand => SelectedPaciente is not null;



	private List<PacienteDbModel> _todosLosPacientes = []; // copia completa para filtrar
	internal async Task RefrescarPacientesAsync() {
		try {
			List<PacienteDbModel> pacientes = await App.Repositorio.SelectPacientes();
			_todosLosPacientes = pacientes;
		} catch (Exception ex) {
			MessageBox.Show("Error cargando pacientes: " + ex.Message);
		}
		FiltrarPacientes();
	}


	private void FiltrarPacientes() {
		if (string.IsNullOrWhiteSpace(FiltroPacientesTexto)) {
			PacientesList = [.. _todosLosPacientes];
		} else {
			var texto = FiltroPacientesTexto.Trim().ToLower();
			PacientesList = [.. _todosLosPacientes
				.Where(p =>
					(p.Nombre?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
					(p.Apellido?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
					(p.Dni?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false)
				)];
		}
	}


	public string? SelectedPacienteDomicilioCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Localidad}, {SelectedPaciente?.Domicilio}";
	public string? SelectedPacienteNombreCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Nombre} {SelectedPaciente?.Apellido}";

	public bool HayPacienteSeleccionado => SelectedPaciente is not null;

	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
}
