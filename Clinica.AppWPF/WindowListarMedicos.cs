using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
namespace Clinica.AppWPF;

public partial class WindowListarMedicos : Window {
	private static WindowModificarMedicoViewModel? SelectedMedico = null;
	private static WindowModificarTurnoViewModel? SelectedTurno = null;
	public WindowListarMedicos() {
		InitializeComponent();
	}

	//----------------------ActualizarSecciones-------------------//
	private void UpdateMedicoUI() {
		//medicosListView.ItemsSource = App.BaseDeDatos.ReadMedicos();
		buttonModificarMedico.IsEnabled = SelectedMedico != null;
	}
	private void UpdateTurnoUI() {
		if (SelectedMedico != null && SelectedMedico.Id != null) {
			//turnosListView.ItemsSource = App.BaseDeDatos.ReadTurnosWhereMedicoId((int)SelectedMedico.Id);
		}
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
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
		UpdateMedicoUI();
		UpdateTurnoUI();
		UpdatePacienteUI();
	}
	private void listViewTurnos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = (WindowModificarTurnoViewModel)turnosListView.SelectedItem;
		UpdateMedicoUI();
		UpdateTurnoUI();
		UpdatePacienteUI();
	}
	private void medicosListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedMedico = (WindowModificarMedicoViewModel)medicosListView.SelectedItem;
		UpdateMedicoUI();
		UpdateTurnoUI();
		UpdatePacienteUI();
	}




	//---------------------botonesDeModificar-------------------//
	private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
		if (SelectedTurno != null) {
			this.AbrirComoDialogo<WindowModificarTurno>(SelectedTurno);
		}
	}
	private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
		if (SelectedMedico != null) {
			this.AbrirComoDialogo<WindowModificarMedico>(SelectedMedico);
		}
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
		this.AbrirComoDialogo<WindowModificarPaciente>();
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
