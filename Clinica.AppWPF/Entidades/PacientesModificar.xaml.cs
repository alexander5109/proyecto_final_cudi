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
			//---------Crear-----------//
			if (SelectedPaciente is null) {
				this.ToDomain().Switch(
					ok => {
						SelectedPaciente = new Paciente(this);
						if (App.BaseDeDatos.CreatePaciente(ok, SelectedPaciente)) {
							this.Cerrar();
						}
					},
					error => {
						MessageBox.Show($"No se puede guardar el paciente: {error}", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}
				);
				return;
			}
			//---------Modificar-----------//
			this.ToDomain().Switch(
				ok => {
					SelectedPaciente.LeerDesdeVentana(this);
					if (App.BaseDeDatos.UpdatePaciente(ok, SelectedPaciente.Id)) {
						this.Cerrar();
					}
				},
				error => {
					MessageBox.Show($"No se puede guardar el paciente: {error}", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}
			);
		}

		private Result<Paciente2025> ToDomain() {
			var nombreRes = NombreCompleto2025.Crear(txtName.Text, txtLastName.Text);
			var dniRes = DniArgentino2025.Crear(txtDni.Text);
			var telefonoRes = Contacto2025Telefono.Crear(txtTelefono.Text);
			var correoRes = Contacto2025CorreoElectronico.Crear(txtEmail.Text);
			var contactoRes = Contacto2025.Crear(correoRes, telefonoRes);
			var provinciaRes = ProvinciaDeArgentina2025.Crear(txtProvincia.Text);
			var localidadRes = LocalidadDeProvincia2025.Crear(txtLocalidad.Text, provinciaRes);
			var domicilioRes = DomicilioArgentino2025.Crear(localidadRes, txtDomicilio.Text);

			var fechaNacRes = txtFechaNacimiento.SelectedDate is DateTime fechaNac
				? FechaDeNacimiento2025.Crear(DateOnly.FromDateTime(fechaNac))
				: new Result<FechaDeNacimiento2025>.Error("Debe seleccionar una fecha de nacimiento válida.");

			var fechaIngRes = txtFechaIngreso.SelectedDate is DateTime fechaIng
				? FechaDeIngreso2025.Crear(DateOnly.FromDateTime(fechaIng))
				: new Result<FechaDeIngreso2025>.Error("Debe seleccionar una fecha de ingreso válida.");

			return Paciente2025.Crear(
				nombreRes,
				dniRes,
				contactoRes,
				domicilioRes,
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
