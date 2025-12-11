using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class RecepcionistaHome : Window {
	public string MensajeBienvenida { get; }
	public RecepcionistaHome() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;

		MensajeBienvenida = $"Bienvenid@ {App.UsuarioActivo?.Nombre ?? "Recepcionista"}";
		DataContext = this; // sencillo, no hace falta más
	}


	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);




	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
	private void MetodoBotonLogout(object sender, RoutedEventArgs e) => this.CerrarSesion();




	private void MetodoBotonGestionTurnos(object sender, RoutedEventArgs e) => this.NavegarA<RecepcionistaGestionDeTurnos>();

    private void MetodoBotonGestionPacientes(object sender, RoutedEventArgs e) => this.NavegarA<RecepcionistaGestionDePacientes>();
}