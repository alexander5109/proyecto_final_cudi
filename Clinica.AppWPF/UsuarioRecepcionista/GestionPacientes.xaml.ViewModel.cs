using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public sealed class GestionPacientesVM : INotifyPropertyChanged {

	// ================================================================
	// CONSTRUCTOR
	// ================================================================
	public GestionPacientesVM() {
		PacientesView = CollectionViewSource.GetDefaultView(_todosLosPacientes);
		PacientesView.Filter = FilterPacientes;
	}



	// ================================================================
	// COLECCIONES
	// ================================================================

	private List<PacienteDbModel> _todosLosPacientes = [];
	public ICollectionView PacientesView { get; private set; }



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
	// METODOS DE UI
	// ================================================================
	internal async Task RefrescarPacientesAsync() {
		var pacientes = await App.Repositorio.SelectPacientes();
		_todosLosPacientes = pacientes;

		// Reasignamos la vista para reflejar la nueva lista
		PacientesView = CollectionViewSource.GetDefaultView(_todosLosPacientes);
		PacientesView.Filter = FilterPacientes;

		OnPropertyChanged(nameof(PacientesView));
		SelectedPaciente = null;

		AplicarFiltros();
	}



	private void AplicarFiltros() {
		PacientesView.Refresh();

		if (SelectedPaciente != null &&
			!PacientesView.Cast<PacienteDbModel>().Contains(SelectedPaciente)) {
			SelectedPaciente = null;
		}
	}



	// ================================================================
	// FILTROS
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

	private bool FilterPacientes(object obj) {
		if (obj is not PacienteDbModel p)
			return false;

		if (string.IsNullOrWhiteSpace(FiltroPacientesTexto))
			return true;

		string texto = FiltroPacientesTexto.Trim();

		return
			(p.Nombre?.Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
			(p.Apellido?.Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
			(p.Dni?.Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false);
	}

	// ================================================================
	// REGLAS
	// ================================================================

	public string? SelectedPacienteDomicilioCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Localidad}, {SelectedPaciente?.Domicilio}";
	public string? SelectedPacienteNombreCompleto => SelectedPaciente is null ? null : $"{SelectedPaciente?.Nombre} {SelectedPaciente?.Apellido}";
	public bool HayPacienteSeleccionado => SelectedPaciente is not null;



	// ================================================================
	// INFRASTRUCTURE
	// ================================================================(propertyName));
	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));
}