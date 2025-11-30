using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.ViewModels;
using static Clinica.Dominio.Dtos.ApiDtos;

namespace Clinica.AppWPF;

public partial class WindowListarPacientes : Window {
	private TurnoListDto? SelectedTurno; // instead of PacienteListDto?
	private PacienteListDto? SelectedPaciente; // instead of WindowModificarPacienteViewModel?
	private MedicoListDto? MedicoRelacionado; // instead of WindowModificarPacienteViewModel?
	private bool _isLoadingPacientes = false;
	//private bool IsBusy = false;
	public WindowListarPacientes() {
		InitializeComponent();
	}
	//----------------------ActualizarSecciones-------------------//
	private async Task UpdatePacienteUIAsync() {
		if (_isLoadingPacientes) return; // evita reentrada
		_isLoadingPacientes = true;
		try {
			//IsBusy = true;
			//List<WindowModificarPacienteViewModel> pacientes = await App.BaseDeDatos.ReadPacientes();
			List<PacienteListDto> pacientes = await Api.Cliente.GetFromJsonAsync<List<PacienteListDto>>("api/pacientes/list");
			pacientesListView.ItemsSource = pacientes;

			// Actualizar enabled/selected depende del ItemsSource y SelectedPaciente
			buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
		} catch (Exception ex) {
			// Manejo de errores centralizado: log / MessageBox
			MessageBox.Show($"Error cargando pacientes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		} finally {
			//IsBusy = false;
			_isLoadingPacientes = false;
		}
	}
	private async Task UpdateTurnosUIAsync() {
		if (pacientesListView.SelectedItem is PacienteListDto paciente) {
			turnosListView.ItemsSource =
				await Api.Cliente.GetFromJsonAsync<List<TurnoListDto>>(
					$"api/pacientes/{paciente.Id}/turnos"
				);
		}
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
	}
	private async Task UpdateMedicoUIAsync() {
		if (turnosListView.SelectedItem is TurnoListDto turno) {
			MedicoRelacionado =
				await Api.Cliente.GetFromJsonAsync<MedicoListDto>(
					$"api/medicos/{turno.MedicoId}"
				);

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
		SelectedTurno = (TurnoListDto)turnosListView.SelectedItem;
		await UpdateMedicoUIAsync();
	}
	private async void ListViewPacientes_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedPaciente = (PacienteListDto)pacientesListView.SelectedItem;
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
