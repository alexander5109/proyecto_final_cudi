using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Clinica.AppWPF;
public partial class WindowListarTurnos : Window {
	private static WindowModificarTurnoViewModel? SelectedTurno = null;

	public WindowListarTurnos() {
        WindowListarTurnosViewModel viewmodel = new();
		InitializeComponent();
	}

	//----------------------ActualizarSecciones-------------------//
	private void UpdateTurnoUI() {
		//turnosListView.ItemsSource = App.BaseDeDatos.ReadTurnos();
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
		txtCalendario.SelectedDate = SelectedTurno?.Fecha;
		txtCalendario.DisplayDate = SelectedTurno?.Fecha ?? DateTime.Today;
	}
	private void UpdateMedicoUI() {
		//txtMedicoDni.Text = SelectedTurno?.MedicoRelacionado?.Dni;
		//txtMedicoNombre.Text = SelectedTurno?.MedicoRelacionado?.Name;
		//txtMedicoApellido.Text = SelectedTurno?.MedicoRelacionado?.LastName;
		//txtMedicoEspecialidad.Text = SelectedTurno?.MedicoRelacionado?.EspecialidadCodigoInterno.ToString();
		//buttonModificarMedico.IsEnabled = SelectedTurno?.MedicoRelacionado != null;
	}
	private void UpdatePacienteUI() {
		//txtPacienteDni.Text = SelectedTurno?.PacienteRelacionado.Dni;
		//txtPacienteNombre.Text = SelectedTurno?.PacienteRelacionado.Name;
		//txtPacienteApellido.Text = SelectedTurno?.PacienteRelacionado.LastName;
		//txtPacienteEmail.Text = SelectedTurno?.PacienteRelacionado.Email;
		//txtPacienteTelefono.Text = SelectedTurno?.PacienteRelacionado.Telefono;
		//buttonModificarPaciente.IsEnabled = SelectedTurno?.PacienteRelacionado != null;
	}




	//----------------------EventosRefresh-------------------//
	private void Window_Activated(object sender, EventArgs e) {
		App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
		UpdateTurnoUI();
		UpdateMedicoUI();
		UpdatePacienteUI();
	}
	private void ListViewTurnos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = (WindowModificarTurnoViewModel)turnosListView.SelectedItem;
		UpdateTurnoUI();
		UpdateMedicoUI();
		UpdatePacienteUI();
	}




	//---------------------botonesDeModificar-------------------//
	private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
		if (SelectedTurno != null) {
			this.AbrirComoDialogo<WindowModificarTurno>(SelectedTurno);
		}
	}
	private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
		//if (SelectedTurno?.MedicoRelacionado != null) {
		//	this.AbrirComoDialogo<WindowModificarMedico>(SelectedTurno.MedicoRelacionado);
		//}
	}
	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		//if (SelectedTurno?.PacienteRelacionado != null) {
		//	this.AbrirComoDialogo<WindowModificarPaciente>(SelectedTurno.PacienteRelacionado);
		//}
	}



	//------------------botonesParaCrear------------------//
	private void ButtonAgregarMedico(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<WindowModificarMedico>();
	}
	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<WindowModificarPaciente>(); // this.NavegarA<WindowModificarPaciente>();
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
	//------------------------Fin.WindowListarTurnos----------------------//
}
