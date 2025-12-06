using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaPacientes : Window {
	private TurnoDto? SelectedTurno;
	private PacienteDto? SelectedPaciente;
	private MedicoDto? MedicoRelacionado;

	public SecretariaPacientes() {
		InitializeComponent();
		_ = CargaInicialAsync();
	}

	//=============================================================
	// Carga Inicial de datos
	//=============================================================
	private async Task CargaInicialAsync() {
		pacientesListView.ItemsSource = await App.Repositorio.SelectPacientes();
	}

	//=============================================================
	// Actualización de UI
	//=============================================================
	private void ActualizarPacienteUI() {
		buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
	}

	private async Task ActualizarTurnosUIAsync() {
		if (SelectedPaciente != null) {
			turnosListView.ItemsSource = await App.Repositorio.SelectTurnosWherePacienteId(SelectedPaciente.Id);
		} else {
			turnosListView.ItemsSource = null;
		}

		buttonModificarTurno.IsEnabled = SelectedTurno != null;
	}


	//=============================================================
	// Eventos de ventana
	//=============================================================
	private async void Window_ActivatedAsync(object sender, EventArgs e) {
		//App.UpdateLabelDataBaseModo(labelBaseDeDatosModo);
		ActualizarPacienteUI();
		await ActualizarTurnosUIAsync();
	}

	private async void ListViewPacientes_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedPaciente = pacientesListView.SelectedItem as PacienteDto;
		await ActualizarTurnosUIAsync();
		ActualizarPacienteUI();
	}

	private async void ListViewTurnos_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = turnosListView.SelectedItem as TurnoDto;
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
	}

	//=============================================================
	// Botones de modificar
	//=============================================================
	private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
		if (SelectedTurno != null) {
			//this.AbrirComoDialogo<WindowModificarTurno>(SelectedTurno);
		}
	}

	private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
		if (MedicoRelacionado != null) {
			//this.AbrirComoDialogo<MedicoModificar>(MedicoRelacionado);
		}
	}

	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (SelectedPaciente != null) {
			this.AbrirComoDialogo<SecretariaPacienteModificar>(SelectedPaciente.Id);
		}
	}

	//=============================================================
	// Botones de crear
	//=============================================================
	//private void ButtonAgregarMedico(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<MedicoModificar>();
	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<SecretariaPacienteModificar>();
	//private void ButtonAgregarTurno(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<WindowModificarTurno>();

	//=============================================================
	// Botones de navegación
	//=============================================================
	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
	private void ButtonHome(object sender, RoutedEventArgs e) => this.VolverAHome();
}
