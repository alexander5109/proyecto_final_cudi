using System.Windows;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaHome : Window {

	public SecretariaHome() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;
	}
	/*
	public void MetodoBotonLogin(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<WindowLogin>();
	}
	private void MetodoBotonMedicos(object sender, RoutedEventArgs e) {
		if (App.Api.UsuarioActual?.RolEnum < 2) {
			//this.NavegarA<WindowListarMedicos>();
		} else {
			this.AbrirComoDialogo<WindowLogin>();
			if (App.Api.UsuarioActual?.RolEnum < 2) {
				//this.NavegarA<WindowListarMedicos>();
			}
		}
	}


	private void MetodoBotonTurnos(object sender, RoutedEventArgs e) {
		if (App.Api.UsuarioActual?.RolEnum < 2) {
			//this.NavegarA<WindowListarTurnos>();
		} else {
			this.AbrirComoDialogo<WindowLogin>();
			if (App.Api.UsuarioActual?.RolEnum < 2) {
				//this.NavegarA<WindowListarTurnos>();
			}
		}
	}
	*/

	private void Window_Activated(object sender, EventArgs e) {
		//App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
	}

	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();

	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);

	private void MetodoBotonLogout(object sender, RoutedEventArgs e) {
		//App.UsuarioActual = null;
		this.AbrirComoDialogo<MainWindow>();
	}




	private void MetodoBotonPacientes(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaPacientes>();

	private void MetodoBotonTurnos2025(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaGestorTurnos>();

    private void MetodoBotonTurnos(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaTurnosDelDia>();
}