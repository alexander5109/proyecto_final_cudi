using Clinica.AppWPF.Infrastructure;
using System.Windows;
using System.Windows.Controls;

namespace Clinica.AppWPF;

public partial class WindowListarTurnos : Window {
	private static TurnoDto? SelectedTurno = null;
	private static MedicoDto? MedicoRelacionado = null;
	private static PacienteDto? PacienteRelacionado = null;

	public WindowListarTurnos() {
		InitializeComponent();
	}

	//----------------------ActualizarSecciones-------------------//
	private void UpdateTurnoUI() {
		buttonModificarTurno.IsEnabled = SelectedTurno != null;
		txtCalendario.SelectedDate = SelectedTurno?.FechaHoraAsignadaDesde;
		txtCalendario.DisplayDate = SelectedTurno?.FechaHoraAsignadaDesde ?? DateTime.Today;
	}
	private void UpdateMedicoUI() {
		txtMedicoDni.Text = MedicoRelacionado?.Dni;
		txtMedicoNombre.Text = MedicoRelacionado?.Nombre;
		txtMedicoApellido.Text = MedicoRelacionado?.Apellido;
		txtMedicoEspecialidad.Text = MedicoRelacionado?.EspecialidadCodigo.ToString();
		buttonModificarMedico.IsEnabled = MedicoRelacionado != null;
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
	async private void Window_Loaded(object sender, EventArgs e) {
		turnosListView.ItemsSource = await App.BaseDeDatos.SelectTurnos();
	}
	async private void Window_Activated(object sender, EventArgs e) {
		// App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
		UpdateTurnoUI();
		UpdateMedicoUI();
		UpdatePacienteUI();
	}
	async private void ListViewTurnos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		SelectedTurno = (TurnoDto)turnosListView.SelectedItem;

		if (SelectedTurno != null) {
			MedicoRelacionado = await App.BaseDeDatos.SelectMedicoWhereId(SelectedTurno.MedicoId);
			PacienteRelacionado = await App.BaseDeDatos.SelectPacienteWhereId(SelectedTurno.PacienteId);
		} else {
			MedicoRelacionado = null;
			PacienteRelacionado = null;
		}

		UpdateTurnoUI();
		UpdateMedicoUI();
		UpdatePacienteUI();
	}




	//---------------------botonesDeModificar-------------------//
	private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
		if (SelectedTurno != null) {
			//this.AbrirComoDialogo<WindowModificarTurno>(SelectedTurno);
		}
	}
	private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
		if (MedicoRelacionado != null) {
			//this.AbrirComoDialogo<MedicoModificar>(MedicoRelacionado);
		}
	}
	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (PacienteRelacionado != null) {
			this.AbrirComoDialogo<WindowModificarPaciente>(PacienteRelacionado);
		}
	}



	//------------------botonesParaCrear------------------//
	private void ButtonAgregarMedico(object sender, RoutedEventArgs e) {
		//this.AbrirComoDialogo<MedicoModificar>();
	}
	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<WindowModificarPaciente>(); // this.NavegarA<WindowModificarPaciente>();
	}
	private void ButtonAgregarTurno(object sender, RoutedEventArgs e) {
		//this.AbrirComoDialogo<WindowModificarTurno>();
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
