using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using System.Windows;
using System.Windows.Controls;

namespace Clinica.AppWPF {
    public partial class TurnosModificar : Window {
		private static Turno? SelectedTurno;

		//---------------------public.constructors-------------------//
        public TurnosModificar() //Constructor vacio ==> Crear.
		{
            InitializeComponent();
			LLenarComboBoxes();
			SelectedTurno = null;
		}

		public TurnosModificar(Turno selectedTurno) //Constructor con un objeto como parametro ==> Modificarlo.
		{
			InitializeComponent();
			LLenarComboBoxes();
			SelectedTurno = selectedTurno;
			SelectedTurno.MostrarseEnVentana(this);
		}


		//---------------------Visualizacion-comboboxes-------------------//
		private void LLenarComboBoxes()  //por defecto llenamos todos los comboboxes
		{
			txtEspecialidades.ItemsSource = App.BaseDeDatos.ReadDistinctEspecialidades();

			txtPacientes.ItemsSource = App.BaseDeDatos.ReadPacientes();
			txtPacientes.DisplayMemberPath = "Displayear";	//Property de cada Objeto para mostrarse como una union de dni nombre y apellido. 
			
			txtMedicos.ItemsSource = App.BaseDeDatos.ReadMedicos();
			txtMedicos.DisplayMemberPath = "Displayear";	//Property de cada Objeto para mostrarse como una union de dni nombre y apellido. 
		}
		private void txtEspecialidades_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			txtMedicos.SelectedValuePath = "Id";
			txtMedicos.DisplayMemberPath = "Displayear";    //Property de cada Objeto para mostrarse como una union de dni nombre y apellido. 
			txtMedicos.ItemsSource = App.BaseDeDatos.ReadMedicosWhereEspecialidad(txtEspecialidades.SelectedItem.ToString());
        }

		//--------------------AsegurarInput-------------------//
		private bool CamposCompletadosCorrectamente(){
			if (
				this.txtPacientes.SelectedValue is null 
				|| this.txtMedicos.SelectedValue is null 
				|| this.txtFecha.SelectedDate is null 
				|| string.IsNullOrEmpty(this.txtHora.Text)
			) {
				MessageBox.Show($"Error: Faltan datos obligatorios por completar", "Faltan datos.", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
			} 

			if ( !App.TryParseHoraField(this.txtHora.Text) ){
				MessageBox.Show($"Error: No se reconoce la hora. \n Ingrese un string con formato valido (hh:mm)");
				return false;
			}
			return true;
		}

		//---------------------botones.GuardarCambios-------------------//

		private void ButtonGuardar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();

			Result<TurnoType> resultado = this.ToDomain();

			resultado.Switch(
				ok => {
					bool exito;

					if (SelectedTurno is null) {
						// Crear nuevo turno
						SelectedTurno = new Turno(this);
						exito = App.BaseDeDatos.CreateTurno(ok, SelectedTurno);
					} else {
						// Actualizar existente
						SelectedTurno.LeerDesdeVentana(this);
						exito = App.BaseDeDatos.UpdateTurno(ok, SelectedTurno);
					}

					if (exito)
						this.Cerrar();
				},
				error => {
					MessageBox.Show(
						$"No se puede guardar el turno: {error}",
						"Error de ingreso",
						MessageBoxButton.OK,
						MessageBoxImage.Warning
					);
				}
			);
		}

		private Result<TurnoType> ToDomain() {
			throw new Exception("Not implemented yet");
		}
		
		
		//---------------------botones.Eliminar-------------------//
		private void ButtonEliminar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();
			//---------Checknulls-----------//
			if (SelectedTurno is null) {
				MessageBox.Show($"No hay item seleccionado.");
				return;
			}
			//---------confirmacion-----------//
			if (MessageBox.Show($"¿Está seguro que desea eliminar este turno?",
				"Confirmar Eliminación",
				MessageBoxButton.OKCancel,
				MessageBoxImage.Warning
			) != MessageBoxResult.OK) {
				return;
			}
			//---------Eliminar-----------//
			if (App.BaseDeDatos.DeleteTurno(SelectedTurno)){
				this.Cerrar();
			}
		}
		
		
		//---------------------botones.Salida-------------------//
		private void ButtonCancelar(object sender, RoutedEventArgs e) {
			this.Cerrar(); // this.NavegarA<Turnos>();
		}
		private void ButtonSalir(object sender, RoutedEventArgs e) {
			this.Salir();
		}
        //------------------------Fin---------------------------//
    }
}
