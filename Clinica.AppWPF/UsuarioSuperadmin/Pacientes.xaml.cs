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
		pacientesListView.ItemsSource = await App.Repositorio.Pacientes.SelectPacientes();
		x_BotonModificarPaciente.IsEnabled = SelectedPaciente != null;
	}
	async private void UpdateTurnoUI() {
		turnosListView.ItemsSource = SelectedPaciente is not null? await App.Repositorio.Turnos.SelectTurnosWherePacienteId(SelectedPaciente.Id): [];
		x_BotonModificarTurno.IsEnabled = SelectedTurno != null;
	}
	async private void UpdateMedicoUI() {
		txtMedicoDni.Text = RelatedMedico?.Dni;
		txtMedicoNombre.Text = RelatedMedico?.Nombre;
		txtMedicoApellido.Text = RelatedMedico?.Apellido;
		txtMedicoEspecialidad.Text = RelatedMedico?.EspecialidadCodigo.ToString();
		x_BotonModificarMedico.IsEnabled = RelatedMedico != null;
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
		RelatedMedico = SelectedTurno is not null? await App.Repositorio.Medicos.SelectMedicoWhereId(SelectedTurno.MedicoId): null ;
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
	private void ClickBoton_ModificarTurno(object sender, RoutedEventArgs e) {
		if (SelectedTurno != null) {
			this.AbrirComoDialogo<TurnosModificar>(SelectedTurno);
		}
	}
	private void ClickBoton_ModificarMedico(object sender, RoutedEventArgs e) {
		if (RelatedMedico != null) {
			this.AbrirComoDialogo<MedicosModificar>(RelatedMedico!);
		}
	}
	private void ClickBoton_ModificarPaciente(object sender, RoutedEventArgs e) {
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
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) {
		this.Salir();
	}
	private void ButtonHome(object sender, RoutedEventArgs e) {
		this.IrARespectivaHome();
	}
	//------------------------Fin.Medicos----------------------//
}
