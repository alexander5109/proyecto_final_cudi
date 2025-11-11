using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using System.Windows;

namespace Clinica.AppWPF {
	public partial class MedicosModificar : Window {
		private static Medico? SelectedMedico;
		//---------------------public.constructors-------------------//
		public MedicosModificar() //Constructor con un objeto como parametro ==> Modificarlo.
		{
			InitializeComponent();
			SelectedMedico = null;
			txtDiasDeAtencion.ItemsSource = HorarioMedico.GetDiasDeLaSemanaAsList();
		}

		public MedicosModificar(Medico selectedMedico) //Constructor con un objeto como parametro ==> Modificarlo.
		{
			InitializeComponent();
			SelectedMedico = selectedMedico;
			SelectedMedico.MostrarseEnVentana(this);
		}
		

		//--------------------AsegurarInput-------------------//
		private bool CamposCompletadosCorrectamente(){
			if (
				this.txtSueldoMinimoGarantizado.Text is null 
				|| this.txtDni.Text is null 
				|| this.txtFechaIngreso.SelectedDate is null 
				|| this.txtGuardia.IsChecked is null
			) {
				MessageBox.Show($"Error: Faltan datos obligatorios por completar.", "Error de ingreso.", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
			 }
					 
			if (!Int64.TryParse(this.txtDni.Text, out _)){
                MessageBox.Show($"Error: El dni no es un numero entero valido.", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
            }
					 
			if (!Double.TryParse(this.txtSueldoMinimoGarantizado.Text, out _)) {
				MessageBox.Show("Error: El sueldo minimo no es un número decimal válido. Use la coma (,) como separador decimal.", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
			}
			foreach (HorarioMedico campo in this.txtDiasDeAtencion.ItemsSource as List<HorarioMedico>){
				if(  string.IsNullOrEmpty(campo.HoraInicio) && string.IsNullOrEmpty(campo.HoraFin) ){
					continue;
				} 
				if( App.TryParseHoraField(campo.HoraInicio) && App.TryParseHoraField(campo.HoraFin) ){
					continue;
				} 
				else{	
					MessageBox.Show($"Error: No se reconoce el horario del día {campo.DiaSemana}. \nIngrese un string con formato valido (hh:mm)", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
					return false;
				}
			}
					 
			return true;
		}



		//---------------------botones.GuardarCambios-------------------//
		private void ButtonGuardar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();

			Result<Medico2025> resultado = this.ToDomain();

			resultado.Switch(
				ok => {
					bool exito;

					if (SelectedMedico is null) {
						SelectedMedico = new Medico(this);
						exito = App.BaseDeDatos.CreateMedico(ok, SelectedMedico);
					} else {
						// Actualizar existente
						SelectedMedico.LeerDesdeVentana(this);
						exito = App.BaseDeDatos.UpdateMedico(ok, SelectedMedico.Id);
					}

					if (exito)
						this.Cerrar();
				},
				error => {
					MessageBox.Show(
						$"No se puede guardar el médico: {error}",
						"Error de ingreso",
						MessageBoxButton.OK,
						MessageBoxImage.Warning
					);
				}
			);
		}

		private Result<Medico2025> ToDomain() {
			throw new NotImplementedException();
		}



		//---------------------botones.Eliminar-------------------//
		private void ButtonEliminar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();
			//---------Checknulls-----------//
			if (SelectedMedico is null || SelectedMedico.Dni is null) {
				MessageBox.Show($"No hay item seleccionado.");
				return;
			}
			//---------confirmacion-----------//
			if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedMedico.Name}", "Confirmar Eliminación", MessageBoxButton.OKCancel, MessageBoxImage.Warning ) != MessageBoxResult.OK) {
				return;
			}
			//---------Eliminar-----------//
			if (App.BaseDeDatos.DeleteMedico(SelectedMedico)){
				this.Cerrar(); // this.NavegarA<Medicos>();
			}
		}
		
		
		
		//---------------------botones.Salida-------------------//
		private void ButtonCancelar(object sender, RoutedEventArgs e) {
			this.Cerrar(); // this.NavegarA<Medicos>();
		}

		private void ButtonSalir(object sender, RoutedEventArgs e) {
			this.Salir();
		}
		//------------------------Fin---------------------------//
	}
}
