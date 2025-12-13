using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.ApiDtos.HorarioDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public class AdminMedicosModificarViewModel : INotifyPropertyChanged {
	public MedicoId Id { get; }

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

	private EspecialidadCodigo _especialidad;
	public EspecialidadCodigo Especialidad { get => _especialidad; set { _especialidad = value; OnPropertyChanged(); } }

	public IEnumerable<EspecialidadCodigo> EspecialidadesDisponibles { get; init; }

	private DateTime _fechaIngreso;
	public DateTime FechaIngreso { get => _fechaIngreso; set { _fechaIngreso = value; OnPropertyChanged(); } }

	private bool _haceGuardias;
	public bool HaceGuardias { get => _haceGuardias; set { _haceGuardias = value; OnPropertyChanged(); } }

	public ObservableCollection<ViewModelHorarioAgrupado> HorariosAgrupados { get; }

	public AdminMedicosModificarViewModel(MedicoDbModel model, IEnumerable<EspecialidadCodigo> especialidades) {
		Id = model.Id;
		_nombre = model.Nombre;
		_apellido = model.Apellido;
		_dni = model.Dni;
		_telefono = model.Telefono;
		_provincia = model.ProvinciaCodigo;
		_domicilio = model.Domicilio;
		_localidad = model.Localidad;
		_especialidad = model.EspecialidadCodigo;
		_fechaIngreso = model.FechaIngreso;
		_haceGuardias = model.HaceGuardias;
		EspecialidadesDisponibles = especialidades;

		HorariosAgrupados = LoadHorarios(model.Horarios);
	}

	private static ObservableCollection<ViewModelHorarioAgrupado> LoadHorarios(IReadOnlyList<HorarioDto> horarios) {
		//if (string.IsNullOrWhiteSpace(json))
		//	return [];

        //List<HorarioDb> horarios = JsonSerializer.Deserialize<List<HorarioDb>>(json)!;

		var agrupado = horarios
			.GroupBy(h => h.DiaSemana)
			.Select(g => new ViewModelHorarioAgrupado(g.Key, [.. g]))
			.ToList();

		return new ObservableCollection<ViewModelHorarioAgrupado>(agrupado);
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	void OnPropertyChanged([CallerMemberName] string? name = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}