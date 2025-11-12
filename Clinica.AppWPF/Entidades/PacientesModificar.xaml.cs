using Clinica.Dominio;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System.Windows;

namespace Clinica.AppWPF {
	public partial class PacientesModificar : Window {
		private static Paciente? SelectedPaciente;
		//---------------------public.constructors-------------------//
		public PacientesModificar() //Constructor vacio ==> Crear.
		{
			InitializeComponent();
			SelectedPaciente = null;
		}

		public PacientesModificar(Paciente selectedPaciente) //Constructor con un objeto como parametro ==> Modificarlo.
		{
			InitializeComponent();
			SelectedPaciente = selectedPaciente;
			SelectedPaciente.MostrarseEnVentana(this);
		}


		//---------------------botones.GuardarCambios-------------------//
		private void ButtonGuardar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();

			Result<PacienteType> resultado = this.ToDomain();

			resultado.Switch(
				ok => {
					bool exito;

					if (SelectedPaciente is null) {
						// Crear nuevo paciente
						SelectedPaciente = new Paciente(this);
						exito = App.BaseDeDatos.CreatePaciente(ok, SelectedPaciente);
					} else {
						// Actualizar existente
						SelectedPaciente.LeerDesdeVentana(this);
						exito = App.BaseDeDatos.UpdatePaciente(ok, SelectedPaciente.Id);
					}

					if (exito)
						this.Cerrar();
				},
				error => {
					MessageBox.Show(
						$"No se puede guardar el paciente: {error}",
						"Error de ingreso",
						MessageBoxButton.OK,
						MessageBoxImage.Warning
					);
				}
			);
		}


		private Result<PacienteType> ToDomain() {
			var nombreResult = NombreCompletoType.Crear(txtName.Text, txtLastName.Text);
			var dniResult = DniArgentinoType.Crear(txtDni.Text);
			var telefonoResult = ContactoTelefonoType.Crear(txtTelefono.Text);
			var correoResult = ContactoEmailType.Crear(txtEmail.Text);



			var contactoResult = ContactoType.Crear(correoResult, telefonoResult);
			var provinciaResult = ProvinciaDeArgentinaType.Crear(txtProvincia.Text);
			var localidadResult = LocalidadDeProvinciaType.Crear(txtLocalidad.Text, provinciaResult);
			var domicilioResult = DomicilioArgentinoType.Crear(localidadResult, txtDomicilio.Text);

			var fechaNacRes = txtFechaNacimiento.SelectedDate is DateTime fechaNac
				? FechaDeNacimientoType.Crear(DateOnly.FromDateTime(fechaNac))
				: new Result<FechaDeNacimientoType>.Error("Debe seleccionar una fecha de nacimiento válida.");

			var fechaIngRes = txtFechaIngreso.SelectedDate is DateTime fechaIng
				? FechaIngresoType.Crear(DateOnly.FromDateTime(fechaIng))
				: new Result<FechaIngresoType>.Error("Debe seleccionar una fecha de ingreso válida.");

			return PacienteType.Crear(
				nombreResult,
				dniResult,
				contactoResult,
				domicilioResult,
				fechaNacRes,
				fechaIngRes
			);
		}


		//---------------------botones.Eliminar-------------------//
		private void ButtonEliminar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();
			//---------Checknulls-----------//
			if (SelectedPaciente is null || SelectedPaciente.Dni is null) {
				MessageBox.Show($"No hay item seleccionado.");
				return;
			}
			//---------confirmacion-----------//
			if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedPaciente.Name}",
				"Confirmar Eliminación",
				MessageBoxButton.OKCancel,
				MessageBoxImage.Warning
			) != MessageBoxResult.OK) {
				return;
			}
			//---------Eliminar-----------//
			if (App.BaseDeDatos.DeletePaciente(SelectedPaciente)) {
				this.Cerrar(); // this.NavegarA<Medicos>();
			}
		}
		//---------------------botones.Salida-------------------//
		private void ButtonCancelar(object sender, RoutedEventArgs e) {
			this.Cerrar(); // this.NavegarA<Pacientes>();
		}

		private void ButtonSalir(object sender, RoutedEventArgs e) {
			this.Salir();
		}
		//------------------------Fin----------------------//
	}
}
