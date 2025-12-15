using System.Windows;

using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;


public partial class DialogoModificarHorarios : Window {
	public DialogoModificarHorariosVM VM { get; }

	public DialogoModificarHorarios(MedicoDbModel model) {
		InitializeComponent();
		VM = new DialogoModificarHorariosVM(model);
		DataContext = VM;
		Loaded += async (_, _) => await VM.CargarHorariosAsync(model.Id);
	}

	// ==========================================================
	// TREEVIEW
	// ==========================================================
	private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		=> VM.OnTreeSelectionChanged(e.NewValue);

	// ==========================================================
	// BOTONES CRUD
	// ==========================================================

	private void ClickBoton_AgregarHorario(object sender, RoutedEventArgs e)
		=> VM.AgregarHorario();

	private void ClickBoton_EditarHorario(object sender, RoutedEventArgs e)
		=> VM.AplicarCambios();

	private void ClickBoton_EliminarHorario(object sender, RoutedEventArgs e) {
		if (MessageBox.Show("¿Eliminar este horario?", "Confirmar",
				MessageBoxButton.YesNo, MessageBoxImage.Warning)
			== MessageBoxResult.Yes) {
			VM.EliminarHorario();
		}
	}

	private async void ClickBoton_GuardarCambios(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		var result = await VM.GuardarAsync();

		result.MatchAndDo(
			_ => MessageBox.Show("Cambios guardados.", "Éxito"),
			err => err.ShowMessageBox()
		);
	}

	// ==========================================================
	// NAV
	// ==========================================================

	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}
