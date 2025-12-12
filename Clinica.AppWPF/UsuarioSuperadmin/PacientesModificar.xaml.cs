using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioSuperadmin;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioSuperadmin;

public partial class PacientesModificar : Window {
	private static PacienteDbModel? SelectedPaciente;
	//---------------------public.constructors-------------------//
	public PacientesModificar() //Constructor vacio ==> Crear.
	{
		InitializeComponent();
		SelectedPaciente = null;
	}

	public PacientesModificar(PacienteDbModel selectedPaciente) //Constructor con un objeto como parametro ==> Modificarlo.
	{
		InitializeComponent();
		SelectedPaciente = selectedPaciente;
		SelectedPaciente.MostrarseEnVentana(this);
	}


	//--------------------AsegurarInput-------------------//
	private bool CamposCompletadosCorrectamente() {
		if (
			 string.IsNullOrEmpty(this.txtDni.Text) ||
			 string.IsNullOrEmpty(this.txtName.Text) ||
			 string.IsNullOrEmpty(this.txtLastName.Text) ||
			 string.IsNullOrEmpty(this.txtEmail.Text) ||
			 string.IsNullOrEmpty(this.txtTelefono.Text) ||
			 string.IsNullOrEmpty(this.txtDomicilio.Text) ||
			 string.IsNullOrEmpty(this.txtLocalidad.Text) ||
			 string.IsNullOrEmpty(this.txtProvincia.Text) ||
			 this.txtFechaIngreso.SelectedDate is null ||
			 this.txtFechaNacimiento.SelectedDate is null
		) {
			MessageBox.Show($"Error: Faltan datos obligatorios por completar.", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
			return false;
		}

		if (!Int64.TryParse(this.txtDni.Text, out _)) {
			MessageBox.Show($"Error: El dni no es un numero entero valido.", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
			return false;
		}
		return true;
	}


	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		// ---------AsegurarInput-----------//
		if (!CamposCompletadosCorrectamente()) {
			return;
		}

		//---------Crear-----------//
		if (SelectedPaciente is null) {
			//var nuevpacoiente = new PacienteDbModel(this);
			//if (App.BaseDeDatos.CreatePaciente(nuevpacoiente)) {
			//	this.Cerrar();
			//}
		}
		//---------Modificar-----------//
		else {
			SelectedPaciente.LeerDesdeVentana(this);
			//if (App.BaseDeDatos.UpdatePaciente(SelectedPaciente)) {
			//	this.Cerrar();
			//}
		}
	}


	//---------------------botones.Eliminar-------------------//
	private void ButtonEliminar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		//---------Checknulls-----------//
		if (SelectedPaciente is null || SelectedPaciente.Dni is null) {
			MessageBox.Show($"No hay item seleccionado.");
			return;
		}
		//---------confirmacion-----------//
		if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedPaciente.Nombre}",
			"Confirmar Eliminación",
			MessageBoxButton.OKCancel,
			MessageBoxImage.Warning
		) != MessageBoxResult.OK) {
			return;
		}
		//---------Eliminar-----------//
		//if (App.BaseDeDatos.DeletePaciente(SelectedPaciente)) {
		//	this.Cerrar(); // this.NavegarA<Medicos>();
		//}
	}
	//---------------------botones.Salida-------------------//
	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e)  => this.Cerrar();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e) {
		this.Salir();
	}
	//------------------------Fin----------------------//
}
