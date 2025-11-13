using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clinica.AppWPF.Entidades;
public static class VentanaMappers {




	// Metodo para aplicarle los cambios de una ventana a una instancia de medico existente.
	public static void LeerDesdeVentana(this TurnoView turnoView, TurnosModificar window) {
		turnoView.Id = window.txtId.Content?.ToString() ?? turnoView.Id;
		turnoView.PacienteId = window.txtPacientes.SelectedValue.ToString();
		turnoView.MedicoId = window.txtMedicos.SelectedValue.ToString();
		turnoView.Fecha = window.txtFecha.SelectedDate.Value.Date; // Set as DateTime, keeping only the date part
		turnoView.Hora = TimeSpan.Parse(window.txtHora.Text);
	}


	// Metodo para mostrarse en una ventana
	public static void MostrarseEnVentana(this TurnoView turnoView, TurnosModificar ventana) {
		ventana.txtMedicos.SelectedValue = turnoView.MedicoId;
		ventana.txtPacientes.SelectedValue = turnoView.PacienteId;
		ventana.txtEspecialidades.SelectedItem = turnoView.MedicoRelacionado.Especialidad;
		ventana.txtId.Content = turnoView.Id;
		ventana.txtFecha.SelectedDate = turnoView.Fecha;
		ventana.txtHora.Text = turnoView.Hora.ToString();
	}



	// Metodo para aplicarle los cambios de una ventana a una instancia de medico existente.
	public static void LeerDesdeVentana(this PacienteView pacienteView, PacientesModificar window) {
		pacienteView.Dni = window.txtDni.Text;
		pacienteView.Name = window.txtName.Text;
		pacienteView.LastName = window.txtLastName.Text;
		pacienteView.FechaIngreso = (DateTime)window.txtFechaIngreso.SelectedDate;
		pacienteView.Email = window.txtEmail.Text;
		pacienteView.Telefono = window.txtTelefono.Text;
		pacienteView.FechaNacimiento = (DateTime)window.txtFechaNacimiento.SelectedDate;
		pacienteView.Domicilio = window.txtDomicilio.Text;
		pacienteView.Localidad = window.txtLocalidad.Text;
		pacienteView.Provincia = window.txtProvincia.Text;
	}


	// Metodo para mostrarse en una ventana
	public static void MostrarEnVentana(this PacientesModificar ventana, PacienteView pacienteView) {
		ventana.txtDni.Text = pacienteView.Dni;
		ventana.txtName.Text = pacienteView.Name;
		ventana.txtLastName.Text = pacienteView.LastName;
		ventana.txtFechaIngreso.SelectedDate = pacienteView.FechaIngreso;
		ventana.txtEmail.Text = pacienteView.Email;
		ventana.txtTelefono.Text = pacienteView.Telefono;
		ventana.txtFechaNacimiento.SelectedDate = pacienteView.	FechaNacimiento;
		ventana.txtDomicilio.Text = pacienteView.Domicilio;
		ventana.txtLocalidad.Text = pacienteView.Localidad;
		ventana.txtProvincia.Text = pacienteView.Provincia;
	}





}
