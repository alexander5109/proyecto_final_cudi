using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF {
	public partial class MainWindow : Window {

		public MainWindow() {
			InitializeComponent();
			soundCheckBox.IsChecked = App.SoundOn;
		}
		public void MetodoBotonLogin(object sender, RoutedEventArgs e) {
			this.AbrirComoDialogo<WindowLogin>();
		}
		private void MetodoBotonMedicos(object sender, RoutedEventArgs e) {
			if (App.UsuarioActual?.Rol == "SECRETARY") {
				this.NavegarA<WindowListarMedicos>();
			} else {
				this.AbrirComoDialogo<WindowLogin>();
				if (App.UsuarioActual?.Rol == "SECRETARY") {
					this.NavegarA<WindowListarMedicos>();
				}
			}
		}

		private void MetodoBotonPacientes(object sender, RoutedEventArgs e) {
			if (App.UsuarioActual?.Rol == "SECRETARY") {
				this.NavegarA<WindowListarPacientes>();
			} else {
				this.AbrirComoDialogo<WindowLogin>();
				if (App.UsuarioActual?.Rol == "SECRETARY") {
					this.NavegarA<WindowListarPacientes>();
				}
			}
		}

		private void MetodoBotonTurnos(object sender, RoutedEventArgs e) {
			if (App.UsuarioActual?.Rol == "SECRETARY") {
				this.NavegarA<WindowListarTurnos>();
			} else {
				this.AbrirComoDialogo<WindowLogin>();
				if (App.UsuarioActual?.Rol == "SECRETARY") {
					this.NavegarA<WindowListarTurnos>();
				}
			}
		}

		private void Window_Activated(object sender, EventArgs e) {
			App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
		}

		private void ButtonSalir(object sender, RoutedEventArgs e) {
			this.Salir();
		}

		private void soundCheckBox_Checked(object sender, RoutedEventArgs e) {
			if (soundCheckBox.IsChecked == true) {
				App.SoundOn = true;
				App.PlayClickJewel();
			} else {
				App.SoundOn = false;
			}
		}

		private void MetodoBotonTurnos2025(object sender, RoutedEventArgs e) {
			if (App.UsuarioActual?.Rol == "SECRETARY") {
				this.NavegarA<WindowGestionTurno>();
			} else {
				this.AbrirComoDialogo<WindowLogin>();
				if (App.UsuarioActual?.Rol == "SECRETARY") {
					this.NavegarA<WindowGestionTurno>();
				}
			}

		}
	}
}