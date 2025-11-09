using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;

namespace Clinica.AppWPF.Mappers;
public static class PacienteMapper {




	// Metodo para aplicarle los cambios de una ventana a una instancia de medico existente.
	public static void LeerDesdeVentana(Paciente paciente, PacientesModificar window) {
		paciente.Dni = window.txtDni.Text;
		paciente.Name = window.txtName.Text;
		paciente.LastName = window.txtLastName.Text;
		paciente.FechaIngreso = (DateTime)window.txtFechaIngreso.SelectedDate;
		paciente.Email = window.txtEmail.Text;
		paciente.Telefono = window.txtTelefono.Text;
		paciente.FechaNacimiento = (DateTime)window.txtFechaNacimiento.SelectedDate;
		paciente.Domicilio = window.txtDomicilio.Text;
		paciente.Localidad = window.txtLocalidad.Text;
		paciente.Provincia = window.txtProvincia.Text;
	}
	public static void MostrarEnVentana(Paciente paciente, PacientesModificar ventana) {
		ventana.txtDni.Text = paciente.Dni;
		ventana.txtName.Text = paciente.Name;
		ventana.txtLastName.Text = paciente.LastName;
		ventana.txtFechaIngreso.SelectedDate = paciente.FechaIngreso;
		ventana.txtEmail.Text = paciente.Email;
		ventana.txtTelefono.Text = paciente.Telefono;
		ventana.txtFechaNacimiento.SelectedDate = paciente.FechaNacimiento;
		ventana.txtDomicilio.Text = paciente.Domicilio;
		ventana.txtLocalidad.Text = paciente.Localidad;
		ventana.txtProvincia.Text = paciente.Provincia;
	}



	public static Result<Paciente2025> LeerDesdeVentana(PacientesModificar window) {

		var nombreResult = NombreCompleto2025.Crear(window.txtName.Text, window.txtLastName.Text);
		var dniResult = DniArgentino2025.Crear(window.txtDni.Text);
		var fechaNacimientoResult = FechaDeNacimiento2025.Crear(window.txtFechaNacimiento.SelectedDate!.Value);

		// Para Contacto2025 asumimos un método Crear que reciba email y teléfono
		var contactoResult = Contacto2025.Crear(window.txtEmail.Text, window.txtTelefono.Text);

		// Para DomicilioArgentino2025 asumimos Crear que reciba provincia, localidad y dirección
		var domicilioResult = DomicilioArgentino2025.Crear(
			window.txtProvincia.Text,
			window.txtLocalidad.Text,
			window.txtDomicilio.Text
		);

		// Si alguno falla, retornamos el primer error
		if (nombreResult is Result<NombreCompleto2025>.Error e1) return new Result<Paciente2025>.Error(e1.Mensaje);
		if (dniResult is Result<DniArgentino2025>.Error e2) return new Result<Paciente2025>.Error(e2.Mensaje);
		if (fechaNacimientoResult is Result<FechaDeNacimiento2025>.Error e3) return new Result<Paciente2025>.Error(e3.Mensaje);
		if (contactoResult is Result<Contacto2025>.Error e4) return new Result<Paciente2025>.Error(e4.Mensaje);
		if (domicilioResult is Result<DomicilioArgentino2025>.Error e5) return new Result<Paciente2025>.Error(e5.Mensaje);

		// Todos son válidos: construimos Paciente2025
		var paciente = new Paciente2025(
			((Result<NombreCompleto2025>.Ok)nombreResult).Value,
			((Result<DniArgentino2025>.Ok)dniResult).Value,
			((Result<Contacto2025>.Ok)contactoResult).Value,
			((Result<DomicilioArgentino2025>.Ok)domicilioResult).Value,
			((Result<FechaDeNacimiento2025>.Ok)fechaNacimientoResult).Value
		);

		return new Result<Paciente2025>.Ok(paciente);
	}

	public static void MostrarEnVentana(Paciente2025 paciente, PacientesModificar ventana) {
		ventana.txtName.Text = paciente.Nombre.Nombre;
		ventana.txtLastName.Text = paciente.Nombre.Apellido;
		ventana.txtDni.Text = paciente.Dni;
		ventana.txtFechaNacimiento.SelectedDate = paciente.FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue);
		ventana.txtEmail.Text = paciente.Contacto.Email;
		ventana.txtTelefono.Text = paciente.Contacto.Telefono;
		ventana.txtProvincia.Text = paciente.Domicilio.Localidad.Provincia.Nombre;
		ventana.txtLocalidad.Text = paciente.Domicilio.Localidad.Nombre;
		ventana.txtDomicilio.Text = paciente.Domicilio.Direccion;
	}








}
