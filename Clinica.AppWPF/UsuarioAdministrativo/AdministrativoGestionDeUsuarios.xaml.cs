using Clinica.AppWPF.Infrastructure;
using System.Windows;
using System.Windows.Controls;
using static Clinica.Shared.ApiDtos.MedicoDtos;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.ApiDtos.TurnoDtos;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class AdministrativoGestionDeUsuarios : Window {
	//private MedicoDto? SelectedMedico = null;
	//private TurnoDto? SelectedTurno = null;
	//private PacienteDto? PacienteRelacionado = null;

	public AdministrativoGestionDeUsuarios() {
		InitializeComponent();
		//_ = CargaInicialAsync();
	}

	//=============================================================
	// Carga Inicial de datos
	//=============================================================
	private async Task CargaInicialAsync() {
		//await ActualizarMedicoUIAsync();
		//await ActualizarTurnoUIAsync();
		//ActualizarPacienteUI();
	}

	//=============================================================
	// Actualización de UI
	//=============================================================
	private async Task ActualizarMedicoUIAsync() {
		//medicosListView.ItemsSource = await App.BaseDeDatos.SelectMedicos();
		//ClickBoton_ModificarMedico.IsEnabled = SelectedMedico != null;
	}

	private async Task ActualizarTurnoUIAsync() {
		//if (SelectedMedico != null) {
		//	turnosListView.ItemsSource = await App.BaseDeDatos.SelectTurnosWhereMedicoId(SelectedMedico.Id);
		//} else {
		//	turnosListView.ItemsSource = null;
		//}

		//SelectedTurno = turnosListView.SelectedItem as TurnoDto;
		//ClickBoton_ModificarTurno.IsEnabled = SelectedTurno != null;

		//if (SelectedTurno != null) {
		//	PacienteRelacionado = await App.BaseDeDatos.SelectPacienteWhereId(SelectedTurno.PacienteId);
		//} else {
		//	PacienteRelacionado = null;
		//}
	}

	private void ActualizarPacienteUI() {
		//txtPacienteDni.Text = PacienteRelacionado?.Dni ?? string.Empty;
		//txtPacienteNombre.Text = PacienteRelacionado?.Nombre ?? string.Empty;
		//txtPacienteApellido.Text = PacienteRelacionado?.Apellido ?? string.Empty;
		//txtPacienteEmail.Text = PacienteRelacionado?.Email ?? string.Empty;
		//txtPacienteTelefono.Text = PacienteRelacionado?.Telefono ?? string.Empty;
		//ClickBoton_ModificarPaciente.IsEnabled = PacienteRelacionado != null;
	}

	//=============================================================
	// Eventos de ventana
	//=============================================================
	private async void Window_Activated(object sender, EventArgs e) {
		// App.UpdateLabelDataBaseModo(labelBaseDeDatosModo);
		//await ActualizarMedicoUIAsync();
		//await ActualizarTurnoUIAsync();
		//ActualizarPacienteUI();
	}

	private async void MedicosListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		//SelectedMedico = medicosListView.SelectedItem as MedicoDto;
		//await ActualizarMedicoUIAsync();
		//await ActualizarTurnoUIAsync();
		//ActualizarPacienteUI();
	}

	private async void TurnosListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		//SelectedTurno = turnosListView.SelectedItem as TurnoDto;
		//await ActualizarTurnoUIAsync();
		//ActualizarPacienteUI();
	}

	//=============================================================
	// Botones de modificar
	//=============================================================
	private void ClickBoton_ModificarTurno(object sender, RoutedEventArgs e) {
		//if (SelectedTurno != null)
		//	this.AbrirComoDialogo<WindowModificarTurno>(SelectedTurno);
	}

	private void ClickBoton_ModificarMedico(object sender, RoutedEventArgs e) {
		//if (SelectedMedico != null)
		//	this.AbrirComoDialogo<DialogoModificarMedico>(SelectedMedico);
	}

	private void ClickBoton_ModificarPaciente(object sender, RoutedEventArgs e) {
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

    private void ButtonAgregarMedico(object sender, RoutedEventArgs e) {

    }






	private void ButtonHome(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}
