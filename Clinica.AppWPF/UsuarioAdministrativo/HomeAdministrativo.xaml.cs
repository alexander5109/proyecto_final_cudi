using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioRecepcionista;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class HomeAdministrativo : Window {
	public string MensajeBienvenida { get; }
	public HomeAdministrativo() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;

		MensajeBienvenida = $"Bienvenid@ {App.UsuarioActivo?.Nombre ?? "Recepcionista"}";
		DataContext = this; // sencillo, no hace falta más
	}

	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();

	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);

	private void MetodoBotonLogout(object sender, RoutedEventArgs e) => this.CerrarSesion();

	private void MetodoBotonGestionTurnos(object sender, RoutedEventArgs e) => this.NavegarA<RecepcionistaGestionDeTurnos>();

	private void MetodoBotonGestionPacientes(object sender, RoutedEventArgs e) => this.NavegarA<RecepcionistaGestionDePacientes>();
}