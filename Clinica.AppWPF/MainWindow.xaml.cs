using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Comun;
using static Clinica.Shared.Dtos.ApiDtos;


namespace Clinica.AppWPF;

public partial class MainWindow : Window {
	public MainWindow() {
		InitializeComponent();
	}

	private async void MetodoBotonIniciarSesion(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (string.IsNullOrEmpty(guiUsuario.Text) || string.IsNullOrEmpty(guiPassword.Password)) {
			MessageBox.Show("Complete todos los campos");
			return;
		}
		UsuarioLoginRequestDto loginRequest = new (guiUsuario.Text, guiPassword.Password);
		Result<UsuarioLoginResponseDto> result = await AuthService.LoginAsync(App.Api, loginRequest);

		result.Match(
			loggedUser => {
				App.Api.SetUsuario(loggedUser);
				this.VolverAHome();
			},
			errorMsg => {
				MessageBox.Show(errorMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		);
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
