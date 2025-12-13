using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class SecretariaPacientesModificar : Window {
	public SecretariaPacientesModificarViewModel VM { get; private set; }

	public SecretariaPacientesModificar() {
		InitializeComponent();
		VM = new(new PacienteDbModel()); 
		DataContext = VM;
	}

	public SecretariaPacientesModificar(PacienteDbModel pacientedbModel) {
		InitializeComponent();
		VM = new(pacientedbModel); //cheap recycle.... maybe we implement the refresh button just in case?
		DataContext = VM;
	}

	// ==========================================================
	// BOTONES: PERSISTENCIA
	// ==========================================================

	private async void ButtonGuardar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		ResultWpf<UnitWpf> result = await VM.GuardarAsync();
		result.MatchAndDo(
			caseOk => MessageBox.Show("Cambios guardados.", "Éxito", MessageBoxButton.OK),
			caseError => caseError.ShowMessageBox()
		);
	}



	// ==========================================================
	// BOTONES: REFRESH
	// ==========================================================

	// I guess i could implement it on the viewmodel and call it through the button here. It's just a matter of calling refresh pacientes and selectonebyid (which we have)


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================

	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e)  => this.Cerrar();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e)
		=> this.Salir();

	private async void ButtonEliminar(object sender, RoutedEventArgs e) {
		if (
			VM.Id is not PacienteId idGood || (
			MessageBox.Show("¿Esta seguro que desea eliminar este paciente?",
			"Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.No)
		) return;

		ResultWpf<UnitWpf> result = await App.Repositorio.DeletePacienteWhereId(idGood);
		result.MatchAndDo(
			caseOk => {
				MessageBox.Show("PacienteExtensiones eliminado.", "Éxito", MessageBoxButton.OK);
				Close();
			},
			caseError => caseError.ShowMessageBox()
		);
	}

}
