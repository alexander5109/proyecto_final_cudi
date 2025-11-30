using System.Collections.ObjectModel;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clinica.AppWPF.ViewModels;


public partial class WindowModificarMedicoViewModel : ObservableObject {

	[ObservableProperty] private ObservableCollection<HorarioMedicoViewModel> horarios = [];

	public ObservableCollection<ViewModelHorariosAgrupados> HorariosAgrupados {
		get {
			ObservableCollection<ViewModelHorariosAgrupados> lista = [];

			// _ValidarRepositorios los 7 días vacíos
			foreach (DiaDeSemanaViewModel dia in DiaDeSemanaViewModel.Los7DiasDeLaSemana) {
				lista.Add(new ViewModelHorariosAgrupados(dia));
			}

			// Mapear horarios existentes
			foreach (HorarioMedicoViewModel horario in Horarios) {
				ViewModelHorariosAgrupados grupo = lista.First(g => g.DiaSemana == horario.DiaSemana);
				grupo.Horarios.Add(horario);
			}

			return lista;
		}
	}



	[ObservableProperty] private int? id;
	[ObservableProperty] private string name = string.Empty;
	[ObservableProperty] private string lastName = string.Empty;
	[ObservableProperty] private string telefono = string.Empty;
	[ObservableProperty] private string dni = string.Empty;
	[ObservableProperty] private string localidad = string.Empty;
	[ObservableProperty] private int? especialidadCodigoInterno = default;
	//[ObservableProperty] private string especialidadCodigoInterno = string.Empty;
	[ObservableProperty] private double? sueldoMinimoGarantizado;
	[ObservableProperty] private string provincia = string.Empty;
	[ObservableProperty] private string domicilio = string.Empty;
	[ObservableProperty] private DateTime? fechaIngreso = DateTime.Today;
	[ObservableProperty] private bool? guardia = false;



	//public List<string> EspecialidadesDisponibles { get; } = EspecialidadMedica2025.EspecialidadesDisponibles;
	public string Displayear => $"{Id}: {EspecialidadCodigoInterno} - {Name} {LastName}";



	//public ViewModelEspecialidadMedica EspecialidadRelacionada {
	//	get {
	//		try {
	//			if (EspecialidadCodigo2025 is null)
	//				throw new Exception("El ID del paciente es nulo.");
	//			return EspecialidadMedica2025.CrearPorCodigoInterno(EspecialidadCodigo2025);
	//			return App.BaseDeDatos.DictEspecialidades[(int)EspecialidadCodigo2025];
	//		} catch (Exception ex) {
	//			MessageBox.Show(
	//				$"Error al obtener el paciente con ID '{EspecialidadCodigo2025}':\n{ex.Message}",
	//				"Error de acceso a datos",
	//				MessageBoxButton.OK,
	//				MessageBoxImage.Error
	//			);
	//			return PacienteViewModel.NewEmpty();
	//		}
	//	}
	//}


	public static WindowModificarMedicoViewModel NewEmpty() => new(
		[],
		default,   // id
		string.Empty,   // nombre
		string.Empty,   // apellido
		string.Empty,   // dni
		string.Empty,   // provinciaCodigo
		string.Empty,   // domicilio
		string.Empty,   // localidad
		default,   // especialidadCodigoInterno
		string.Empty,   // telefono
		false,          // guardia
		DateTime.Today, // fechaIngreso
		default              // sueldoMinimoGarantizado
	);




	public WindowModificarMedicoViewModel(
		ObservableCollection<HorarioMedicoViewModel> horarios,
		int? id,
		string? name,
		string? lastName,
		string? dni,
		string? provincia,
		string? domicilio,
		string? localidad,
		int? especialidadCodigoInterno,
		string? telefono,
		bool? guardia,
		DateTime? fechaIngreso,
		double? sueldoMinimoGarantizado
	) {
		Horarios = horarios;
		Id = id;
		Name = name ?? string.Empty;
		LastName = lastName ?? string.Empty;
		Dni = dni ?? string.Empty;
		Provincia = provincia ?? string.Empty;
		Domicilio = domicilio ?? string.Empty;
		Localidad = localidad ?? string.Empty;
		EspecialidadCodigoInterno = especialidadCodigoInterno;
		Telefono = telefono ?? string.Empty;
		Guardia = guardia ?? false;
		FechaIngreso = fechaIngreso ?? DateTime.Today;
		SueldoMinimoGarantizado = sueldoMinimoGarantizado;

		//Horarios.CollectionChanged += (_, __) => OnPropertyChanged(nameof(HorariosAgrupados));
	}



	public Result<Medico2025> ToDomain() {
		throw new NotImplementedException("Implementar Medico2025 ToDomain");
		//return Medico2025.Crear(
		//	NombreCompleto2025.Crear(this.Name, this.LastName),
		//	EspecialidadMedica2025.CrearPorCodigoInterno(
		//		this.EspecialidadCodigo2025
		//	//EspecialidadMedica2025.RamasDisponibles.First()
		//	),
		//	DniArgentino2025.Crear(this.Dni),
		//	DomicilioArgentino2025.Crear(
		//	LocalidadDeProvincia2025.Crear(this.Localidad, ProvinciaArgentina2025.Crear(this.Provincia)),
		//	this.Domicilio
		//),
		//	ContactoTelefono2025.Crear(this.Telefono),
		//	ListaHorarioMedicos2025.Crear(
		//	this.Horarios.Select(h =>
		//	),
		//	FechaRegistro2025.Crear(this.FechaIngreso),
		//	this.Guardia ?? false
		//);
	}





}