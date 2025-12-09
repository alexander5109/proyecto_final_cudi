using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Entidades;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaDisponibilidades : Window {
	public SecretariaDisponibilidadesViewModel VM { get; }

	public SecretariaDisponibilidades(PacienteId pacienteId) {
		InitializeComponent();
		VM = new SecretariaDisponibilidadesViewModel(pacienteId);
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

		var d = VM.SelectedDisponibilidad;
		MessageBox.Show($"Reservando turno: {d.Fecha} {d.Hora}", "Reservar");
		Close();
	}

	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		if (VM.SelectedPaciente != null) {
			this.AbrirComoDialogo<SecretariaPacienteFormulario>(VM.SelectedPaciente.Id);
		}
	}

	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		Close();
	}

	private void ButtonSalir(object sender, RoutedEventArgs e)
		=> this.Salir();
}