using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class DialogoModificarUsuarios : Window {
	public DialogoUsuarioModificarVM VM { get; }

	// ==========================================================
	// CONSTRUCTORES
	// ==========================================================
	public DialogoModificarUsuarios() {
		InitializeComponent();
		VM = new();
		DataContext = VM;
		Loaded += async (_, __) => await VM.RefrescarMedicosAsync();
	}

	public DialogoModificarUsuarios(UsuarioDbModel model) {
		InitializeComponent();
		VM = new DialogoUsuarioModificarVM(model);
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
			VM.Id is not UsuarioId2025 idGood || (
			MessageBox.Show("¿Esta seguro que desea eliminar este usuario?",
			"Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.No)
		) return;

		ResultWpf<UnitWpf> result = await App.Repositorio.Usuarios.DeleteUsuarioWhereId(idGood);
		result.MatchAndDo(
			caseOk => {
				MessageBox.Show("Usuario eliminado.", "Éxito", MessageBoxButton.OK);
				this.Cerrar();
			},
			caseError => caseError.ShowMessageBox()
		);
	}

	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}