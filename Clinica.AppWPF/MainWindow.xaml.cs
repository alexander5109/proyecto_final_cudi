using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioSecretaria;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Shared.Dtos;
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
				RedirigirAUsuarioHome(loggedUser);
			},
			errorMsg => {
				MessageBox.Show(errorMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		);
	}


	private void RedirigirAUsuarioHome(UsuarioLoginResponseDto user) {
		switch (user.EnumRole) {
			//case UsuarioEnumRole.Nivel1Superadmin:
			//	this.NavegarA<SuperaadminHome>();
			//	break;
			//case UsuarioEnumRole.Nivel2Administrativo:
			//	this.NavegarA<AdministrativoHome>();
			//	break;
			case UsuarioEnumRole.Nivel3Secretaria:
				this.NavegarA<SecretariaHome>();
				break;
			//case UsuarioEnumRole.Nivel4Medico:
			//	this.NavegarA<MedicoHome>();
			//	break;
			default:
				MessageBox.Show($"Rol de usuario >>{App.Api.UsuarioActual!.EnumRole}<<no reconocido o no soportado todavia.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				break;
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
