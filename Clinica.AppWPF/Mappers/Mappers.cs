using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;

namespace Clinica.AppWPF.Mappers;
public static class PacienteMapper {
	public static Paciente LeerDesdeVentana(PacientesModificar w) {
		return new Paciente2025(
			Nombre: new NombreCompleto2025(w.txtName.Text, w.txtLastName.Text),
			Dni: new DniArgentino2025(w.txtDni.Text),
			Contacto: new Contacto2025(w.txtEmail.Text, w.txtTelefono.Text),
			Domicilio: new DomicilioArgentino2025(
				w.txtDomicilio.Text,
				w.txtLocalidad.Text,
				w.txtProvincia.Text),
			FechaNacimiento: new FechaDeNacimiento2025((DateTime)w.txtFechaNacimiento.SelectedDate)
		);
	}

	public static void MostrarseEnVentana(Paciente2025 paciente, PacientesModificar w) {
		w.txtName.Text = paciente.Nombre.Nombre;
		w.txtLastName.Text = paciente.Nombre.Apellido;
		w.txtDni.Text = paciente.Dni.ToString();
		w.txtEmail.Text = paciente.Contacto.Email;
		w.txtTelefono.Text = paciente.Contacto.Telefono;
		w.txtDomicilio.Text = paciente.Domicilio.Calle;
		w.txtLocalidad.Text = paciente.Domicilio.Localidad;
		w.txtProvincia.Text = paciente.Domicilio.Provincia;
		w.txtFechaNacimiento.SelectedDate = paciente.FechaNacimiento.Valor;
	}
}
