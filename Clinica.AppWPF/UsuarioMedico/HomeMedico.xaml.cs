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

	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
	private void MetodoBotonLogout(object sender, RoutedEventArgs e) => this.CerrarSesion();

	private void MetodoBotonMisPacientes(object sender, RoutedEventArgs e) => this.NavegarA<RecepcionistaGestionDePacientes>();

    private void MetodoMisTurnos(object sender, RoutedEventArgs e) => this.NavegarA<RecepcionistaGestionDeTurnos>();
}