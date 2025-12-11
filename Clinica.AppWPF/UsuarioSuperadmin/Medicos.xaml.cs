using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioSuperadmin;

public partial class Medicos : Window {
	private static MedicoDbModel? SelectedMedico = null;
	private static TurnoDbModel? SelectedTurno = null;
	private static PacienteDbModel? PacienteRelacionado = null;
	public Medicos() {
		InitializeComponent();
	}

	//----------------------ActualizarSecciones-------------------//
	async private void UpdateMedicoUI() {
		medicosListView.ItemsSource = await App.Repositorio.SelectMedicosWithHorarios();
		buttonModificarMedico.IsEnabled = SelectedMedico != null;
	}
	async private void UpdateTurnoUI() {
		turnosListView.ItemsSource = SelectedMedico is not null ? await App.Repositorio.SelectTurnosWhereMedicoId(SelectedMedico.Id) : [];
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
	}
	private void UpdatePacienteUI() {

		txtPacienteDni.Text = PacienteRelacionado?.Dni;
		txtPacienteNombre.Text = PacienteRelacionado?.Nombre;
		txtPacienteApellido.Text = PacienteRelacionado?.Apellido;
		txtPacienteEmail.Text = PacienteRelacionado?.Email;
		txtPacienteTelefono.Text = PacienteRelacionado?.Telefono;
		buttonModificarPaciente.IsEnabled = PacienteRelacionado != null;
	}



	//----------------------EventosRefresh-------------------//
	private void Window_Activated(object sender, EventArgs e) {
		//App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
		UpdateMedicoUI();
		UpdateTurnoUI();
		UpdatePacienteUI();
	}
	async private void listViewTurnos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = (TurnoDbModel)turnosListView.SelectedItem;
		PacienteRelacionado = SelectedTurno is not null? await App.Repositorio.SelectPacienteWhereId(SelectedTurno.PacienteId): null;
		UpdateMedicoUI();
		UpdateTurnoUI();
		UpdatePacienteUI();
	}
	async private void medicosListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedMedico = (MedicoDbModel)medicosListView.SelectedItem;
		UpdateMedicoUI();
		UpdateTurnoUI();
		UpdatePacienteUI();
	}




	//---------------------botonesDeModificar-------------------//
	private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
		if (SelectedTurno != null) {
			this.AbrirComoDialogo<TurnosModificar>(SelectedTurno);
		}
	}
	private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
		if (SelectedMedico != null) {
			this.AbrirComoDialogo<MedicosModificar>(SelectedMedico);
		}
	}
	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (PacienteRelacionado != null) {
			this.AbrirComoDialogo<PacientesModificar>(PacienteRelacionado);
		}
	}



	//------------------botonesParaCrear------------------//
	private void ButtonAgregarMedico(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<MedicosModificar>();
	}
	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<PacientesModificar>();
	}
	private void ButtonAgregarTurno(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<MedicosModificar>();
	}




	//---------------------botonesDeVolver-------------------//
	private void ButtonSalir(object sender, RoutedEventArgs e) {
		this.Salir();
	}
	private void ButtonHome(object sender, RoutedEventArgs e) {
		this.IrARespectivaHome();
	}
	//------------------------Fin.Medicos----------------------//
}
