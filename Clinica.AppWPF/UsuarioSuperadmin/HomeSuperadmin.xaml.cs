using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioSuperadmin;

public partial class HomeSuperadmin : Window {

	public HomeSuperadmin() {
		InitializeComponent();
		soundCheckBox.IsChecked = SoundsService.SoundOn;
	}
	private void Window_Activated(object sender, EventArgs e) {
		//App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
	}

	private void ButtonSalir(object sender, RoutedEventArgs e) {
		this.Salir();
	}

	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) {
		SoundsService.ToggleSound(this.soundCheckBox.IsChecked);
	}
	private void MetodoBotonLogout(object sender, RoutedEventArgs e) {

	}
}