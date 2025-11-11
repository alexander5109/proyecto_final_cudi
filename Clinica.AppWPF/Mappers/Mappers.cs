using Clinica.Dominio.Entidades;

namespace Clinica.AppWPF.Mappers;
public static class PacienteMapper {




	// Metodo para aplicarle los cambios de una ventana a una instancia de medico existente.
	//public static void LeerDesdeVentana(Paciente paciente, PacientesModificar window) {
	//	paciente.Dni = window.txtDni.Text;
	//	paciente.Name = window.txtName.Text;
	//	paciente.LastName = window.txtLastName.Text;
	//	paciente.FechaIngreso = (DateTime)window.txtFechaIngreso.SelectedDate;
	//	paciente.Email = window.txtEmail.Text;
	//	paciente.Telefono = window.txtTelefono.Text;
	//	paciente.FechaNacimiento = (DateTime)window.txtFechaNacimiento.SelectedDate;
	//	paciente.Domicilio = window.txtDomicilio.Text;
	//	paciente.Localidad = window.txtLocalidad.Text;
	//	paciente.Provincia = window.txtProvincia.Text;
	//}
	//public static void MostrarEnVentana(Paciente paciente, PacientesModificar ventana) {
	//	ventana.txtDni.Text = paciente.Dni;
	//	ventana.txtName.Text = paciente.Name;
	//	ventana.txtLastName.Text = paciente.LastName;
	//	ventana.txtFechaIngreso.SelectedDate = paciente.FechaIngreso;
	//	ventana.txtEmail.Text = paciente.Email;
	//	ventana.txtTelefono.Text = paciente.Telefono;
	//	ventana.txtFechaNacimiento.SelectedDate = paciente.FechaNacimiento;
	//	ventana.txtDomicilio.Text = paciente.Domicilio;
	//	ventana.txtLocalidad.Text = paciente.Localidad;
	//	ventana.txtProvincia.Text = paciente.Provincia;
	//}

	//---------------------------------------2025models------------------------------------------//




	public static void MostrarEnVentana(Paciente2025EnDb pacienteResult, PacientesModificar ventana) {
	var ok = pacienteResult.Paciente;
		ventana.txtName.Text = ok.NombreCompleto.Nombre;
		ventana.txtLastName.Text = ok.NombreCompleto.Apellido;
		ventana.txtDni.Text = ok.Dni.Value;
		ventana.txtFechaNacimiento.SelectedDate = ok.FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue);
		ventana.txtEmail.Text = ok.Contacto.Email.Value;
		ventana.txtTelefono.Text = ok.Contacto.Telefono.Value;
		ventana.txtProvincia.Text = ok.Domicilio.Localidad.Provincia.Nombre;
		ventana.txtLocalidad.Text = ok.Domicilio.Localidad.Nombre;
		ventana.txtDomicilio.Text = ok.Domicilio.Direccion;
	}





}
