using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Clinica.AppWPF;

public partial class WindowListarPacientes : Window {
	private static WindowModificarTurnoViewModel? SelectedTurno;
	private static WindowModificarPacienteViewModel? SelectedPaciente;
	private bool _isLoadingPacientes = false;
	private bool IsBusy = false;
	public WindowListarPacientes() {
		InitializeComponent();
	}

	//----------------------ActualizarSecciones-------------------//
	private async Task UpdatePacienteUIAsync() {
		if (_isLoadingPacientes) return; // evita reentrada
		_isLoadingPacientes = true;
		try {
			// Si querés mostrar un spinner:
			IsBusy = true;
			var pacientes = await App.BaseDeDatos.ReadPacientes();
			pacientesListView.ItemsSource = pacientes;

			// Actualizar enabled/selected depende del ItemsSource y SelectedPaciente
			buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
		} catch (Exception ex) {
			// Manejo de errores centralizado: log / MessageBox
			MessageBox.Show($"Error cargando pacientes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		} finally {
			IsBusy = false;
			_isLoadingPacientes = false;
		}
	}
	private void UpdateTurnoUI() {
		if (SelectedPaciente != null && SelectedPaciente.Id != null) {
			//turnosListView.ItemsSource = App.BaseDeDatos.ReadTurnosWherePacienteId((int)SelectedPaciente.Id);
		}
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
	}
	private void UpdateMedicoUI() {
		//txtMedicoDni.Text = SelectedTurno?.MedicoRelacionado.Dni;
		//txtMedicoNombre.Text = SelectedTurno?.MedicoRelacionado.Name;
		//txtMedicoApellido.Text = SelectedTurno?.MedicoRelacionado.LastName;
		//txtMedicoEspecialidad.Text = SelectedTurno?.MedicoRelacionado?.EspecialidadCodigoInterno.ToString();
		//buttonModificarMedico.IsEnabled = SelectedTurno?.MedicoRelacionado != null;
	}




	//----------------------EventosRefresh-------------------//
	private async void Window_ActivatedAsync(object sender, EventArgs e) {
		try {
			App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);

			// Espera a que terminen las cargas (si hay varias, puedes hacerlas en paralelo)
			await UpdatePacienteUIAsync();

			// Si UpdateTurnoUI y UpdateMedicoUI son síncronas, se llaman normalmente:
			UpdateTurnoUI();
			UpdateMedicoUI();
		} catch (Exception ex) {
			// Atrapá excepciones aquí (async void propaga excepciones al SyncContext si no se capturan)
			MessageBox.Show($"Error en Activated: {ex.Message}");
		}
	}
	private async void ListViewTurnos_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = (WindowModificarTurnoViewModel)turnosListView.SelectedItem;
		await UpdatePacienteUIAsync();
		UpdateTurnoUI();
		UpdateMedicoUI();
	}
	private async void ListViewPacientes_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedPaciente = (WindowModificarPacienteViewModel)pacientesListView.SelectedItem;
		UpdateMedicoUI();
		UpdateTurnoUI();
		await UpdatePacienteUIAsync();
	}




	//---------------------botonesDeModificar-------------------//
	private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
		if (SelectedTurno != null) {
			this.AbrirComoDialogo<WindowModificarTurno>(SelectedTurno);
		}
	}
	private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
		//if (SelectedTurno?.MedicoRelacionado != null) {
		//	this.AbrirComoDialogo<WindowModificarMedico>(SelectedTurno?.MedicoRelacionado!);
		//}
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
		this.AbrirComoDialogo<WindowModificarPaciente>(); // this.NavegarA<WindowModificarPaciente>();
	}
	private void ButtonAgregarTurno(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<WindowModificarMedico>();
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
