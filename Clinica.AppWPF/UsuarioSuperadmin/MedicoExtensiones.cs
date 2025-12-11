using Clinica.AppWPF.UsuarioSuperadmin;
using static Clinica.Shared.DbModels.DbModels;
using SystemTextJson = System.Text.Json;

namespace Clinica.AppWPF.UsuarioSuperadmin;
//---------------------------------Tablas.Horarios-------------------------------//
public class HorarioMedico {
	public string? DiaSemana { get; set; }
	public string? HoraInicio { get; set; }
	public string? HoraFin { get; set; }

	public static List<HorarioMedico> GetDiasDeLaSemanaAsList() {
		return [
				new() { DiaSemana = "Lunes" },
				new() { DiaSemana = "Martes" },
				new() { DiaSemana = "Miércoles" },
				new() { DiaSemana = "Jueves" },
				new() { DiaSemana = "Viernes" },
				new() { DiaSemana = "Sábado" },
				new() { DiaSemana = "Domingo" }
			];
	}
	public static Dictionary<string, HorarioMedico> GetDiasDeLaSemanaAsDict() {
		return new Dictionary<string, HorarioMedico> {
				{ "Lunes", new HorarioMedico { DiaSemana = "Lunes" } },
				{ "Martes", new HorarioMedico { DiaSemana = "Martes" } },
				{ "Miércoles", new HorarioMedico { DiaSemana = "Miércoles" } },
				{ "Jueves", new HorarioMedico { DiaSemana = "Jueves" } },
				{ "Viernes", new HorarioMedico { DiaSemana = "Viernes" } },
				{ "Sábado", new HorarioMedico { DiaSemana = "Sábado" } },
				{ "Domingo", new HorarioMedico { DiaSemana = "Domingo" } }
			};
	}
}

public static class MedicoExtensiones {

	//public static void LeerDesdeVentana(this MedicoDbModel? instance, MedicosModificar window) {
		//if (instance is null) return;
		//instance.Nombre = window.txtName.Text;
		//instance.LastName = window.txtLastName.Text;
		//instance.Dni = window.txtDni.Text;
		//instance.Telefono = window.txtTelefono.Text;
		//instance.Provincia = window.txtProvincia.Text;
		//instance.Domicilio = window.txtDomicilio.Text;
		//instance.Localidad = window.txtLocalidad.Text;
		//instance.Especialidad = window.txtEspecialidad.Text;
		//instance.FechaIngreso = (DateTime)window.txtFechaIngreso.SelectedDate;
		//instance.Guardia = (bool)window.txtGuardia.IsChecked;
		//this.DiasDeAtencion = //Al haber pasado los datos como List de HorariosMedicos, los objetos originales fueron modificados in-place. Assi que aca no hay que hacer nada.
	//}

	public static void MostrarseEnVentana(this MedicoDbModel? instance, MedicosModificar ventana) {
		if (instance is null) return;
		ventana.txtName.Text = instance.Nombre;
		ventana.txtLastName.Text = instance.Apellido;
		ventana.txtDni.Text = instance.Dni;
		ventana.txtTelefono.Text = instance.Telefono;
		ventana.txtProvincia.Text = instance.ProvinciaCodigo.ToString();
		ventana.txtDomicilio.Text = instance.Domicilio;
		ventana.txtLocalidad.Text = instance.Localidad;
		ventana.txtEspecialidad.Text = instance.EspecialidadCodigo.ToString();
		ventana.txtFechaIngreso.SelectedDate = instance.FechaIngreso;
		ventana.txtGuardia.IsChecked = instance.HaceGuardias;
		//ventana.txtDiasDeAtencion.ItemsSource = instance.DiasDeAtencion.Values.ToList();
	}



}
