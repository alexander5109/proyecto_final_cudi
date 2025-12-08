using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;



public partial class SecretariaPacientesModificar : Window {
	private PacienteUpdateViewModel ViewModel = new();
	private PacienteId PacienteId = new(-1);
	public SecretariaPacientesModificar() {
		InitializeComponent();
	}

	public SecretariaPacientesModificar(PacienteId id) {
		PacienteId = id;
		InitializeComponent();
		_ = CargaInicialAsync();
		DataContext = ViewModel;

	}

	private async Task CargaInicialAsync() {
		PacienteDto? dto = await App.Repositorio.SelectPacienteWhereId(PacienteId);
		if (dto == null) {
			MessageBox.Show("No se encontró el paciente con el Id proporcionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		} else {
			ViewModel = dto.ToViewModel();
			DataContext = ViewModel;
		}
	}

	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		//App.PlayClickJewel();
		// ---------AsegurarInput-----------//
		//if (!CamposCompletadosCorrectamente()) {
		//	return;
		//}

		//---------CrearResult-----------//
		//if (ViewModel is null) {
		//	var nuevpacoiente = new PacienteUpdateViewModel(this);
		//	if (App.Repositorio.CreatePaciente(nuevpacoiente)) {
		//		this.Cerrar();
		//	}
		//}
		//---------Modificar-----------//
		//else {
		//	ViewModel.LeerDesdeVentana(this);
		//	if (App.Repositorio.UpdatePaciente(ViewModel)) {
		//		this.Cerrar();
		//	}
		//}
	}


	//---------------------botones.Eliminar-------------------//
	private void ButtonEliminar(object sender, RoutedEventArgs e) {
		//App.PlayClickJewel();
		//---------Checknulls-----------//
		//if (ViewModel is null || ViewModel.Dni is null) {
		//	MessageBox.Show($"No hay item seleccionado.");
		//	return;
		//}
		//---------confirmacion-----------//
		//if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {ViewModel.Name}",
		//	"Confirmar Eliminación",
		//	MessageBoxButton.OKCancel,
		//	MessageBoxImage.Warning
		//) != MessageBoxResult.OK) {
		//	return;
		//}
		//---------Eliminar-----------//
		//if (App.Repositorio.DeletePaciente(ViewModel)) {
		//	this.Cerrar(); // this.NavegarA<Medicos>();
		//}
	}
	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); // this.NavegarA<SecretariaPacientes>();
	}

	private void ButtonSalir(object sender, RoutedEventArgs e) {
		this.Salir();
	}

	private void Window_Activated(object sender, EventArgs e) {

	}


	private void ButtonSolicitarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is not null) {
			this.AbrirComoDialogo<SecretariaBuscadorDeDisponibilidades>(VM.SelectedPaciente);
		}
	}
	//------------------------Fin----------------------//
}

public partial class PacienteUpdateViewModel : ObservableObject {
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

public static class SecretariaPacienteExtentions {
	public static PacienteUpdateViewModel ToViewModel(this PacienteDto dto)
		=> new PacienteUpdateViewModel {
			Id = dto.Id.Valor,
			Dni = dto.Dni,
			Nombre = dto.Nombre,
			Apellido = dto.Apellido,
			FechaIngreso = dto.FechaIngreso,
			Email = dto.Email,
			Telefono = dto.Telefono,
			FechaNacimiento = dto.FechaNacimiento,
			Domicilio = dto.Domicilio,
			Localidad = dto.Localidad,
			ProvinciaCodigo = dto.ProvinciaCodigo
		};


	public static Result<Paciente2025> ToDomain(this PacienteUpdateViewModel viewModel) {
		return Paciente2025.CrearResult(
			PacienteId.CrearResult(viewModel.Id),
			NombreCompleto2025.CrearResult(viewModel.Nombre, viewModel.Apellido),
			DniArgentino2025.CrearResult(viewModel.Dni),
			Contacto2025.CrearResult(
				ContactoEmail2025.CrearResult(viewModel.Email),
				ContactoTelefono2025.CrearResult(viewModel.Telefono)
			),
			DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(viewModel.Localidad, ProvinciaArgentina2025.CrearResultPorCodigo(viewModel.ProvinciaCodigo)),
				viewModel.Domicilio
			),
			FechaDeNacimiento2025.CrearResult(viewModel.FechaNacimiento),
			FechaRegistro2025.CrearResult(viewModel.FechaIngreso)
		);
	}

}
