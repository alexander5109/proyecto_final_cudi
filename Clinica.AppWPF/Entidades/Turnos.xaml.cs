using System.Windows;
using System.Windows.Controls;

namespace Clinica.AppWPF {
	public partial class Turnos : Window {
		private static Turno? SelectedTurno = null;

		public Turnos() {
			InitializeComponent();
		}

		//----------------------ActualizarSecciones-------------------//
		private void UpdateTurnoUI() {
			turnosListView.ItemsSource = App.BaseDeDatos.ReadTurnos();
			buttonModificarTurno.IsEnabled = SelectedTurno != null;
			txtCalendario.SelectedDate = SelectedTurno?.Fecha;
			txtCalendario.DisplayDate = SelectedTurno?.Fecha ?? DateTime.Today;
		}
		private void UpdateMedicoUI() {
			txtMedicoDni.Text = SelectedTurno?.MedicoRelacionado?.Dni;
			txtMedicoNombre.Text = SelectedTurno?.MedicoRelacionado?.Name;
			txtMedicoApellido.Text = SelectedTurno?.MedicoRelacionado?.LastName;
			txtMedicoEspecialidad.Text = SelectedTurno?.MedicoRelacionado?.Especialidad;
			buttonModificarMedico.IsEnabled = SelectedTurno?.MedicoRelacionado != null;
		}
		private void UpdatePacienteUI() {
			if (SelectedTurno?.PacienteRelacionado is not null) {
				var p = SelectedTurno.PacienteRelacionado.Value; // Paciente2025EnDb
				txtPacienteDni.Text = p.Paciente.Dni; // implicit string
				txtPacienteNombre.Text = p.Paciente.NombreCompleto.Nombre;
				txtPacienteApellido.Text = p.Paciente.NombreCompleto.Apellido;
				txtPacienteEmail.Text = p.Paciente.Contacto.Email; // implicit string
				txtPacienteTelefono.Text = p.Paciente.Contacto.Telefono; // implicit string
				buttonModificarPaciente.IsEnabled = true;
			} else {
				txtPacienteDni.Text = "";
				txtPacienteNombre.Text = "";
				txtPacienteApellido.Text = "";
				txtPacienteEmail.Text = "";
				txtPacienteTelefono.Text = "";
				buttonModificarPaciente.IsEnabled = false;
			}
		}





		//----------------------EventosRefresh-------------------//
		private void Window_Activated(object sender, EventArgs e) {
			App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
			UpdateTurnoUI();
			UpdateMedicoUI();
			UpdatePacienteUI();
		}
		private void listViewTurnos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			SelectedTurno = (Turno)turnosListView.SelectedItem;
			UpdateTurnoUI();
			UpdateMedicoUI();
			UpdatePacienteUI();
		}




		//---------------------botonesDeModificar-------------------//
		private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
			if (SelectedTurno != null) {
				this.AbrirComoDialogo<TurnosModificar>(SelectedTurno);
			}
		}
		private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
			if (SelectedTurno?.MedicoRelacionado != null) {
				this.AbrirComoDialogo<MedicosModificar>(SelectedTurno?.MedicoRelacionado);
			}
		}
		private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
			if (SelectedTurno?.PacienteRelacionado != null) {
				this.AbrirComoDialogo<PacientesModificar>(SelectedTurno?.PacienteRelacionado);
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
		private void ButtonSalir(object sender, RoutedEventArgs e) {
			this.Salir();
		}
		private void ButtonHome(object sender, RoutedEventArgs e) {
			this.VolverAHome();
		}
		//------------------------Fin.Turnos----------------------//
	}
}
