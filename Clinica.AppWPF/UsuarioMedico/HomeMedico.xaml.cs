using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioRecepcionista;

namespace Clinica.AppWPF.UsuarioMedico;

public partial class HomeMedico : Window {
	public string MensajeBienvenida { get; set; }

	public HomeMedico() {
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
    private void ClickBoton_AtencionDelDia(object sender, RoutedEventArgs e) => this.NavegarA<MedicoAtencionDelDia>();
}