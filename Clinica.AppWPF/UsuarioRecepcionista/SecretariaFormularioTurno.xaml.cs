using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class RecepcionistaDisponibilidades : Window {
	public RecepcionistaDisponibilidadesViewModel VM { get; }

	public RecepcionistaDisponibilidades(PacienteDbModel paciente) {
		InitializeComponent();
		VM = new RecepcionistaDisponibilidadesViewModel(paciente);
		DataContext = VM;
	}

	private async void ButtonConsultar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		await VM.RefreshDisponibilidadesAsync();
	}

	private void Click_AgendarNuevoTurno(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		if (VM.SelectedDisponibilidad is null) {
			MessageBox.Show("No hay disponibilidad seleccionada.");
			return;
		}

        Comodidades.DisponibilidadEspecialidadModelView d = VM.SelectedDisponibilidad;
		MessageBox.Show($"Reservando turno: {d.Fecha} {d.Hora}", "Reservar");
		Close();
	}

	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		if (VM.SelectedPaciente != null) {
			this.AbrirComoDialogo<RecepcionistaPacienteFormulario>(VM.SelectedPaciente.Id);
		}
	}

	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		Close();
	}

	private void ButtonSalir(object sender, RoutedEventArgs e)
		=> this.Salir();
}