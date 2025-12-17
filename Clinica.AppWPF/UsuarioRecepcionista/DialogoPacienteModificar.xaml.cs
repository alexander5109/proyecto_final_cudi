using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class DialogoPacienteModificar : Window {
	public DialogoPacienteModificarVM VM { get; private set; }

	// ==========================================================
	// CONSTRUCTORES
	// ==========================================================

	public DialogoPacienteModificar() {
		InitializeComponent();
		VM = new();
		DataContext = VM;
	}

	public DialogoPacienteModificar(PacienteDbModel pacientedbModel) {
		InitializeComponent();
		VM = new(pacientedbModel);
		DataContext = VM;
	}

	// ==========================================================
	// BOTONES: PERSISTENCIA
	// ==========================================================

	private async void ClickBoton_GuardarCambios(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		ResultWpf<UnitWpf> result = await VM.GuardarAsync();
		result.MatchAndDo(
			caseOk => MessageBox.Show("Cambios guardados.", "Éxito", MessageBoxButton.OK),
			caseError => caseError.ShowMessageBox()
		);
	}

	private async void ClickBoton_Eliminar(object sender, RoutedEventArgs e) {
		if (
			VM.Id is not PacienteId2025 idGood || (
			MessageBox.Show("¿Esta seguro que desea eliminar este paciente?",
			"Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.No)
		) return;

		ResultWpf<UnitWpf> result = await App.Repositorio.Pacientes.DeletePacienteWhereId(idGood);
		result.MatchAndDo(
			caseOk => {
				MessageBox.Show("PacienteExtensiones eliminado.", "Éxito", MessageBoxButton.OK);
				Close();
			},
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

	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();

}

