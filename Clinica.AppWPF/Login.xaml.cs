using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;


namespace Clinica.AppWPF;

public partial class Login : Window {
	public Login() {
		InitializeComponent();
	}
	//public Login(Window previousWindow) {
	//	InitializeComponent();
	//}





	private bool _enCooldown;
	private async void ClickBoton_Refrescar(object sender, RoutedEventArgs e) {
		if (_enCooldown)
			return;
	}


	private async void ClickBoton_IniciarSesion(object sender, RoutedEventArgs e) {
		if (_enCooldown)
			return;

		try {
			_enCooldown = true;
			if (sender is Button btn)
				btn.IsEnabled = false;




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
					App.UsuarioActivo = await App.Repositorio.Usuarios.SelectUsuarioProfileWhereUsername(loggedUser.Username);
					this.IrARespectivaHome();
					return;
				},
				errorMsg => {
					errorMsg.ShowMessageBox();
					return;
				}
			);


		} finally {
			await Task.Delay(1000);
			if (sender is Button btn)
				btn.IsEnabled = true;

			_enCooldown = false;
		}



	}




	public void ClickBoton_Salir(object sender, RoutedEventArgs e) {
		this.Salir();
	}

	private void SoundCheckBox_Checked(object sender, RoutedEventArgs e) {
		SoundsService.ToggleSound(soundCheckBox.IsChecked);
	}
}
