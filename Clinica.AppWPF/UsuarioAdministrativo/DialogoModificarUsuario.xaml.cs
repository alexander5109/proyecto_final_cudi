using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioAdministrativo;


public partial class DialogoModificarUsuario : Window {
	public DialogoModificarUsuario() {
		InitializeComponent();
	}



	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();

    private void ClickBoton_IniciarSesion(object sender, RoutedEventArgs e) {

	}
}
