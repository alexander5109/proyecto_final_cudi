using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.AppWPF.CommonViewModels.CommonEnumsToViewModel;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public class AdminMedicosModificarViewModel : INotifyPropertyChanged {
	public MedicoId? Id { get; }

	private string _nombre;
	public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(); } }

	private string _apellido;
	public string Apellido { get => _apellido; set { _apellido = value; OnPropertyChanged(); } }

	private string _dni;
	public string Dni { get => _dni; set { _dni = value; OnPropertyChanged(); } }

	private string _telefono;
	public string Telefono { get => _telefono; set { _telefono = value; OnPropertyChanged(); } }

	private ProvinciaCodigo _provincia;
	public ProvinciaCodigo Provincia { get => _provincia; set { _provincia = value; OnPropertyChanged(); } }

	private string _domicilio;
	public string Domicilio { get => _domicilio; set { _domicilio = value; OnPropertyChanged(); } }

	private string _localidad;
	public string Localidad { get => _localidad; set { _localidad = value; OnPropertyChanged(); } }

	//private EspecialidadCodigo _especialidad;
	//public EspecialidadCodigo Especialidad { get => _especialidad; set { _especialidad = value; OnPropertyChanged(); } }


	private EspecialidadViewModel? _selectedEspecialidad;
	public EspecialidadViewModel? SelectedEspecialidad {
		get => _selectedEspecialidad;
		set {
			if (_selectedEspecialidad == value) return;
			_selectedEspecialidad = value;

			OnPropertyChanged(nameof(SelectedEspecialidad));
		}
	}



	public ObservableCollection<EspecialidadViewModel> EspecialidadesDisponiblesItemsSource { get; } = [];

	private DateTime _fechaIngreso = DateTime.Today;

	public DateTime FechaIngreso {
		get => _fechaIngreso;
		set {
			_fechaIngreso = value;
			OnPropertyChanged(nameof(FechaIngreso));
		}
	}

	private bool _haceGuardias;
	public bool HaceGuardias { get => _haceGuardias; set { _haceGuardias = value; OnPropertyChanged(); } }

	public ObservableCollection<ViewModelHorarioAgrupado> HorariosAgrupados { get; } = [];

	public AdminMedicosModificarViewModel(MedicoDbModel medicoDbModel) {
		Id = medicoDbModel.Id;
		_nombre = medicoDbModel.Nombre;
		_apellido = medicoDbModel.Apellido;
		_dni = medicoDbModel.Dni;
		_telefono = medicoDbModel.Telefono;
		_provincia = medicoDbModel.ProvinciaCodigo;
		_domicilio = medicoDbModel.Domicilio;
		_localidad = medicoDbModel.Localidad;
		_fechaIngreso = medicoDbModel.FechaIngreso;
		_haceGuardias = medicoDbModel.HaceGuardias;


		EspecialidadesDisponiblesItemsSource.Clear();
		foreach (EspecialidadViewModel? esp in Especialidad2025.Todas.Select(x => new EspecialidadViewModel(x)))
			EspecialidadesDisponiblesItemsSource.Add(esp);





		Especialidad2025.CrearResult(medicoDbModel.EspecialidadCodigo)
			.MatchAndDo(
				ok => {
					SelectedEspecialidad = new EspecialidadViewModel(ok);
				},
				err => {
					//MessageBox.Show($"El código de especialidad no existe <{(byte)medicoDbModel.EspecialidadCodigo}>");
				});

	}


	public event PropertyChangedEventHandler? PropertyChanged;
	void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


	public async Task CargarHorariosAsync() {
		HorariosAgrupados.Clear();

		if (Id is not MedicoId idGood) {
			MessageBox.Show("why is medicoid null?");
			return;
		}
		IReadOnlyList<HorarioDbModel>? horarios = await App.Repositorio.SelectHorariosWhereMedicoId(idGood);
		if (horarios == null) {
			MessageBox.Show("why is horarios null?");
			return;
		}


		IOrderedEnumerable<IGrouping<DayOfWeek, HorarioDbModel>> grupos = horarios
			.GroupBy(h => h.DiaSemana)
			.OrderBy(g => g.Key);


		foreach (IGrouping<DayOfWeek, HorarioDbModel>? grupo in grupos) {
			HorariosAgrupados.Add(
				new ViewModelHorarioAgrupado(
					grupo.Key,
					[.. grupo]
				)
			);
		}



		bool test = horarios.Count > 0;
		bool test2 = grupos.Count() > 0;
		bool test3 = HorariosAgrupados.Count > 0;
		//bool test4 = TreeView.Items.Count > 0;
	}
}
