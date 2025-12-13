using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class HomeAdministrativo : Window {
	public string MensajeBienvenida { get; }
	public HomeAdministrativo() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;

		MensajeBienvenida = $"Bienvenid@ {App.UsuarioActivo?.Nombre ?? "Recepcionista"}";
		DataContext = this; // sencillo, no hace falta más
	}

	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);



	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();

	private void ClickBoton_Logout(object sender, RoutedEventArgs e) => this.CerrarSesion();



	private void ClickBoton_CuentasDeUsuario(object sender, RoutedEventArgs e) => this.NavegarA<AdminUsuarios>();

	private void ClickBoton_GuestionPersonalMedico(object sender, RoutedEventArgs e) => this.NavegarA<AdminMedicos>();
}