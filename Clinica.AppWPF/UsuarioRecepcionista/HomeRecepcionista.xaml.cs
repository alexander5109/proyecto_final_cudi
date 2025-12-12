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


	private async Task CargarPacientesYMedicosOnce() {
		await App.Repositorio.EnsureMedicosLoaded();
		await App.Repositorio.EnsurePacientesLoaded();
	}



	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) => SoundsService.ToggleSound(this.soundCheckBox.IsChecked);




	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
	private void MetodoBotonLogout(object sender, RoutedEventArgs e) => this.CerrarSesion();




	async private void MetodoBotonGestionTurnos(object sender, RoutedEventArgs e) {
		this.NavegarA<RecepcionistaGestionDeTurnos>();
		await CargarPacientesYMedicosOnce();
	}


	async private void MetodoBotonGestionPacientes(object sender, RoutedEventArgs e) {
		this.NavegarA<RecepcionistaGestionDePacientes>();
		await CargarPacientesYMedicosOnce();
	}



}