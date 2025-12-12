using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;


namespace Clinica.AppWPF;

public partial class Login : Window {
	public Login() {
		InitializeComponent();
	}
	//public Login(Window previousWindow) {
	//	InitializeComponent();
	//}

	private async void ClickBoton_IniciarSesion(object sender, RoutedEventArgs e) {
		//SoundsService.PlayClickSound();
		if (string.IsNullOrEmpty(guiUsuario.Text) || string.IsNullOrEmpty(guiPassword.Password)) {
			MessageBox.Show("Complete todos los campos");
			return;
		}
		UsuarioLoginRequestDto loginRequest = new(guiUsuario.Text, guiPassword.Password);
		//MessageBox.Show("Iniciando sesión...");
		ResultWpf<UsuarioLoginResponseDto> result = await AuthService.LoginAsync(App.Api, loginRequest);

		result.MatchAndDo(
			async loggedUser => {
				App.Api.SetUsuario(loggedUser);
				App.UsuarioActivo = await App.Repositorio.SelectUsuarioProfileWhereUsername(new UserName(loggedUser.Username));
				this.IrARespectivaHome();
				return;
			},
			errorMsg => {
				errorMsg.ShowMessageBox();
				return;
			}
		);
	}




	public void ClickBoton_Salir(object sender, RoutedEventArgs e) {
		this.Salir();
	}

	private void SoundCheckBox_Checked(object sender, RoutedEventArgs e) {
		SoundsService.ToggleSound(soundCheckBox.IsChecked);
	}
}
