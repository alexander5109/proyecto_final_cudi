using System.Net.Http.Headers;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Comun;


namespace Clinica.AppWPF;

public partial class WindowLogin : Window {
	private readonly WindowLoginViewModel viewModel;
	public WindowLogin() {

		InitializeComponent();
		soundCheckBox.IsChecked = App.SoundOn;
		viewModel = new WindowLoginViewModel(Api.Cliente);
		DataContext = viewModel;
	}


	private async void MetodoBotonIniciarSesion(object sender, RoutedEventArgs e) {

		//if (checkboxJSON.IsChecked == true) {
		//App.BaseDeDatos = new BaseDeDatosJSON();
		//} else if ( ValidarCamposVacios() ) {
		//if (ValidarCamposVacios()) => App.BaseDeDatos = new BaseDeDatosWebAPI();
		//} else {
		//App.BaseDeDatos = new BaseDeDatosSQL($"Server={labelServidor.Text};Database=ClinicaMedica;User ID={guiUsuario.Text};Password={guiPassword.Text};");
		//App.UsuarioLogueado = App.BaseDeDatos.ConectadaExitosamente;
		//if (App.UsuarioLogueado) {
		//	this.Cerrar();
		//}

		viewModel.Usuario = guiUsuario.Text;
		viewModel.Password = guiPassword.Password;

		Result<UsuarioLogueadoDTO> result = await viewModel.IntentarLoginAsync();

		if (result is Result<UsuarioLogueadoDTO>.Error err) {
			MessageBox.Show(err.Mensaje, "Error", MessageBoxButton.OK);
			return;
		}

		Result<UsuarioLogueadoDTO>.Ok ok = (Result<UsuarioLogueadoDTO>.Ok)result;
		UsuarioLogueadoDTO usuario = ok.Valor;

		// guardar token y configurar httpclient
		Api.Cliente.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", usuario.Token);

		App.UsuarioActual = usuario;
		this.Cerrar();
	}

	public void MetodoBotonSalir(object sender, RoutedEventArgs e) {
		this.Salir();
	}

	private void MetodoBotonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar();
	}

	private void soundCheckBox_Checked(object sender, RoutedEventArgs e) {
		if (soundCheckBox.IsChecked == true) {
			App.SoundOn = true;
			App.PlayClickJewel();
		} else {
			App.SoundOn = false;
		}
	}
}
