using System.ComponentModel;
using System.Windows;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public sealed class AdminMedicosViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;


	private MedicoDbModel? _selectedMedico;
	public MedicoDbModel? SelectedMedico {
		get => _selectedMedico;
		set {
			if (_selectedMedico != value) {
				_selectedMedico = value;
				OnPropertyChanged(nameof(SelectedMedico));
				OnPropertyChanged(nameof(HayMedicoSeleccionado));
				OnPropertyChanged(nameof(SelectedMedicoDomicilioCompleto));
				OnPropertyChanged(nameof(SelectedMedicoNombreCompleto));
			}
		}
	}

	private string _filtroMedicosTexto = string.Empty;
	public string FiltroMedicosTexto {
		get => _filtroMedicosTexto;
		set {
			if (_filtroMedicosTexto != value) {
				_filtroMedicosTexto = value;
				OnPropertyChanged(nameof(FiltroMedicosTexto));
				FiltrarMedicos(); // cada vez que cambia el texto, aplicamos el filtro
			}
		}
	}

	private List<MedicoDbModel> _medicos = [];
	public List<MedicoDbModel> MedicosList {
		get => _medicos;
		set { _medicos = value; OnPropertyChanged(nameof(MedicosList)); }
	}
	public bool ModificarMedicoCommand => SelectedMedico is not null;



	private List<MedicoDbModel> _todosLosMedicos = []; // copia completa para filtrar
	internal async Task RefrescarMedicosAsync() {
		try {
			List<MedicoDbModel> medicos = await App.Repositorio.SelectMedicos();
			_todosLosMedicos = medicos;
		} catch (Exception ex) {
			MessageBox.Show("Error cargando medicos: " + ex.Message);
		}
		FiltrarMedicos();
	}


	private void FiltrarMedicos() {
		if (string.IsNullOrWhiteSpace(FiltroMedicosTexto)) {
			MedicosList = [.. _todosLosMedicos];
		} else {
			var texto = FiltroMedicosTexto.Trim().ToLower();
			MedicosList = [.. _todosLosMedicos
				.Where(p =>
					(p.Nombre?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
					(p.Apellido?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
					(p.EspecialidadCodigo.ToString().Contains(texto, StringComparison.CurrentCultureIgnoreCase))
				)];
		}
	}


	public string? SelectedMedicoDomicilioCompleto => SelectedMedico is null ? null : $"{SelectedMedico?.Localidad}, {SelectedMedico?.Domicilio}";
	public string? SelectedMedicoNombreCompleto => SelectedMedico is null ? null : $"{SelectedMedico?.Nombre} {SelectedMedico?.Apellido}";

	public bool HayMedicoSeleccionado => SelectedMedico is not null;

	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
}
