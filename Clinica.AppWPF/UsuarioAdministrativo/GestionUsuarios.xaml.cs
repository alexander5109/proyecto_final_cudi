using System.Windows;
using Clinica.AppWPF.Infrastructure;
//using Clinica.AppWPF.Ventanas;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class GestionUsuarios : Window {
	public GestionUsuariosVM VM { get; }

	public GestionUsuarios() {
		InitializeComponent();
		VM = new GestionUsuariosVM();
		DataContext = VM;

		Loaded += async (_, __) => await VM.RefrescarUsuariosAsync();
	}


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ButtonHome(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
	private async void ButtonAgregarUsuario(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<DialogoModificarUsuarios>();
		await VM.RefrescarUsuariosAsync();
	}
	private async void ClickBoton_ModificarUsuario(object sender, RoutedEventArgs e) {
		if (VM.SelectedUsuario is not null) {
			this.AbrirComoDialogo<DialogoModificarUsuarios>(VM.SelectedUsuario);
			await VM.RefrescarUsuariosAsync();
		} else {
			MessageBox.Show("No hay usuario seleccionado. (este boton deberia estar desabilitado)");
		}
	}

	//async private void ClickBoton_ModificarUsuarioRoles(object sender, RoutedEventArgs e) {
	//	if (VM.SelectedUsuario is not null) {
	//		this.AbrirComoDialogo<DialogoModificarRoles>(VM.SelectedUsuario);
	//		await VM.RefrescarUsuariosAsync();
	//	} else {
	//		MessageBox.Show("No hay usuario seleccionado. (este boton deberia estar desabilitado)");
	//	}
	//}
}
