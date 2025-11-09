using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;

namespace Clinica.AppWPF.Mappers;
public static class PacienteMapper {
	public static Result<Paciente2025> LeerDesdeVentana(PacientesModificar w) {
		var nombre = NombreCompleto2025.Crear(w.txtName.Text, w.txtLastName.Text);
		var dni = DniArgentino2025.Crear(w.txtDni.Text);
		var contacto = Contacto2025.Crear(w.txtEmail.Text, w.txtTelefono.Text);
		var domicilio = DomicilioArgentino2025.Crear(w.txtDomicilio.Text, w.txtLocalidad.Text, w.txtProvincia.Text);
		var nacimiento = FechaDeNacimiento2025.Crear(w.txtFechaNacimiento.SelectedDate ?? DateTime.MinValue);

		// Si alguno falló, devolver su error
		if (!nombre.EsOk) return new Result<Paciente2025>.Error(nombre.Error);
		if (!dni.EsOk) return new Result<Paciente2025>.Error(dni.Error);
		if (!contacto.EsOk) return new Result<Paciente2025>.Error(contacto.Error);
		if (!domicilio.EsOk) return new Result<Paciente2025>.Error(domicilio.Error);

		return new Result<Paciente2025>.Ok(
			new Paciente2025(
				nombre.Valor,
				dni.Valor,
				contacto.Valor,
				domicilio.Valor,
				nacimiento
			)
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
