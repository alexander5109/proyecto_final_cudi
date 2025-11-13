using System.Collections.ObjectModel;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Clinica.AppWPF.ModelViews;


public partial class MedicoView : ObservableObject {
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
	[ObservableProperty] private ObservableCollection<HorarioMedicoView> horarios = [];
	public List<string> EspecialidadesDisponibles { get; } = MedicoEspecialidad2025.EspecialidadesDisponibles;
	[JsonIgnore] public string Displayear => $"{Id}: {Especialidad} - {Name} {LastName}";

	public static MedicoView NewEmpty() => new MedicoView(
		new ObservableCollection<HorarioMedicoView>(),
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


	public MedicoView(
		ObservableCollection<HorarioMedicoView> horarios,
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
	}



	public Result<Medico2025> ToDomain() {
		// --- Value Objects base ---
		var nombreResult = NombreCompleto2025.Crear(this.Name, this.LastName);
		var especialidadResult = MedicoEspecialidad2025.Crear(this.Especialidad, "Clinica General");
		var dniResult = DniArgentino2025.Crear(this.Dni);
		var domicilioResult = DomicilioArgentino2025.Crear(
			LocalidadDeProvincia2025.Crear(this.Localidad, ProvinciaArgentina2025.Crear(this.Provincia)),
			this.Domicilio
		);
		var telefonoResult = ContactoTelefono2025.Crear(this.Telefono);
		var fechaIngresoResult = FechaIngreso2025.Crear(this.FechaIngreso);
		var sueldoResult = MedicoSueldoMinimo2025.Crear(this.SueldoMinimoGarantizado);
		Result<ListaHorarioMedicos2025> horariosResult = ListaHorarioMedicos2025.Crear(
			this.Horarios
				.SelectMany(dia =>
					dia.FranjasHora.Select(h => HorarioMedico2025.Crear(
						DiaSemana2025.Crear(dia.DiaName),
						HorarioHora2025.Crear(h.Desde),
						HorarioHora2025.Crear(h.Hasta)
					))
				)
				.ToList()
		);

		// --- Combinamos todo en el agregado ---
		return Medico2025.Crear(
			nombreResult,
			especialidadResult,
			dniResult,
			domicilioResult,
			telefonoResult,
			horariosResult,
			fechaIngresoResult,
			sueldoResult,
			this.Guardia ?? false
		);
	}





}