using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.AppWPF.UsuarioMedico;

public partial class MedicoAtencionDelDia : Window {
	public MedicoAtencionDelDiaVM VM { get; }

	public MedicoAtencionDelDia() {
		InitializeComponent();
		if (App.UsuarioActivo!.MedicoRelacionadoId is not MedicoId medicoIdGood) {
			MessageBox.Show("Su usuario no tiene un medico relacionado. Voy a crashear");
			throw new Exception("Su usuario no tiene un medico relacionado. Voy a crashear");
		}

		VM = new MedicoAtencionDelDiaVM(medicoIdGood);
		DataContext = VM;

		Loaded += async (_, __) => await VM.RefrescarMisTurnosAsync();
	}


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ClickBoton_Home(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}
