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

			var resultado = this.ToDomain();

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


		private Result<Paciente2025> ToDomain() {
			var nombreResult = NombreCompleto2025.Crear(txtName.Text, txtLastName.Text);
			var dniResult = DniArgentino2025.Crear(txtDni.Text);
			var telefonoResult = Contacto2025Telefono.Crear(txtTelefono.Text);
			var correoResult = Contacto2025CorreoElectronico.Crear(txtEmail.Text);



			var contactoResult = Contacto2025.Crear(correoResult, telefonoResult);
			var provinciaResult = ProvinciaDeArgentina2025.Crear(txtProvincia.Text);
			var localidadResult = LocalidadDeProvincia2025.Crear(txtLocalidad.Text, provinciaResult);
			var domicilioResult = DomicilioArgentino2025.Crear(localidadResult, txtDomicilio.Text);

			var fechaNacRes = txtFechaNacimiento.SelectedDate is DateTime fechaNac
				? FechaDeNacimiento2025.Crear(DateOnly.FromDateTime(fechaNac))
				: new Result<FechaDeNacimiento2025>.Error("Debe seleccionar una fecha de nacimiento válida.");

			var fechaIngRes = txtFechaIngreso.SelectedDate is DateTime fechaIng
				? FechaIngreso2025.Crear(DateOnly.FromDateTime(fechaIng))
				: new Result<FechaIngreso2025>.Error("Debe seleccionar una fecha de ingreso válida.");

			return Paciente2025.Crear(
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
