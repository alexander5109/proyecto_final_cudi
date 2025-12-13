using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class SecretariaPacientes : Window {
	public SecretariaPacientesViewModel VM { get; }

	public SecretariaPacientes() {
		InitializeComponent();
		VM = new SecretariaPacientesViewModel();
		DataContext = VM;

		Loaded += async (_, __) => await VM.RefrescarPacientesAsync();
	}

	private void ButtonHome(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();

	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<SecretariaPacientesModificar>();

	private void ClickBoton_ModificarPaciente(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is not null) {
			this.AbrirComoDialogo<SecretariaPacientesModificar>(VM.SelectedPaciente.Id);
		} else {
			MessageBox.Show("No hay paciente seleecionado");
		}
	}
	private void ButtonBuscarDisponibilidades(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is not null) {
			this.AbrirComoDialogo<SecretariaTurnosSacar>(VM.SelectedPaciente);
		} else {
			MessageBox.Show("No hay paciente seleecionado");
		}
	}




	private bool _enCooldown;
	private async void ClickBoton_Refrescar(object sender, RoutedEventArgs e) {
		if (_enCooldown)
			return;
		try {
			_enCooldown = true;
			if (sender is Button btn)
				btn.IsEnabled = false;
			await VM.RefrescarPacientesAsync();
		} finally {
			await Task.Delay(2000);
			if (sender is Button btn)
				btn.IsEnabled = true;

			_enCooldown = false;
		}
	}
}
