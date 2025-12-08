using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaHome : Window {
	public SecretariaHome() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;
	}

	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();

	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);

	private void MetodoBotonLogout(object sender, RoutedEventArgs e) => this.CerrarSesion();

	private void MetodoBotonGestionTurnos(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaGestionDeTurnos>();

}