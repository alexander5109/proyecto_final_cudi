using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Entidades;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;


public partial class PacienteModificar : Window {
	private Paciente ViewModel = new();
	private PacienteId PacienteId = new(-1);
	public PacienteModificar() {
		InitializeComponent();
	}

	public PacienteModificar(PacienteId id) {
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
		//	var nuevpacoiente = new Paciente(this);
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
		this.Cerrar(); // this.NavegarA<Pacientes>();
	}

	private void ButtonSalir(object sender, RoutedEventArgs e) {
		this.Salir();
	}

	private void Window_Activated(object sender, EventArgs e) {

	}
	//------------------------Fin----------------------//
}
