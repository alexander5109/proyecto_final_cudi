using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioRecepcionista;

namespace Clinica.AppWPF.UsuarioMedico;

public partial class HomeMedico : Window {

	public HomeMedico() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;
	}


	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);

	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
	private void ClickBoton_Logout(object sender, RoutedEventArgs e) => this.CerrarSesion();

	private void ClickBoton_MisPacientes(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaPacientes>();

    private void MetodoMisTurnos(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaTurnos>();
}