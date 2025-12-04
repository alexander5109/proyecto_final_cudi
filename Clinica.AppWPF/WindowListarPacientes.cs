using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF;

public partial class WindowListarPacientes : Window {
	private TurnoDto? SelectedTurno;
	private PacienteDto? SelectedPaciente;
	private MedicoDto? MedicoRelacionado;
	private bool pacientesCargados = false;
	public WindowListarPacientes() {
		InitializeComponent();
	}
	//----------------------ActualizarSecciones-------------------//
	private async Task UpdatePacienteUIAsync() {
		if (pacientesCargados) return; // evita reentrada
		try {
			pacientesListView.ItemsSource = await App.BaseDeDatos.SelectPacientes();
			buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
			pacientesCargados = true;
		} catch (Exception) {
			pacientesCargados = false;
		}
	}
	private async Task UpdateTurnosUIAsync() {
		if (pacientesListView.SelectedItem is PacienteDto paciente) {
			turnosListView.ItemsSource = await App.BaseDeDatos.SelectTurnosWherePacienteId(paciente.Id);
		}
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
	}
	private async Task UpdateMedicoUIAsync() {
		if (turnosListView.SelectedItem is TurnoDto turno) {
			MedicoRelacionado = await App.BaseDeDatos.SelectMedicoWhereId(turno.MedicoId);
			txtMedicoDni.Text = MedicoRelacionado?.Dni;
			txtMedicoNombre.Text = MedicoRelacionado?.Nombre;
			txtMedicoApellido.Text = MedicoRelacionado?.Apellido;
			txtMedicoEspecialidad.Text = MedicoRelacionado?.EspecialidadCodigoInterno.ToString();
			buttonModificarMedico.IsEnabled = MedicoRelacionado != null;
		}
	}

	//----------------------EventosRefresh-------------------//
	private async void Window_Loaded(object sender, RoutedEventArgs e) {
		await UpdatePacienteUIAsync();
	}

	private async void Window_ActivatedAsync(object sender, EventArgs e) {
		try {
			App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
			await UpdatePacienteUIAsync();
			await UpdateTurnosUIAsync();
			await UpdateMedicoUIAsync();
		} catch (Exception ex) {
			MessageBox.Show($"Error en Activated: {ex.Message}");
		}
	}
	private async void ListViewTurnos_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = (TurnoDto)turnosListView.SelectedItem;
		await UpdateMedicoUIAsync();
	}
	private async void ListViewPacientes_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedPaciente = (PacienteDto)pacientesListView.SelectedItem;
		await UpdateTurnosUIAsync();
		await UpdateMedicoUIAsync(); // opcional, si un turno estaba seleccionado
	}




	//---------------------botonesDeModificar-------------------//
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





	//------------------botonesParaCrear------------------//
	private void ButtonAgregarMedico(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<WindowModificarMedico>();
	}
	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<WindowModificarPaciente>();
	}
	private void ButtonAgregarTurno(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<WindowModificarTurno>();
	}



	//---------------------botonesDeVolver-------------------//
	private void ButtonSalir(object sender, RoutedEventArgs e) {
		this.Salir();
	}
	private void ButtonHome(object sender, RoutedEventArgs e) {
		this.VolverAHome();
	}
	//------------------------Fin.WindowListarMedicos----------------------//
}
