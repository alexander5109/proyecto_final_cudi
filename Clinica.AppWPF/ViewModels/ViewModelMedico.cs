using System.Collections.ObjectModel;
using System.Windows;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Clinica.DataPersistencia;

namespace Clinica.AppWPF.ViewModels;



// View-model simple types (internos, auto-contenidos para demo)
public record ViewModelEspecialidadMedica(int CodigoInterno, string Titulo);
public record ViewModelDiaSemana(int Value, string NombreDia);
public record ViewModelDisponibilidadEspecialidad(DateTime Fecha, string Hora, string Medico);





public partial class ViewModelHorario : ObservableObject {
	[ObservableProperty] private DayOfWeek diaSemana;
	[ObservableProperty] private TimeOnly desde;
	[ObservableProperty] private TimeOnly hasta;
}

public partial class ViewModelHorariosAgrupados : ObservableObject {
	public DayOfWeek DiaSemana { get; }
	public string DiaSemanaNombre => DiaSemana.ATexto();
	[ObservableProperty] private ObservableCollection<ViewModelHorario> horarios = [];

	public ViewModelHorariosAgrupados(DayOfWeek dia) {
		DiaSemana = dia;
	}
}


public static class ModelDtoExtensions {
	public static ViewModelMedico ToViewModel(this MedicoDto medicoDto) {
		return new ViewModelMedico(
			new ObservableCollection<ViewModelHorario>(
				medicoDto.Horarios.Select(h => new ViewModelHorario {
					DiaSemana = Enum.Parse<DayOfWeek>(h.DiaSemana),
					Desde = TimeOnly.Parse(h.Desde),
					Hasta = TimeOnly.Parse(h.Hasta)
				})
			),
			medicoDto.Id,
			medicoDto.Name,
			medicoDto.LastName,
			medicoDto.Dni,
			medicoDto.Provincia,
			medicoDto.Domicilio,
			medicoDto.Localidad,
			medicoDto.EspecialidadCodigoInterno,
			medicoDto.Telefono,
			medicoDto.Guardia,
			medicoDto.FechaIngreso,
			medicoDto.SueldoMinimoGarantizado
		);
	}
}

public partial class ViewModelMedico : ObservableObject {



	[ObservableProperty]
	private ObservableCollection<ViewModelHorario> horarios = [];

	public ObservableCollection<ViewModelHorariosAgrupados> HorariosAgrupados {
		get {
			ObservableCollection<ViewModelHorariosAgrupados> lista = [];

			// Programar los 7 días vacíos
			foreach (DayOfWeek dia in DiaSemana2025.Los7EnumDias) {
				lista.Add(new ViewModelHorariosAgrupados(dia));
			}

			// Mapear horarios existentes
			foreach (ViewModelHorario horario in Horarios) {
				var grupo = lista.First(g => g.DiaSemana == horario.DiaSemana);
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
	[ObservableProperty] private double ?sueldoMinimoGarantizado;
	[ObservableProperty] private string provincia = string.Empty;
	[ObservableProperty] private string domicilio = string.Empty;
	[ObservableProperty] private DateTime? fechaIngreso = DateTime.Today;
	[ObservableProperty] private bool? guardia = false;



	//public List<string> EspecialidadesDisponibles { get; } = EspecialidadMedica2025.EspecialidadesDisponibles;
	[JsonIgnore] public string Displayear => $"{Id}: {EspecialidadCodigoInterno} - {Name} {LastName}";



	//public ViewModelEspecialidadMedica EspecialidadRelacionada {
	//	get {
	//		try {
	//			if (EspecialidadCodigoInterno is null)
	//				throw new Exception("El ID del paciente es nulo.");
	//			return EspecialidadMedica2025.CrearPorCodigoInterno(EspecialidadCodigoInterno);
	//			return App.BaseDeDatos.DictEspecialidades[(int)EspecialidadCodigoInterno];
	//		} catch (Exception ex) {
	//			MessageBox.Show(
	//				$"Error al obtener el paciente con ID '{EspecialidadCodigoInterno}':\n{ex.Message}",
	//				"Error de acceso a datos",
	//				MessageBoxButton.OK,
	//				MessageBoxImage.Error
	//			);
	//			return ViewModelPaciente.NewEmpty();
	//		}
	//	}
	//}


	public static ViewModelMedico NewEmpty() => new(
		[],
		default,   // id
		string.Empty,   // name
		string.Empty,   // lastName
		string.Empty,   // dni
		string.Empty,   // provincia
		string.Empty,   // domicilio
		string.Empty,   // localidad
		default,   // especialidadCodigoInterno
		string.Empty,   // telefono
		false,          // guardia
		DateTime.Today, // fechaIngreso
		default              // sueldoMinimoGarantizado
	);




	public ViewModelMedico(
		ObservableCollection<ViewModelHorario> horarios,
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
		return Medico2025.Crear(
			NombreCompleto2025.Crear(this.Name, this.LastName),
			EspecialidadMedica2025.CrearPorCodigoInterno(
				this.EspecialidadCodigoInterno
			//EspecialidadMedica2025.RamasDisponibles.First()
			),
			DniArgentino2025.Crear(this.Dni),
			DomicilioArgentino2025.Crear(
			LocalidadDeProvincia2025.Crear(this.Localidad, ProvinciaArgentina2025.Crear(this.Provincia)),
			this.Domicilio
		),
			ContactoTelefono2025.Crear(this.Telefono),
			ListaHorarioMedicos2025.Crear(
			this.Horarios.Select(h =>
				HorarioMedico2025.Crear(
					DiaSemana2025.Crear(h.DiaSemana),
					HorarioHora2025.Crear(h.Desde),
					HorarioHora2025.Crear(h.Hasta)
				))
			),
			FechaIngreso2025.Crear(this.FechaIngreso),
			MedicoSueldoMinimo2025.Crear(this.SueldoMinimoGarantizado),
			this.Guardia ?? false
		);
	}





}