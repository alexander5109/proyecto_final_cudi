using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioAdministrativo;


public partial class DialogoModificarUsuarios : Window {
	public DialogoModificarUsuarios() {
		InitializeComponent();
	}



	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.NavegarA<GestionUsuarios>();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}
