using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaGeneral : Window {

	private PacienteDto? SelectedPaciente;
	private TurnoDto? SelectedTurno;

	private ICollectionView? pacientesView;

	public SecretariaGeneral() {
		InitializeComponent();
		_ = CargaInicialAsync();
	}

	// ---------------------------------------------------------
	//  CARGA INICIAL
	// ---------------------------------------------------------
	private async Task CargaInicialAsync() {
		var pacientes = await App.Repositorio.SelectPacientes();
		pacientesView = CollectionViewSource.GetDefaultView(pacientes);
		pacientesView.Filter = PacientesFilter;

		pacientesListView.ItemsSource = pacientesView;
	}

	// ---------------------------------------------------------
	//  FILTRADO
	// ---------------------------------------------------------
	private bool PacientesFilter(object obj) {
		if (obj is not PacienteDto p) return false;
		string filtroApellido = txtFilterApellido.Text.Trim().ToLower();
		string filtroNombre = txtFilterNombre.Text.Trim().ToLower();
		string filtroDni = txtFilterDni.Text.Trim().ToLower();
		//string filtroEmail = txtFilterEmail.Text.Trim().ToLower();
		//string filtroTelefono = txtFilterTelefono.Text.Trim().ToLower();

		bool c1 = string.IsNullOrEmpty(filtroApellido) || p.Apellido.ToLower().Contains(filtroApellido);
		bool c2 = string.IsNullOrEmpty(filtroNombre) || p.Nombre.ToLower().Contains(filtroNombre);
		bool c3 = string.IsNullOrEmpty(filtroDni) || p.Dni.Contains(filtroDni);
		//bool c4 = string.IsNullOrEmpty(filtroEmail) || p.Email.Contains(filtroEmail);
		//bool c5 = string.IsNullOrEmpty(filtroTelefono) || p.Telefono.Contains(filtroTelefono);

		return c1 && c2 && c3; // && c4 && c5;
	}

	private void FilterTextChanged(object sender, TextChangedEventArgs e) {
		pacientesView?.Refresh();
	}

	// ---------------------------------------------------------
	//  SELECCIÓN DE PACIENTES Y TURNOS
	// ---------------------------------------------------------
	private async void ListViewPacientes_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedPaciente = pacientesListView.SelectedItem as PacienteDto;
		await ActualizarTurnosUIAsync();
		ActualizarBotonesPaciente();
	}

	private async Task ActualizarTurnosUIAsync() {
		if (SelectedPaciente is null) {
			turnosListView.ItemsSource = null;
			return;
		}

		var turnos = await App.Repositorio.SelectTurnosWherePacienteId(SelectedPaciente.Id);
		turnosListView.ItemsSource = CollectionViewSource.GetDefaultView(turnos);
	}

	private void turnosListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = turnosListView.SelectedItem as TurnoDto;
	}

	// ---------------------------------------------------------
	//  ACTUALIZACIONES DE UI
	// ---------------------------------------------------------
	private void ActualizarBotonesPaciente() {
		buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
		buttonReservarTurno.IsEnabled = SelectedPaciente != null;
	}

	private async void Window_Activated(object sender, EventArgs e) {
		// Cada vez que vuelves de una ventana de edición, se refrescan datos
		await ActualizarTurnosUIAsync();
		pacientesView?.Refresh();
		ActualizarBotonesPaciente();
	}

	// ---------------------------------------------------------
	//  BOTONES
	// ---------------------------------------------------------

	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
	private void ButtonHome(object sender, RoutedEventArgs e) => this.VolverARespectivoHome();

	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<SecretariaPacientesModificar>();
	}

	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (SelectedPaciente is null) return;
		this.AbrirComoDialogo<SecretariaPacientesModificar>(SelectedPaciente.Id);
	}

	private void ButtonReservarTurno(object sender, RoutedEventArgs e) {
		if (SelectedPaciente is null) return;
		this.AbrirComoDialogo<SecretariaConsultaDisponibilidades>(SelectedPaciente);
	}
}
