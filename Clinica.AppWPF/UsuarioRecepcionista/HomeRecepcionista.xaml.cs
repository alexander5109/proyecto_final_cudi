using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class HomeRecepcionista : Window {
	public string MensajeBienvenida { get; }
	public HomeRecepcionista() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;

		MensajeBienvenida = $"Bienvenid@ {App.UsuarioActivo?.Nombre ?? "Recepcionista"}";
		DataContext = this; // sencillo, no hace falta más
	}




	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);




	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
	private void ClickBoton_Logout(object sender, RoutedEventArgs e) => this.CerrarSesion();




	async private void ClickBoton_GestionTurnos(object sender, RoutedEventArgs e) {
		this.NavegarA<SecretariaTurnos>();
	}


	async private void ClickBoton_GestionPacientes(object sender, RoutedEventArgs e) {
		this.NavegarA<SecretariaPacientes>();
	}



}