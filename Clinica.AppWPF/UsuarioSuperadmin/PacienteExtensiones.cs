using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioSuperadmin;


//---------------------------------Tablas.Pacientes-------------------------------//
public static class PacienteExtensiones {
	public static void LeerDesdeVentana(this PacienteDbModel? instance, PacientesModificar window) {
		if (instance is null) return;
		//instance.Dni = window.txtDni.Text;
		//instance.Name = window.txtName.Text;
		//instance.LastName = window.txtLastName.Text;
		//instance.FechaIngreso = (DateTime)window.txtFechaIngreso.SelectedDate;
		//instance.Email = window.txtEmail.Text;
		//instance.Telefono = window.txtTelefono.Text;
		//instance.FechaNacimiento = (DateTime)window.txtFechaNacimiento.SelectedDate;
		//instance.Domicilio = window.txtDomicilio.Text;
		//instance.Localidad = window.txtLocalidad.Text;
		//instance.ProvinciaEnum = window.txtProvincia.Text;
	}


	// Metodo para mostrarse en una ventana
	public static void MostrarseEnVentana(this PacienteDbModel? instance, PacientesModificar ventana) {
		if (instance is null) return;
		ventana.txtDni.Text = instance.Dni;
		ventana.txtName.Text = instance.Nombre;
		ventana.txtLastName.Text = instance.Apellido;
		ventana.txtFechaIngreso.SelectedDate = instance.FechaIngreso;
		ventana.txtEmail.Text = instance.Email;
		ventana.txtTelefono.Text = instance.Telefono;
		ventana.txtFechaNacimiento.SelectedDate = instance.FechaNacimiento;
		ventana.txtDomicilio.Text = instance.Domicilio;
		ventana.txtLocalidad.Text = instance.Localidad;
		ventana.txtProvincia.Text = instance.ProvinciaCodigo.ToString();
	}
}
