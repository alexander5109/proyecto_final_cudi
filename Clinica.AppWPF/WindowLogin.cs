using System.Net.Http.Headers;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;


namespace Clinica.AppWPF;

public partial class WindowLogin : Window {
	public WindowLogin() {
		InitializeComponent();
	}

	private async void MetodoBotonIniciarSesion(object sender, RoutedEventArgs e) {
		if (string.IsNullOrEmpty(guiUsuario.Text) || string.IsNullOrEmpty(guiPassword.Password)){
			MessageBox.Show("Complete todos los campos");
			return;
		}

		var result = await AuthService.LoginAsync(App.ApiClient, guiUsuario.Text, guiPassword.Password)

		if (result.IsError) {
			MessageBox.Show(result.UnwrapAsError(), "Error", MessageBoxButton.OK);
			return;
		} else {
			SoundsService.PlayClickSound();
			this.Cerrar();
		}
	}

	public void MetodoBotonSalir(object sender, RoutedEventArgs e) {
		this.Salir();
	}

	private void MetodoBotonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar();
	}
	private void SoundCheckBox_Checked(object sender, RoutedEventArgs e) {
		SoundsService.ToggleSound(soundCheckBox.IsChecked);
	}
}
