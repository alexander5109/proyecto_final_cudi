using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioSuperadmin;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioSuperadmin;

public static class Utilidades {
	public static bool TryParseHoraField(string? campo) {
		if (TimeOnly.TryParse(campo, out _)) {
			return true;
		} else {
			return false;
		}

	}
}
public partial class MedicosModificar : Window {
	private static MedicoDbModel? SelectedMedico;
	//---------------------public.constructors-------------------//
	public MedicosModificar() //Constructor con un objeto como parametro ==> Modificarlo.
	{
		InitializeComponent();
		SelectedMedico = null;
		txtDiasDeAtencion.ItemsSource = HorarioMedico.GetDiasDeLaSemanaAsList();
	}

	public MedicosModificar(MedicoDbModel selectedMedico) //Constructor con un objeto como parametro ==> Modificarlo.
	{
		InitializeComponent();
		SelectedMedico = selectedMedico;
		SelectedMedico.MostrarseEnVentana(this);
	}


	//--------------------AsegurarInput-------------------//
	private bool CamposCompletadosCorrectamente() {
		if (
			this.txtSueldoMinimoGarantizado.Text is null
			|| this.txtDni.Text is null
			|| this.txtFechaIngreso.SelectedDate is null
			|| this.txtGuardia.IsChecked is null
		) {
			MessageBox.Show($"Error: Faltan datos obligatorios por completar.", "Error de ingreso.", MessageBoxButton.OK, MessageBoxImage.Warning);
			return false;
		}

		if (!Int64.TryParse(this.txtDni.Text, out _)) {
			MessageBox.Show($"Error: El dni no es un numero entero valido.", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
			return false;
		}

		if (!Double.TryParse(this.txtSueldoMinimoGarantizado.Text, out _)) {
			MessageBox.Show("Error: El sueldo minimo no es un número decimal válido. Use la coma (,) como separador decimal.", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
			return false;
		}
		if (this.txtDiasDeAtencion.ItemsSource is not List<HorarioMedico> result)
			return true;   // o yield break, o lo que corresponda

		foreach (HorarioMedico campo in (result)) {
			if (string.IsNullOrEmpty(campo.HoraInicio) && string.IsNullOrEmpty(campo.HoraFin)) {
				continue;
			}
			if (Utilidades.TryParseHoraField(campo.HoraInicio) && Utilidades.TryParseHoraField(campo.HoraFin)) {
				continue;
			} else {
				MessageBox.Show($"Error: No se reconoce el horario del día {campo.DiaSemana}. \nIngrese un string con formato valido (hh:mm)", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
			}
		}

		return true;
	}



	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (!CamposCompletadosCorrectamente()) {
			return;
		}

		//if (SelectedMedico is null) {
		//	SelectedMedico = new Medico(this);
		//	if (App.BaseDeDatos.CreateMedico(SelectedMedico)) {
		//		this.Cerrar();
		//	}
		//} else {
		//	SelectedMedico.LeerDesdeVentana(this);
		//	if (App.BaseDeDatos.UpdateMedico(SelectedMedico)) {
		//		this.Cerrar();
		//	}
		//}
	}


	//---------------------botones.Eliminar-------------------//
	private void ButtonEliminar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		//---------Checknulls-----------//
		if (SelectedMedico is null || SelectedMedico.Dni is null) {
			MessageBox.Show($"No hay item seleccionado.");
			return;
		}
		//---------confirmacion-----------//
		if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedMedico.Nombre}", "Confirmar Eliminación", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK) {
			return;
		}
		//---------Eliminar-----------//
		//if (App.BaseDeDatos.DeleteMedico(SelectedMedico)) {
		//	this.Cerrar(); // this.NavegarA<Medicos>();
		//}
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
