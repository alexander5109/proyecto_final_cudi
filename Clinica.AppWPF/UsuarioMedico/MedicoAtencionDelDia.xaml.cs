using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioAdministrativo;

namespace Clinica.AppWPF.UsuarioMedico;

public partial class MedicoAtencionDelDia : Window {
	public MedicoAtencionDelDiaVM VM { get; }

	public MedicoAtencionDelDia() {
		InitializeComponent();
		VM = new MedicoAtencionDelDiaVM();
		DataContext = VM;

		Loaded += async (_, __) => await VM.RefrescarMisTurnosAsync();
	}


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ClickBoton_Home(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}
