using System.Collections.ObjectModel;
using System.ComponentModel;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioMedico;





public sealed class MedicoAtencionDelDiaViewModel : INotifyPropertyChanged {

	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	public event PropertyChangedEventHandler? PropertyChanged;
	public ObservableCollection<HorarioViewModel> HorariosViewModelList { get; } = [];

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
				OnSelectedMedicoChanged();
			}
		}
	}

	private async void OnSelectedMedicoChanged() {
		await CargarHorariosDeMedicoSeleccionado();
	}

	private string _filtroMedicosTexto = string.Empty;
	public string FiltroMedicosTexto {
		get => _filtroMedicosTexto;
		set {
			if (_filtroMedicosTexto != value) {
				_filtroMedicosTexto = value;
				OnPropertyChanged(nameof(FiltroMedicosTexto));
				FiltrarMedicos(); // Aplica el filtro cada vez que cambia el texto
			}
		}
	}


	// ================================================================
	// COLECCIONES
	// ================================================================
	private List<MedicoDbModel> _todosLosMedicos = []; // Copia completa para filtrar
	public ObservableCollection<MedicoDbModel> MedicosList { get; } = [];




	internal async Task RefrescarMedicosAsync() {
		List<MedicoDbModel> medicos = await App.Repositorio.Medicos.SelectMedicos();

		_todosLosMedicos = medicos;
		FiltrarMedicos();
	}

	private void FiltrarMedicos() {
		MedicosList.Clear();

		IEnumerable<MedicoDbModel> origen;

		if (string.IsNullOrWhiteSpace(FiltroMedicosTexto)) {
			origen = _todosLosMedicos;
		} else {
			var texto = FiltroMedicosTexto.Trim();

			origen = _todosLosMedicos.Where(m =>
				(m.Nombre?.Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
				(m.Apellido?.Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
				m.EspecialidadCodigo
					.ToString()
					.Contains(texto, StringComparison.CurrentCultureIgnoreCase)
			);
		}

		foreach (MedicoDbModel medico in origen)
			MedicosList.Add(medico);
	}


	public string? SelectedMedicoDomicilioCompleto => SelectedMedico is null ? null : $"{SelectedMedico?.Localidad}, {SelectedMedico?.Domicilio}";
	public string? SelectedMedicoNombreCompleto => SelectedMedico is null ? null : $"{SelectedMedico?.Nombre} {SelectedMedico?.Apellido}";

	public bool HayMedicoSeleccionado => SelectedMedico is not null;

	private async Task CargarHorariosDeMedicoSeleccionado() {
		HorariosViewModelList.Clear();

		if (SelectedMedico is null) {
			// MessageBox.Show("por que es null el selectmedico?"); // porque se actualizo el listview de medicos tras usarse un filtro!
			return;
		}
		IReadOnlyList<HorarioDbModel>? horarios =
			await App.Repositorio.Horarios.SelectHorariosWhereMedicoId(SelectedMedico.Id);

		if (horarios is null || horarios.Count == 0)
			return;

		foreach (HorarioDbModel h in horarios)
			HorariosViewModelList.Add(new HorarioViewModel(h));
	}


	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class HorarioViewModel(HorarioDbModel horarioDbModel) {
	public int MedicoId { get; } = horarioDbModel.MedicoId.Valor;
	public DayOfWeek DiaSemana { get; } = horarioDbModel.DiaSemana;
	public string DiaSemanaDescripcion => DiaSemana.ATexto();
	public TimeSpan HoraDesde { get; } = horarioDbModel.HoraDesde;
	public string HoraDesdeStr => HoraDesde.ToString(@"hh\:mm");
	public TimeSpan HoraHasta { get; } = horarioDbModel.HoraHasta;
	public string HoraHastaStr => HoraHasta.ToString(@"hh\:mm");
	public DateTime VigenteDesde { get; } = horarioDbModel.VigenteDesde;
	public DateTime? VigenteHasta { get; } = horarioDbModel.VigenteHasta ?? DateTime.MaxValue;
}
