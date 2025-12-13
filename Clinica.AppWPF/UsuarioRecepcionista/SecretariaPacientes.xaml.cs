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

	// ==========================================================
	// BOTONES: SELECTED ITEM ACTIONS
	// ==========================================================
	private void Click_AgregarPaciente(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<SecretariaPacientesModificar>();
	private void ClickBoton_ModificarPaciente(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is not null) {
			this.AbrirComoDialogo<SecretariaPacientesModificar>(VM.SelectedPaciente);
		} else {
			MessageBox.Show("No hay paciente seleecionado. Pero este mensaje no deberia aparecer nunca porque el boton tendria que estar desabilitado.");
		}
	}
	private void ClickBoton_BuscarDisponibilidades(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is not null) {
			this.AbrirComoDialogo<SecretariaTurnosSacar>(VM.SelectedPaciente);
		} else {
			MessageBox.Show("No hay paciente seleecionado");
		}
	}

	// ==========================================================
	// BOTONES: REFRESH
	// ==========================================================

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


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
	private void ClickBoton_Home(object sender, RoutedEventArgs e) => this.IrARespectivaHome();


}