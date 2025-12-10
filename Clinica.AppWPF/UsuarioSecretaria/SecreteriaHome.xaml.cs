using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaHome : Window {
	public string MensajeBienvenida { get; }
	public SecretariaHome() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;

		MensajeBienvenida = $"Bienvenid@ {App.UsuarioActivo?.Nombre ?? "Recepcionista"}";
		DataContext = this; // sencillo, no hace falta más
	}

	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();

	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);

	private void MetodoBotonLogout(object sender, RoutedEventArgs e) => this.CerrarSesion();

	private void MetodoBotonGestionTurnos(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaGestionDeTurnos>();

    private void MetodoBotonGestionPacientes(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaGestionDePacientes>();
}