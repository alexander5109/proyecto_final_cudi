using System.ComponentModel;
using System.Runtime.CompilerServices;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
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

	//public ObservableCollection<ViewModelHorarioAgrupado> HorariosAgrupados { get; } //should it be associated to the selected medicoviewmodel or to the window viewmodel itself

	public AdminMedicosModificarViewModel(MedicoDbModel medicoDbModel, IEnumerable<EspecialidadCodigo> especialidades) {
		Id = medicoDbModel.Id;
		_nombre = medicoDbModel.Nombre;
		_apellido = medicoDbModel.Apellido;
		_dni = medicoDbModel.Dni;
		_telefono = medicoDbModel.Telefono;
		_provincia = medicoDbModel.ProvinciaCodigo;
		_domicilio = medicoDbModel.Domicilio;
		_localidad = medicoDbModel.Localidad;
		_especialidad = medicoDbModel.EspecialidadCodigo;
		_fechaIngreso = medicoDbModel.FechaIngreso;
		_haceGuardias = medicoDbModel.HaceGuardias;
		EspecialidadesDisponibles = especialidades;
	}


	public event PropertyChangedEventHandler? PropertyChanged;
	void OnPropertyChanged([CallerMemberName] string? name = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}