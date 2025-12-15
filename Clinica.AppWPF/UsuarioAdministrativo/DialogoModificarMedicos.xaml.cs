using System.Linq;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class DialogoModificarMedicos : Window {
	public DialogoMedicoModificarVM VM { get; }

	// ==========================================================
	// CONSTRUCTORES
	// ==========================================================
	public DialogoModificarMedicos() {
		InitializeComponent();
		VM = new DialogoMedicoModificarVM(new MedicoDbModel());
		DataContext = VM;
	}

	public DialogoModificarMedicos(MedicoDbModel model) {
		InitializeComponent();
		VM = new DialogoMedicoModificarVM(model);
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
			VM.Id is not MedicoId idGood || (
			MessageBox.Show("¿Esta seguro que desea eliminar este paciente?",
			"Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.No)
		) return;

		ResultWpf<UnitWpf> result = await App.Repositorio.DeleteMedicoWhereId(idGood);
		result.MatchAndDo(
			caseOk => {
				MessageBox.Show("PacienteExtensiones eliminado.", "Éxito", MessageBoxButton.OK);
				Close();
			},
			caseError => caseError.ShowMessageBox()
		);
	}

	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}