using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class AdminMedicos : Window {
	public AdminMedicosViewModel VM { get; }

	public AdminMedicos() {
		InitializeComponent();
		VM = new AdminMedicosViewModel();
		DataContext = VM;

		Loaded += async (_, __) => await VM.RefrescarMedicosAsync();
	}


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ButtonHome(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
	private void ButtonAgregarMedico(object sender, RoutedEventArgs e) => this.NavegarA<AdminMedicosModificar>();
	private void ClickBoton_ModificarMedico(object sender, RoutedEventArgs e) {
		if (VM.SelectedMedico is not null) {
			this.NavegarA<AdminMedicosModificar>(VM.SelectedMedico);
		} else {
			MessageBox.Show("No hay médico seleccionado. (este boton deberia estar desabilitado)");
		}
	}

}
