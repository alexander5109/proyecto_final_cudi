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
			App.PlayClickJewel();
			// ---------AsegurarInput-----------//
			//if (!CamposCompletadosCorrectamente()) {
			//	return;
			//}

			//---------Crear-----------//
			if (SelectedPaciente is null) {
				//Result<Paciente2025> resultadoConversion = ;
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
			}
			//---------Modificar-----------//
			else {
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
		}

		private Result<Paciente2025> ToDomain() {
			// convertir strings del formulario a los value objects
			var nombreRes = NombreCompleto2025.Crear(txtName.Text, txtLastName.Text);
			var dniRes = DniArgentino2025.Crear(txtDni.Text);
			var telefono = Contacto2025Telefono.Crear(txtTelefono.Text);
			var correo = Contacto2025CorreoElectronico.Crear(txtEmail.Text);
			var contactoRes = Contacto2025.Crear(correo, telefono);
			var provincia = ProvinciaDeArgentina2025.Crear(txtProvincia.Text);
			var localidad = LocalidadDeProvincia2025.Crear(txtLocalidad.Text, provincia);
			var domicilioRes = DomicilioArgentino2025.Crear(localidad, txtDomicilio.Text);
			var fechaRes = txtFechaNacimiento.SelectedDate is DateTime fechaNac
				? FechaDeNacimiento2025.Crear(DateOnly.FromDateTime(fechaNac))
				: new Result<FechaDeNacimiento2025>.Error("Debe seleccionar una fecha de nacimiento válida.");
			var fechaIngRes = txtFechaIngreso.SelectedDate is DateTime fechaIng
				? FechaDeIngreso2025.Crear(DateOnly.FromDateTime(fechaIng))
				: new Result<FechaDeIngreso2025>.Error("Debe seleccionar una fecha de ingreso válida.");

			// combinar resultados: si alguno falla, devolvemos el error correspondiente
			if (nombreRes is Result<NombreCompleto2025>.Error e1) return new Result<Paciente2025>.Error(e1.Mensaje);
			if (dniRes is Result<DniArgentino2025>.Error e2) return new Result<Paciente2025>.Error(e2.Mensaje);
			if (contactoRes is Result<Contacto2025>.Error e3) return new Result<Paciente2025>.Error(e3.Mensaje);
			if (domicilioRes is Result<DomicilioArgentino2025>.Error e4) return new Result<Paciente2025>.Error(e4.Mensaje);
			if (fechaRes is Result<FechaDeNacimiento2025>.Error e5) return new Result<Paciente2025>.Error(e5.Mensaje);

			// si todo está bien, creamos el Paciente2025
			var paciente = new Paciente2025(
				((Result<NombreCompleto2025>.Ok)nombreRes).Value,
				((Result<DniArgentino2025>.Ok)dniRes).Value,
				((Result<Contacto2025>.Ok)contactoRes).Value,
				((Result<DomicilioArgentino2025>.Ok)domicilioRes).Value,
				((Result<FechaDeNacimiento2025>.Ok)fechaRes).Value,
				((Result<FechaDeIngreso2025>.Ok)fechaIngRes).Value
			);

			return new Result<Paciente2025>.Ok(paciente);
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
