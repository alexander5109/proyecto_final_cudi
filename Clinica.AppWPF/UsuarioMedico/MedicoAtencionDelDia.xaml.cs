using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.AppWPF.UsuarioMedico;

public partial class MedicoAtencionDelDia : Window {
	public MedicoAtencionDelDiaVM VM { get; }

	public MedicoAtencionDelDia() {
		InitializeComponent();
		if (App.UsuarioActivo!.MedicoRelacionadoId is not MedicoId2025 medicoIdGood) {
			MessageBox.Show("Su usuario no tiene un medico relacionado. \n No tiene permisos para ver esta sección todavia. \n Consule personal administrativo.");
			this.CerrarSesion();
			//throw new Exception("Su usuario no tiene un medico relacionado. Voy a crashear");
		} else {

			VM = new MedicoAtencionDelDiaVM(medicoIdGood);
			DataContext = VM;

			Loaded += async (_, __) => await VM.RefrescarMisTurnosAsync();
		}
	}


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ClickBoton_Home(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();

	async private void ClickBoton_ConfirmarObservacion(object sender, RoutedEventArgs e) {
		var result = await VM.ConfirmarDiagnosticoAsync();
		result.MatchAndDo(
			async caseOk => {
				VM.Observaciones = null;
				await VM.CargarAtencionesDePacienteSeleccionado();
				await VM.RefrescarMisTurnosAsync();
				MessageBox.Show("Cambios guardados.", "Éxito", MessageBoxButton.OK);
			},
			caseError => caseError.ShowMessageBox()
		);


	}
}
