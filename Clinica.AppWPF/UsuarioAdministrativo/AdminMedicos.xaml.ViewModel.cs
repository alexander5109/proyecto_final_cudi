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

	public ObservableCollection<HorarioViewModel> HorariosViewModelList { get; } = new ObservableCollection<HorarioViewModel>();

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

				// Cargar horarios directamente desde SelectedMedico.Horarios
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
				FiltrarMedicos(); // Aplica el filtro cada vez que cambia el texto
			}
		}
	}

	private List<MedicoDbModel> _medicos = new List<MedicoDbModel>();
	public List<MedicoDbModel> MedicosList {
		get => _medicos;
		set {
			_medicos = value;
			OnPropertyChanged(nameof(MedicosList));
		}
	}

	public bool ModificarMedicoCommand => SelectedMedico is not null;

	private List<MedicoDbModel> _todosLosMedicos = new List<MedicoDbModel>(); // Copia completa para filtrar
	internal async Task RefrescarMedicosAsync() {
		List<MedicoDbModel> medicos = await App.Repositorio.SelectMedicos();
		//foreach (MedicoDbModel medico in medicos) {
			//string horariosTexto =
			//	medico.Horarios.Count == 0
			//		? "SIN HORARIOS"
			//		: string.Join(
			//			"\n",
			//			medico.Horarios.Select(h =>
			//				$"{h.DiaSemana.ATexto()} " +
			//				$"{h.HoraDesde:hh\\:mm}–{h.HoraHasta:hh\\:mm} " +
			//				$"({h.VigenteDesde:dd/MM/yyyy} → " // +
			//				//$"{(h.VigenteHasta.HasValue ? h.VigenteHasta.Value.ToString("dd/MM/yyyy") : "Indefinido")})"
			//			)
			//		);

			//MessageBox.Show(
			//	$"Cargando médico: {medico.Nombre}\n\nHorarios:\n{horariosTexto}",
			//	"Debug horarios"
			//);
		//}

		_todosLosMedicos = medicos;
		FiltrarMedicos();
	}

	private void FiltrarMedicos() {
		if (string.IsNullOrWhiteSpace(FiltroMedicosTexto)) {
			MedicosList = new List<MedicoDbModel>(_todosLosMedicos);
		} else {
			var texto = FiltroMedicosTexto.Trim().ToLower();
			MedicosList = _todosLosMedicos.Where(p =>
					(p.Nombre?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
					(p.Apellido?.ToLower().Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
					(p.EspecialidadCodigo.ToString().Contains(texto, StringComparison.CurrentCultureIgnoreCase)))
				.ToList();
		}
	}

	public string? SelectedMedicoDomicilioCompleto => SelectedMedico is null ? null : $"{SelectedMedico?.Localidad}, {SelectedMedico?.Domicilio}";
	public string? SelectedMedicoNombreCompleto => SelectedMedico is null ? null : $"{SelectedMedico?.Nombre} {SelectedMedico?.Apellido}";

	public bool HayMedicoSeleccionado => SelectedMedico is not null;

	private void CargarHorariosDeMedicoSeleccionado() {
		HorariosViewModelList.Clear();

		//if (SelectedMedico?.Horarios.Count == 0) {
		//	MessageBox.Show($"{SelectedMedico?.Nombre} no tiene ningun horario cargado");
		//} else {
		//	foreach (var h in SelectedMedico.Horarios)
		//		HorariosViewModelList.Add(new HorarioViewModel(h)); // Aquí estamos creando un ViewModel para cada HorarioDto
		//}
	}

	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
