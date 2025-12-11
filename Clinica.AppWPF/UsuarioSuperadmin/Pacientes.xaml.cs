using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioSuperadmin;

public partial class Pacientes : Window {
	private static TurnoDbModel? SelectedTurno;
	private static PacienteDbModel? SelectedPaciente;
	private static MedicoDbModel? RelatedMedico;

	public Pacientes() {
		InitializeComponent();
	}

	//----------------------ActualizarSecciones-------------------//
	async private void UpdatePacienteUI() {
		pacientesListView.ItemsSource = await App.Repositorio.SelectPacientes();
		buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
	}
	async private void UpdateTurnoUI() {
		turnosListView.ItemsSource = SelectedPaciente is not null? await App.Repositorio.SelectTurnosWherePacienteId(SelectedPaciente.Id): [];
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
	}
	async private void UpdateMedicoUI() {
		txtMedicoDni.Text = RelatedMedico?.Dni;
		txtMedicoNombre.Text = RelatedMedico?.Nombre;
		txtMedicoApellido.Text = RelatedMedico?.Apellido;
		txtMedicoEspecialidad.Text = RelatedMedico?.EspecialidadCodigo.ToString();
		buttonModificarMedico.IsEnabled = RelatedMedico != null;
	}




	//----------------------EventosRefresh-------------------//
	private void Window_Activated(object sender, EventArgs e) {
		//App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
		UpdatePacienteUI();
		UpdateTurnoUI();
		UpdateMedicoUI();
	}
	async private void listViewTurnos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = (TurnoDbModel)turnosListView.SelectedItem;
		RelatedMedico = SelectedTurno is not null? await App.Repositorio.SelectMedicoWhereId(SelectedTurno.MedicoId): null ;
		UpdatePacienteUI();
		UpdateTurnoUI();
		UpdateMedicoUI();
	}
	private void pacientesListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedPaciente = (PacienteDbModel)pacientesListView.SelectedItem;
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
		if (RelatedMedico != null) {
			this.AbrirComoDialogo<MedicosModificar>(RelatedMedico!);
		}
	}
	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (SelectedPaciente != null) {
			this.AbrirComoDialogo<PacientesModificar>(SelectedPaciente);
		}
	}





	//------------------botonesParaCrear------------------//
	private void ButtonAgregarMedico(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<MedicosModificar>();
	}
	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<PacientesModificar>(); // this.NavegarA<PacientesModificar>();
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
