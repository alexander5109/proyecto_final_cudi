using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioRecepcionista;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class AdminMedicos : Window {
	public AdminMedicosViewModel VM { get; }

	public AdminMedicos() {
		InitializeComponent();
		VM = new AdminMedicosViewModel();
		DataContext = VM;

		Loaded += async (_, __) => await CargaInicialAsync();
	}

	private async Task CargaInicialAsync() {
		await VM.RefrescarMedicosAsync();
	}

	private void ButtonHome(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();

	private void ButtonAgregarMedico(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<AdminMedicosModificar>();

	private void ClickBoton_ModificarMedico(object sender, RoutedEventArgs e) {
		if (VM.SelectedMedico is not null) {
			this.AbrirComoDialogo<AdminMedicosModificar>(VM.SelectedMedico.Id);
		} else {
			MessageBox.Show("No hay paciente seleecionado");
		}
	}

}
