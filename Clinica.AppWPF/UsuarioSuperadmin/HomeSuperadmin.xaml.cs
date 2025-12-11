using System.Windows;
using Clinica.AppWPF.UsuarioSuperadmin;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioRecepcionista;

namespace Clinica.AppWPF.UsuarioSuperadmin;

public partial class HomeSuperadmin : Window {

	public HomeSuperadmin() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;
	}

	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);

	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
	private void MetodoBotonLogout(object sender, RoutedEventArgs e) => this.CerrarSesion();



	private void MetodoBotonGestionMedicos(object sender, RoutedEventArgs e) => this.NavegarA<Medicos>();
	private void MetodoBotonGestionTurnos(object sender, RoutedEventArgs e) => this.NavegarA<Turnos>();
	private void MetodoBotonGestionPacientes(object sender, RoutedEventArgs e) => this.NavegarA<Pacientes>();

    private void MetodoBotonGestionUsuarios(object sender, RoutedEventArgs e) {

    }
}