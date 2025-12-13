using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.Shared.ApiDtos.HorarioDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;




public class HorarioViewModel(HorarioDto model) : INotifyPropertyChanged {
	//public int Id { get; } = model.Id.Valor;
	public int MedicoId { get; } = model.MedicoId.Valor;

	public DayOfWeek DiaSemana { get; } = model.DiaSemana;
	public string DiaSemanaDescripcion => DiaSemana.ATexto();

	public TimeSpan HoraDesde { get; } = model.HoraDesde;
	public string HoraDesdeStr => HoraDesde.ToString(@"hh\:mm");

	public TimeSpan HoraHasta { get; } = model.HoraHasta;
	public string HoraHastaStr => HoraHasta.ToString(@"hh\:mm");

	public DateTime VigenteDesde { get; } = model.VigenteDesde;
	public DateTime? VigenteHasta { get; } = model.VigenteHasta;

	public event PropertyChangedEventHandler? PropertyChanged;
}




public sealed class AdminMedicosViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;


	//public ICommand EditarHorarioCommand { get; }
	//public ICommand EliminarHorarioCommand { get; }

	public ObservableCollection<HorarioViewModel> HorariosViewModelList { get; } = [];

	public async Task CargarHorariosAsync(int medicoId) {
		//Los medicoDbModel ya traen HorarioDbModel
		//var response = await App.Repositorio.SelectHorariosWhereMedicoId(new MedicoId(medicoId));
		//HorariosViewModelList.Clear();
		//foreach (var h in response)
		//	HorariosViewModelList.Add(new HorarioViewModel(h));
	}

	private void CargarHorariosDeMedicoSeleccionado() {
		HorariosViewModelList.Clear();
		if (SelectedMedico is null) return;

		if (SelectedMedico.Horarios.Count == 0) {
			MessageBox.Show($"{SelectedMedico.Nombre} no tiene ningun horario cargado");
		} else {
			foreach (var h in SelectedMedico.Horarios)
				HorariosViewModelList.Add(new HorarioViewModel(h));
		}


	}


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

				CargarHorariosDeMedicoSeleccionado();
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
		set { _medicos = value; 
			OnPropertyChanged(nameof(MedicosList));
			OnPropertyChanged(nameof(HorariosViewModelList));
		}
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

