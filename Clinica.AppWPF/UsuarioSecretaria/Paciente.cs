using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class Paciente : ObservableObject {
	[ObservableProperty] private int? id = default;
	[ObservableProperty] private string dni = string.Empty;
	[ObservableProperty] private string nombre = string.Empty;
	[ObservableProperty] private string apellido = string.Empty;
	[ObservableProperty] private DateTime fechaIngreso;
	[ObservableProperty] private string domicilio = string.Empty;
	[ObservableProperty] private string localidad = string.Empty;
	[ObservableProperty] private ProvinciaCodigo2025 provinciaCodigo = default;
	[ObservableProperty] private string telefono = string.Empty;
	[ObservableProperty] private string email = string.Empty;
	[ObservableProperty] private DateTime fechaNacimiento = DateTime.MinValue;
	public string Provincia {
		get => ProvinciaCodigo.ATexto();
		set { }
	}
	public string Displayear => $"{Id}: {Nombre} {Apellido}";
}
