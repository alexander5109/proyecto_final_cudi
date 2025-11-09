using Clinica.AppWPF.Mappers;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System.Windows;

namespace Clinica.AppWPF {
	public partial class PacientesModificar : Window {
		private static Result<Paciente2025EnDb>? SelectedPaciente;

		//---------------------Constructores-------------------//
		public PacientesModificar() // Constructor vacío => Crear
		{
			InitializeComponent();
			SelectedPaciente = null;
		}

		public PacientesModificar(Result<Paciente2025EnDb> selectedPaciente) // Modificar
		{
			InitializeComponent();

			// Guardamos el Result tal cual
			SelectedPaciente = selectedPaciente;

			selectedPaciente.Match(
				ok => {
					// Mostramos los datos válidos en la ventana
					PacienteMapper.MostrarEnVentana(ok, this);
					this.Title = $"Modificar Paciente - {ok.Paciente.NombreCompleto}";
					return 0;
				},
				error => {
					// En caso de Error, dejamos la ventana con campos vacíos o defaults
					//PacienteMapper.MostrarEnVentana(new Result<Paciente2025EnDb>.Error(error), this);
					this.Title = "Modificar Paciente - Datos Inválidos";
					return 0;
				}
			);
		}


		//---------------------Leer desde ventana-------------------//
		public static Result<Paciente2025EnDb> LeerDesdeVentana(PacientesModificar window) {
			var nombreResult = NombreCompleto2025.Crear(window.txtName.Text, window.txtLastName.Text);
			var dniResult = DniArgentino2025.Crear(window.txtDni.Text);
			var fechaResult = FechaDeNacimiento2025.Crear(window.txtFechaNacimiento.SelectedDate!.Value);
			var contactoResult = Contacto2025.Crear(window.txtEmail.Text, window.txtTelefono.Text);
			var domicilioResult = DomicilioArgentino2025.Crear(window.txtProvincia.Text, window.txtLocalidad.Text, window.txtDomicilio.Text);

			// Early return con el primer error que ocurra
			if (nombreResult is Result<NombreCompleto2025>.Error e1) return new Result<Paciente2025EnDb>.Error(e1.Mensaje);
			if (dniResult is Result<DniArgentino2025>.Error e2) return new Result<Paciente2025EnDb>.Error(e2.Mensaje);
			if (fechaResult is Result<FechaDeNacimiento2025>.Error e3) return new Result<Paciente2025EnDb>.Error(e3.Mensaje);
			if (contactoResult is Result<Contacto2025>.Error e4) return new Result<Paciente2025EnDb>.Error(e4.Mensaje);
			if (domicilioResult is Result<DomicilioArgentino2025>.Error e5) return new Result<Paciente2025EnDb>.Error(e5.Mensaje);

			// Todos válidos
			var paciente = new Paciente2025(
				((Result<NombreCompleto2025>.Ok)nombreResult).Value,
				((Result<DniArgentino2025>.Ok)dniResult).Value,
				((Result<Contacto2025>.Ok)contactoResult).Value,
				((Result<DomicilioArgentino2025>.Ok)domicilioResult).Value,
				((Result<FechaDeNacimiento2025>.Ok)fechaResult).Value
			);

			return new Result<Paciente2025EnDb>.Ok(new Paciente2025EnDb("", paciente));
		}

		//---------------------Botón Guardar-------------------//
		private void ButtonGuardar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();

			var pacienteResult = LeerDesdeVentana(this);

			pacienteResult.Match(
				ok => {
					if (SelectedPaciente is null) {
						// Crear
						if (App.BaseDeDatos.CreatePaciente(ok.Paciente)) {
							this.Cerrar();
						}
					} else {
						// Modificar
						if (App.BaseDeDatos.UpdatePaciente(ok)) {
							this.Cerrar();
						}
					}
					return 0;
				},
				error => {
					MessageBox.Show(error, "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
					return 0;
				}
			);
		}

		//---------------------Botón Eliminar-------------------//
		private void ButtonEliminar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();

			if (SelectedPaciente is null) {
				MessageBox.Show("No hay paciente seleccionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			SelectedPaciente.Match(
				ok => {
					if (MessageBox.Show(
						"¿Está seguro que desea eliminar este paciente?",
						"Confirmar Eliminación",
						MessageBoxButton.OKCancel,
						MessageBoxImage.Warning) == MessageBoxResult.OK) {
						if (App.BaseDeDatos.DeletePaciente(ok)) {
							this.Cerrar();
						}
					}
					return 0; // Match necesita un TOut
				},
				error => {
					MessageBox.Show($"No se puede eliminar el paciente porque hay errores en los datos: {error}",
						"Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return 0;
				}
			);
		}


		//---------------------Botones de salida-------------------//
		private void ButtonCancelar(object sender, RoutedEventArgs e) => this.Cerrar();
		private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
	}
}
