using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class GestionPacientes : Window {
	public GestionPacientesVM VM { get; }

	public GestionPacientes() {
		InitializeComponent();
		VM = new GestionPacientesVM();
		DataContext = VM;
		Loaded += async (_, __) => await VM.RefrescarPacientesAsync();
	}

	// ==========================================================
	// BOTONES: SELECTED ITEM ACTIONS
	// ==========================================================
	private async void Click_AgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<DialogoPacienteModificar>();
		await VM.RefrescarPacientesAsync();
	}
	private async void ClickBoton_ModificarPaciente(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is not null) {
			this.AbrirComoDialogo<DialogoPacienteModificar>(VM.SelectedPaciente);
			await VM.RefrescarPacientesAsync();
		} else {
			MessageBox.Show("No hay paciente seleecionado. Pero este mensaje no deberia aparecer nunca porque el boton tendria que estar desabilitado.");
		}
	}
	private void ClickBoton_BuscarDisponibilidades(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is not null) {
			this.AbrirComoDialogo<DialogoTurnoProgramar>(VM.SelectedPaciente);
		} else {
			MessageBox.Show("No hay paciente seleecionado");
		}
	}


	// ==========================================================
	// BOTONES: ORDENAR
	// ==========================================================
	private GridViewColumnHeader? _ultimaColumnaClicked = null;
	private ListSortDirection _ultimaDireccion = ListSortDirection.Ascending;

	private void ClickCabecera_OrdenarFilas(object sender, RoutedEventArgs e) {
		if (sender is not GridViewColumnHeader header || header.Tag == null) return;

		string sortBy = header.Tag.ToString()!;
		ListSortDirection direction = ListSortDirection.Ascending;

		if (_ultimaColumnaClicked == header && _ultimaDireccion == ListSortDirection.Ascending)
			direction = ListSortDirection.Descending;

		VM.PacientesView.SortDescriptions.Clear();
		VM.PacientesView.SortDescriptions.Add(new SortDescription(sortBy, direction));
		VM.PacientesView.Refresh();

		_ultimaColumnaClicked = header;
		_ultimaDireccion = direction;
	}



	// ==========================================================
	// BOTONES: REFRESH
	// ==========================================================

	private bool _enCooldown;
	private async void ClickBoton_Refrescar(object sender, RoutedEventArgs e) {
		if (_enCooldown)
			return;
		try {
			_enCooldown = true;
			if (sender is Button btn)
				btn.IsEnabled = false;
			await VM.RefrescarPacientesAsync();
		} finally {
			await Task.Delay(2000);
			if (sender is Button btn)
				btn.IsEnabled = true;

			_enCooldown = false;
		}
	}


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
	private void ClickBoton_Home(object sender, RoutedEventArgs e) => this.IrARespectivaHome();


}