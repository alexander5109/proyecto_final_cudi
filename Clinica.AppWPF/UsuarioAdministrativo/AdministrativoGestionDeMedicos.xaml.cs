using Clinica.AppWPF.Infrastructure;
using System.Windows;
using System.Windows.Controls;
using static Clinica.Shared.ApiDtos.MedicoDtos;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.ApiDtos.TurnoDtos;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class AdministrativoGestionDeMedicos : Window {
	private MedicoDto? SelectedMedico = null;
	private TurnoDto? SelectedTurno = null;
	private PacienteDto? PacienteRelacionado = null;

	public AdministrativoGestionDeMedicos() {
		InitializeComponent();
		_ = CargaInicialAsync();
	}

	//=============================================================
	// Carga Inicial de datos
	//=============================================================
	private async Task CargaInicialAsync() {
		await ActualizarMedicoUIAsync();
		await ActualizarTurnoUIAsync();
		//ActualizarPacienteUI();
	}

	//=============================================================
	// Actualización de UI
	//=============================================================
	private async Task ActualizarMedicoUIAsync() {
		//medicosListView.ItemsSource = await App.BaseDeDatos.SelectMedicos();
		//buttonModificarMedico.IsEnabled = SelectedMedico != null;
	}

	private async Task ActualizarTurnoUIAsync() {
		//if (SelectedMedico != null) {
		//	turnosListView.ItemsSource = await App.BaseDeDatos.SelectTurnosWhereMedicoId(SelectedMedico.Id);
		//} else {
		//	turnosListView.ItemsSource = null;
		//}

		//SelectedTurno = turnosListView.SelectedItem as TurnoDto;
		//buttonModificarTurno.IsEnabled = SelectedTurno != null;

		//if (SelectedTurno != null) {
		//	PacienteRelacionado = await App.BaseDeDatos.SelectPacienteWhereId(SelectedTurno.PacienteId);
		//} else {
		//	PacienteRelacionado = null;
		//}
	}


	//=============================================================
	// Eventos de ventana
	//=============================================================
	private async void Window_Activated(object sender, EventArgs e) {
		// App.UpdateLabelDataBaseModo(labelBaseDeDatosModo);
		await ActualizarMedicoUIAsync();
		await ActualizarTurnoUIAsync();
		//ActualizarPacienteUI();
	}

	private async void MedicosListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedMedico = medicosListView.SelectedItem as MedicoDto;
		await ActualizarMedicoUIAsync();
		await ActualizarTurnoUIAsync();
		//ActualizarPacienteUI();
	}

	private async void TurnosListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		//SelectedTurno = turnosListView.SelectedItem as TurnoDto;
		await ActualizarTurnoUIAsync();
		//ActualizarPacienteUI();
	}

	//=============================================================
	// Botones de modificar
	//=============================================================
	private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
		//if (SelectedTurno != null)
		//	this.AbrirComoDialogo<WindowModificarTurno>(SelectedTurno);
	}

	private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
		//if (SelectedMedico != null)
		//	this.AbrirComoDialogo<DialogoModificarMedico>(SelectedMedico);
	}

	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		//if (PacienteRelacionado != null)
		//	this.AbrirComoDialogo<WindowModificarPaciente>(PacienteRelacionado);
	}

	//=============================================================
	// Botones de crear
	//=============================================================
	//private void ButtonAgregarMedico(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<DialogoModificarMedico>();
	//private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<WindowModificarPaciente>();
	//private void ButtonAgregarTurno(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<WindowModificarTurno>();

	//=============================================================
	// Botones de navegación
	//=============================================================
	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();

    private void ButtonAgregarMedico(object sender, RoutedEventArgs e) {

    }

    private void ButtonHome(object sender, RoutedEventArgs e) {

    }
    //private void ButtonHome(object sender, RoutedEventArgs e) => this.VolverAHome();
}
