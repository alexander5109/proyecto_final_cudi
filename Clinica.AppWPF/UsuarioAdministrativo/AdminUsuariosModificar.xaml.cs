using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioAdministrativo;


public partial class AdminUsuariosModificar : Window {
	public AdminUsuariosModificar() {
		InitializeComponent();
	}



	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.NavegarA<AdminUsuarios>();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}
