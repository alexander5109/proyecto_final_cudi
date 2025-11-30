using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clinica.AppWPF.ViewModels;
//---------------------------------Tablas.WindowListarPacientes-------------------------------//
public partial class WindowModificarPacienteViewModel : ObservableObject {
	[ObservableProperty] private int? id = default;
	[ObservableProperty] private string dni = string.Empty;
	[ObservableProperty] private string nombre = string.Empty;
	[ObservableProperty] private string apellido = string.Empty;
	[ObservableProperty] private DateTime? fechaIngreso;
	[ObservableProperty] private string domicilio = string.Empty;
	[ObservableProperty] private string localidad = string.Empty;
	[ObservableProperty] private byte provinciaCodigo = default;
	[ObservableProperty] private string telefono = string.Empty;
	[ObservableProperty] private string email = string.Empty;
	[ObservableProperty] private DateTime? fechaNacimiento;
	public string Displayear => $"{Id}: {Nombre} {Apellido}";

	public static WindowModificarPacienteViewModel NewEmpty() => new(
		default,   // id
		string.Empty,   // dni
		string.Empty,   // nombre
		string.Empty,   // apellido
		null,           // fechaIngreso
		string.Empty,   // email
		string.Empty,   // telefono
		null,           // fechaNacimiento
		string.Empty,   // domicilio
		string.Empty,   // localidad
		default    // provinciaCodigo
	);


	public WindowModificarPacienteViewModel(
		int? id,
		string? dni,
		string? name,
		string? lastName,
		DateTime? fechaIngreso,
		string? email,
		string? telefono,
		DateTime? fechaNacimiento,
		string? domicilio,
		string? localidad,
		byte? provincia
	) {
		Id = id;
		Dni = dni ?? string.Empty;
		Nombre = name ?? string.Empty;
		Apellido = lastName ?? string.Empty;
		FechaIngreso = fechaIngreso;
		Email = email ?? string.Empty;
		Telefono = telefono ?? string.Empty;
		FechaNacimiento = fechaNacimiento;
		Domicilio = domicilio ?? string.Empty;
		Localidad = localidad ?? string.Empty;
		ProvinciaCodigo = provincia ?? 0;
	}

	public Result<Paciente2025> ToDomain() {


		//throw new NotImplementedException("Implementar Medico2025 ToDomain");
		return Paciente2025.Crear(
			PacienteId.Crear(Id),
			NombreCompleto2025.Crear(Nombre, Apellido),
			DniArgentino2025.Crear(Dni),
			Contacto2025.Crear(
				ContactoEmail2025.Crear(Email),
				ContactoTelefono2025.Crear(Telefono)
			),
			DomicilioArgentino2025.Crear(
				LocalidadDeProvincia2025.Crear(this.Localidad, ProvinciaArgentina2025.CrearPorCodigo(this.ProvinciaCodigo)),
				this.Domicilio
			),
			FechaDeNacimiento2025.Crear(FechaNacimiento),
			FechaRegistro2025.Crear(FechaIngreso)
		);
	}
}
