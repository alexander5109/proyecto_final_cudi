using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class HomeRecepcionista : Window {
	public string MensajeBienvenida { get; set;  }
	public HomeRecepcionista() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;
		DataContext = this; 
		MensajeBienvenida = $"Bienvenid@ {App.UsuarioActivo?.Nombre ?? "Recepcionista"}";
	}


	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
	private void ClickBoton_Logout(object sender, RoutedEventArgs e) => this.CerrarSesion();
	private void ClickBoton_GestionTurnos(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaTurnos>();
	private void ClickBoton_GestionPacientes(object sender, RoutedEventArgs e) => this.NavegarA<GestionPacientes>();

}