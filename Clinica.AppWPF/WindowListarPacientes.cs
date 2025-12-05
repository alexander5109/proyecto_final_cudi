using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF;

public partial class WindowListarPacientes : Window {
	private TurnoDto? SelectedTurno;
	private PacienteDto? SelectedPaciente;
	private MedicoDto? MedicoRelacionado;

	public WindowListarPacientes() {
		InitializeComponent();
		_ = CargaInicialAsync();
	}

	//=============================================================
	// Carga Inicial de datos
	//=============================================================
	private async Task CargaInicialAsync() {
		pacientesListView.ItemsSource = await App.BaseDeDatos.SelectPacientes();
	}

	//=============================================================
	// Actualización de UI
	//=============================================================
	private void ActualizarPacienteUI() {
		buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
	}

	private async Task ActualizarTurnosUIAsync() {
		if (SelectedPaciente != null) {
			turnosListView.ItemsSource = await App.BaseDeDatos.SelectTurnosWherePacienteId(SelectedPaciente.Id);
		} else {
			turnosListView.ItemsSource = null;
		}

		buttonModificarTurno.IsEnabled = SelectedTurno != null;
	}

	private async Task ActualizarMedicoUIAsync() {
		if (SelectedTurno != null) {
			MedicoRelacionado = await App.BaseDeDatos.SelectMedicoWhereId(SelectedTurno.MedicoId);

			txtMedicoDni.Text = MedicoRelacionado?.Dni ?? string.Empty;
			txtMedicoNombre.Text = MedicoRelacionado?.Nombre ?? string.Empty;
			txtMedicoApellido.Text = MedicoRelacionado?.Apellido ?? string.Empty;
			txtMedicoEspecialidad.Text = MedicoRelacionado?.EspecialidadCodigo.ToString();

			buttonModificarMedico.IsEnabled = MedicoRelacionado != null;
		} else {
			MedicoRelacionado = null;
			txtMedicoDni.Text = txtMedicoNombre.Text = txtMedicoApellido.Text = txtMedicoEspecialidad.Text = string.Empty;
			buttonModificarMedico.IsEnabled = false;
		}
	}

	//=============================================================
	// Eventos de ventana
	//=============================================================
	private async void Window_ActivatedAsync(object sender, EventArgs e) {
		App.UpdateLabelDataBaseModo(labelBaseDeDatosModo);
		ActualizarPacienteUI();
		await ActualizarTurnosUIAsync();
		await ActualizarMedicoUIAsync();
	}

	private async void ListViewPacientes_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedPaciente = pacientesListView.SelectedItem as PacienteDto;
		await ActualizarTurnosUIAsync();
		await ActualizarMedicoUIAsync();
		ActualizarPacienteUI();
	}

	private async void ListViewTurnos_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = turnosListView.SelectedItem as TurnoDto;
		await ActualizarMedicoUIAsync();
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
	}

	//=============================================================
	// Botones de modificar
	//=============================================================
	private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
		if (SelectedTurno != null) {
			this.AbrirComoDialogo<WindowModificarTurno>(SelectedTurno);
		}
	}

	private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
		if (MedicoRelacionado != null) {
			this.AbrirComoDialogo<WindowModificarMedico>(MedicoRelacionado);
		}
	}

	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (SelectedPaciente != null) {
			this.AbrirComoDialogo<WindowModificarPaciente>(SelectedPaciente);
		}
	}

	//=============================================================
	// Botones de crear
	//=============================================================
	private void ButtonAgregarMedico(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<WindowModificarMedico>();
	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<WindowModificarPaciente>();
	private void ButtonAgregarTurno(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<WindowModificarTurno>();

	//=============================================================
	// Botones de navegación
	//=============================================================
	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
	private void ButtonHome(object sender, RoutedEventArgs e) => this.VolverAHome();
}
