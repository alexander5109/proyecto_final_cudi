using System;
using System.Net.Http.Headers;
using System.Windows;
using Clinica.Dominio.Comun;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF;


public partial class WindowLoginView2025 : Window {
	private readonly WindowLoginViewModel2025 _vm;

	public WindowLoginView2025() {
		InitializeComponent();
		_vm = new WindowLoginViewModel2025(App.ApiClient);
		DataContext = _vm;
	}

	private async void IniciarSesion_Click(object sender, RoutedEventArgs e) {
		_vm.Usuario = txtUsuario.Text;
		_vm.Password = txtPassword.Password;

		var result = await _vm.IntentarLoginAsync();

		if (result is Result<UsuarioLogueadoDTO>.Error err) {
			MessageBox.Show(err.Mensaje, "Error", MessageBoxButton.OK);
			return;
		}

		Result<UsuarioLogueadoDTO>.Ok ok = (Result<UsuarioLogueadoDTO>.Ok)result;
		UsuarioLogueadoDTO usuario = ok.Valor;

		// guardar token y configurar httpclient
		App.ApiClient.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", usuario.Token);

		App.UsuarioActual = usuario;
		this.Cerrar();
	}

	private void Salir_Click(object sender, RoutedEventArgs e) {
		this.Salir();
	}
}
