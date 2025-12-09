using System.Windows;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.Dtos.ApiDtos;


namespace Clinica.AppWPF;

public partial class Login : Window {
	public Login() {
		InitializeComponent();
	}
	//public Login(Window previousWindow) {
	//	InitializeComponent();
	//}

	private async void MetodoBotonIniciarSesion(object sender, RoutedEventArgs e) {
		//SoundsService.PlayClickSound();
		if (string.IsNullOrEmpty(guiUsuario.Text) || string.IsNullOrEmpty(guiPassword.Password)) {
			MessageBox.Show("Complete todos los campos");
			return;
		}
		UsuarioLoginRequestDto loginRequest = new (guiUsuario.Text, guiPassword.Password);
		//MessageBox.Show("Iniciando sesión...");
		ResultWpf<UsuarioLoginResponseDto> result = await AuthService.LoginAsync(App.Api, loginRequest);

		result.MatchAndDo(
			loggedUser => {
				App.Api.SetUsuario(loggedUser);
				this.IrARespectivaHome(loggedUser);
				return;
			},
			errorMsg => {
				errorMsg.ShowMessageBox();
				return;
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
