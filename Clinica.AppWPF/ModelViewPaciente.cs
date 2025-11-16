using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Clinica.AppWPF.ModelViews;
//---------------------------------Tablas.WindowListarPacientes-------------------------------//
public partial class ModelViewPaciente : ObservableObject {
	[ObservableProperty] private string id = string.Empty;
	[ObservableProperty] private string dni = string.Empty;
	[ObservableProperty] private string name = string.Empty;
	[ObservableProperty] private string lastName = string.Empty;
	[ObservableProperty] private DateTime? fechaIngreso;
	[ObservableProperty] private string email = string.Empty;
	[ObservableProperty] private string telefono = string.Empty;
	[ObservableProperty] private DateTime? fechaNacimiento;
	[ObservableProperty] private string domicilio = string.Empty;
	[ObservableProperty] private string localidad = string.Empty;
	[ObservableProperty] private string provincia = string.Empty;
	[JsonIgnore] public string Displayear => $"{Id}: {Name} {LastName}";

	public static ModelViewPaciente NewEmpty() => new(
		string.Empty,   // id
		string.Empty,   // dni
		string.Empty,   // name
		string.Empty,   // lastName
		null,           // fechaIngreso
		string.Empty,   // email
		string.Empty,   // telefono
		null,           // fechaNacimiento
		string.Empty,   // domicilio
		string.Empty,   // localidad
		string.Empty    // provincia
	);


	public ModelViewPaciente(
		string id,
		string? dni,
		string? name,
		string? lastName,
		DateTime? fechaIngreso,
		string? email,
		string? telefono,
		DateTime? fechaNacimiento,
		string? domicilio,
		string? localidad,
		string? provincia
	) {
		Id = id ?? string.Empty;
		Dni = dni ?? string.Empty;
		Name = name ?? string.Empty;
		LastName = lastName ?? string.Empty;
		FechaIngreso = fechaIngreso;
		Email = email ?? string.Empty;
		Telefono = telefono ?? string.Empty;
		FechaNacimiento = fechaNacimiento;
		Domicilio = domicilio ?? string.Empty;
		Localidad = localidad ?? string.Empty;
		Provincia = provincia ?? string.Empty;
	}

	public Result<Paciente2025> ToDomain() {
		return Paciente2025.Crear(
			NombreCompleto2025.Crear(Name, LastName),
			DniArgentino2025.Crear(Dni),
			Contacto2025.Crear(
				ContactoEmail2025.Crear(Email),
				ContactoTelefono2025.Crear(Telefono)
			),
			DomicilioArgentino2025.Crear(
				LocalidadDeProvincia2025.Crear(this.Localidad, ProvinciaArgentina2025.Crear(this.Provincia)),
				this.Domicilio
			),
			FechaDeNacimiento2025.Crear(FechaNacimiento),
			FechaIngreso2025.Crear(FechaIngreso)
		);
	}
}
