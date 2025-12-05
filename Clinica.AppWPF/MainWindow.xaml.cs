using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.Pacientes;

namespace Clinica.AppWPF {
	public partial class MainWindow : Window {

		public MainWindow() {
			InitializeComponent();
			soundCheckBox.IsChecked = SoundsService.SoundOn;
		}
		public void MetodoBotonLogin(object sender, RoutedEventArgs e) {
			this.AbrirComoDialogo<WindowLogin>();
		}
		private void MetodoBotonMedicos(object sender, RoutedEventArgs e) {
			if (App.Api.UsuarioActual?.RolEnum < 2) {
				//this.NavegarA<WindowListarMedicos>();
			} else {
				this.AbrirComoDialogo<WindowLogin>();
				if (App.Api.UsuarioActual?.RolEnum < 2) {
					//this.NavegarA<WindowListarMedicos>();
				}
			}
		}

		private void MetodoBotonPacientes(object sender, RoutedEventArgs e) {
			if (App.Api.UsuarioActual?.RolEnum < 2) {
				this.NavegarA<PacientesVer>();
			} else {
				this.AbrirComoDialogo<WindowLogin>();
				if (App.Api.UsuarioActual?.RolEnum < 2) {
					this.NavegarA<PacientesVer>();
				}
			}
		}

		private void MetodoBotonTurnos(object sender, RoutedEventArgs e) {
			if (App.Api.UsuarioActual?.RolEnum < 2) {
				//this.NavegarA<WindowListarTurnos>();
			} else {
				this.AbrirComoDialogo<WindowLogin>();
				if (App.Api.UsuarioActual?.RolEnum < 2) {
					//this.NavegarA<WindowListarTurnos>();
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
			SoundsService.ToggleSound(this.soundCheckBox.IsChecked);
		}

		private void MetodoBotonTurnos2025(object sender, RoutedEventArgs e) {
			if (App.Api.UsuarioActual?.RolEnum < 2) {
				//this.NavegarA<WindowGestionTurno>();
			} else {
				this.AbrirComoDialogo<WindowLogin>();
				if (App.Api.UsuarioActual?.RolEnum < 2) {
					//this.NavegarA<WindowGestionTurno>();
				}
			}

		}
	}
}