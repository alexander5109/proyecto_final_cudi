using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioSuperadmin;

public partial class Turnos : Window {
	private static TurnoDbModel? SelectedTurno = null;
	private static MedicoDbModel? MedicoRelacionado = null;
	private static PacienteDbModel? PacienteRelacionado = null;

	public Turnos() {
		InitializeComponent();
	}

	//----------------------ActualizarSecciones-------------------//
	async private void UpdateTurnoUI() {
		turnosListView.ItemsSource = await App.Repositorio.Turnos.SelectTurnos();
		x_BotonModificarPaciente.IsEnabled = SelectedTurno != null;
		txtCalendario.SelectedDate = SelectedTurno?.FechaHoraAsignadaDesde;
		txtCalendario.DisplayDate = SelectedTurno?.FechaHoraAsignadaDesde ?? DateTime.Today;
	}
	private void UpdateMedicoUI() {
		txtMedicoDni.Text = MedicoRelacionado?.Dni;
		txtMedicoNombre.Text = MedicoRelacionado?.Nombre;
		txtMedicoApellido.Text = MedicoRelacionado?.Apellido;
		txtMedicoEspecialidad.Text = MedicoRelacionado?.EspecialidadCodigo.ToString();
		x_BotonModificarMedico.IsEnabled = MedicoRelacionado != null;
	}
	private void UpdatePacienteUI() {
		txtPacienteDni.Text = PacienteRelacionado?.Dni;
		txtPacienteNombre.Text = PacienteRelacionado?.Nombre;
		txtPacienteApellido.Text = PacienteRelacionado?.Apellido;
		txtPacienteEmail.Text = PacienteRelacionado?.Email;
		txtPacienteTelefono.Text = PacienteRelacionado?.Telefono;
		x_BotonModificarPaciente.IsEnabled = PacienteRelacionado != null;
	}




	//----------------------EventosRefresh-------------------//
	private void Window_Activated(object sender, EventArgs e) {
		//App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
		UpdateTurnoUI();
		UpdateMedicoUI();
		UpdatePacienteUI();
	}
	private void listViewTurnos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = (TurnoDbModel)turnosListView.SelectedItem;
		UpdateTurnoUI();
		UpdateMedicoUI();
		UpdatePacienteUI();
	}




	//---------------------botonesDeModificar-------------------//
	private void ClickBoton_ModificarTurno(object sender, RoutedEventArgs e) {
		if (SelectedTurno != null) {
			this.AbrirComoDialogo<TurnosModificar>(SelectedTurno);
		}
	}
	private void ClickBoton_ModificarMedico(object sender, RoutedEventArgs e) {
		if (MedicoRelacionado != null) {
			this.AbrirComoDialogo<MedicosModificar>(MedicoRelacionado);
		}
	}
	private void ClickBoton_ModificarPaciente(object sender, RoutedEventArgs e) {
		if (PacienteRelacionado != null) {
			this.AbrirComoDialogo<PacientesModificar>(PacienteRelacionado);
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
		this.AbrirComoDialogo<TurnosModificar>();
	}





	//---------------------botonesDeVolver-------------------//
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) {
		this.Salir();
	}
	private void ButtonHome(object sender, RoutedEventArgs e) {
		this.IrARespectivaHome();
	}
	//------------------------Fin.Turnos----------------------//
}
