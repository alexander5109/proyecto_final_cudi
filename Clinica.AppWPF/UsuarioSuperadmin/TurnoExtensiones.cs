using Clinica.Dominio.TiposExtensiones;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioSuperadmin;
//---------------------------------Tablas.Turnos-------------------------------//
public static class TurnoExtensiones {
	public static void LeerDesdeVentana(this TurnoDbModel? instance, TurnosModificar window) {
		//this.Id = window.txtId.Content?.ToString() ?? this.Id;
		//this.PacienteId = window.txtPacientes.SelectedValue.ToString();
		//this.MedicoId = window.txtMedicos.SelectedValue.ToString();
		//instance.FechaHoraAsignadaDesde = window.txtFecha?.SelectedDate; // Set as DateTime, keeping only the date part
		//instance.FechaHoraAsignadaDesde = TimeSpan.Parse(window.txtHora.Text);
	}


	// Metodo para mostrarse en una ventana
	public static void MostrarseEnVentana(this TurnoDbModel? instance, TurnosModificar ventana) {
		if (instance is null) return;
		ventana.txtMedicos.SelectedValue = instance.MedicoId.Valor;
		ventana.txtPacientes.SelectedValue = instance.PacienteId.Valor;
		//ventana.txtEspecialidades.SelectedItem = instance.MedicoRelacionado?.EspecialidadEnumCodigo.ToString();
		ventana.txtId.Content = instance.Id.Valor;
		ventana.txtFecha.SelectedDate = instance.FechaHoraAsignadaDesde;
		ventana.txtHora.Text = instance.FechaHoraAsignadaDesde.ATextoHoras();
	}

}
