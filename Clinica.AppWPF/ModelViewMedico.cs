using System.Collections.ObjectModel;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Clinica.AppWPF.ModelViews;


public partial class ModelViewMedico : ObservableObject {


	[ObservableProperty]
	private ObservableCollection<ModelViewHorario> horarios = [];

	public ObservableCollection<ModelViewHorariosAgrupados> HorariosAgrupados {
		get {
			ObservableCollection<ModelViewHorariosAgrupados> lista = [];

			// Programar los 7 días vacíos
			foreach (DayOfWeek dia in DiaSemana2025.Los7EnumDias) {
				lista.Add(new ModelViewHorariosAgrupados(dia));
			}

			// Mapear horarios existentes
			foreach (ModelViewHorario horario in Horarios) {
				var grupo = lista.First(g => g.DiaSemana == horario.DiaSemana);
				grupo.Horarios.Add(horario);
			}

			return lista;
		}
	}



	[ObservableProperty] private string id = string.Empty;
	[ObservableProperty] private string name = string.Empty;
	[ObservableProperty] private string lastName = string.Empty;
	[ObservableProperty] private string telefono = string.Empty;
	[ObservableProperty] private string dni = string.Empty;
	[ObservableProperty] private string localidad = string.Empty;
	[ObservableProperty] private string especialidad = string.Empty;
	[ObservableProperty] private decimal sueldoMinimoGarantizado;
	[ObservableProperty] private string provincia = string.Empty;
	[ObservableProperty] private string domicilio = string.Empty;
	[ObservableProperty] private DateTime? fechaIngreso = DateTime.Today;
	[ObservableProperty] private bool? guardia = false;



	public List<string> EspecialidadesDisponibles { get; } = EspecialidadMedica2025.EspecialidadesDisponibles;
	[JsonIgnore] public string Displayear => $"{Id}: {Especialidad} - {Name} {LastName}";

	public static ModelViewMedico NewEmpty() => new(
		[],
		string.Empty,   // id
		string.Empty,   // name
		string.Empty,   // lastName
		string.Empty,   // dni
		string.Empty,   // provincia
		string.Empty,   // domicilio
		string.Empty,   // localidad
		string.Empty,   // especialidad
		string.Empty,   // telefono
		false,          // guardia
		DateTime.Today, // fechaIngreso
		0m              // sueldoMinimoGarantizado
	);


	public ModelViewMedico(
		ObservableCollection<ModelViewHorario> horarios,
		string? id,
		string? name,
		string? lastName,
		string? dni,
		string? provincia,
		string? domicilio,
		string? localidad,
		string? especialidad,
		string? telefono,
		bool? guardia,
		DateTime? fechaIngreso,
		decimal? sueldoMinimoGarantizado
	) {
		Horarios = horarios;
		Id = id ?? string.Empty;
		Name = name ?? string.Empty;
		LastName = lastName ?? string.Empty;
		Dni = dni ?? string.Empty;
		Provincia = provincia ?? string.Empty;
		Domicilio = domicilio ?? string.Empty;
		Localidad = localidad ?? string.Empty;
		Especialidad = especialidad ?? string.Empty;
		Telefono = telefono ?? string.Empty;
		Guardia = guardia ?? false;
		FechaIngreso = fechaIngreso ?? DateTime.Today;
		SueldoMinimoGarantizado = sueldoMinimoGarantizado ?? 0m;

		//Horarios.CollectionChanged += (_, __) => OnPropertyChanged(nameof(HorariosAgrupados));
	}



	public Result<Medico2025> ToDomain() {
		return Medico2025.Crear(
			NombreCompleto2025.Crear(this.Name, this.LastName),
			EspecialidadMedica2025.Crear(
				this.Especialidad
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