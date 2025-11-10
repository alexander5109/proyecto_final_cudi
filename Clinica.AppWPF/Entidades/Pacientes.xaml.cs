using Clinica.AppWPF.Entidades;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Clinica.AppWPF {
	public partial class Pacientes : Window {
		private Result<Paciente2025EnDb>? SelectedPaciente;
		private Turno? SelectedTurno;
		public ObservableCollection<PacienteModelView> PacientesModelViewList { get; set; }
		public Pacientes() {
			InitializeComponent();
			//PacientesModelViewList = new ObservableCollection<PacienteModelView>(
			//	App.BaseDeDatos.DictPacientes.Values.Select(x => new PacienteModelView(x))
			//);
			//MessageBox.Show("Pacientes cargados: " + PacientesModelViewList.Count.ToString());
			//MessageBox.Show("Pacientes cargados en DictPacientes: " + App.BaseDeDatos.DictPacientes.Count.ToString());
			//pacientesListView.ItemsSource = PacientesModelViewList;
			//buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
			//this.DataContext = this;
		}

		//----------------------ActualizarSecciones-------------------//

		private void UpdatePacienteUI() {
			//PacientesModelViewList = new ObservableCollection<PacienteModelView>(
			//	App.BaseDeDatos.DictPacientes.Values.Select(x => new PacienteModelView(x))
			//);
			//buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
		}
		private void UpdateTurnoUI() {
			//SelectedPaciente?.Match(
			//	ok => {
			//		turnosListView.ItemsSource = App.BaseDeDatos.ReadTurnosWherePacienteId(ok);
			//		buttonModificarTurno.IsEnabled = true;

			//	},
			//	error => {
			//		buttonModificarTurno.IsEnabled = false;
			//	}
			//);
		}
		private void UpdateMedicoUI() {
			//if (SelectedTurno?.MedicoRelacionado is not null) {
			//	var m = SelectedTurno.MedicoRelacionado;
			//	txtMedicoDni.Text = m.Dni;
			//	txtMedicoNombre.Text = m.Name;
			//	txtMedicoApellido.Text = m.LastName;
			//	txtMedicoEspecialidad.Text = m.Especialidad;
			//	buttonModificarMedico.IsEnabled = true;
			//} else {
			//	txtMedicoDni.Text = "";
			//	txtMedicoNombre.Text = "";
			//	txtMedicoApellido.Text = "";
			//	txtMedicoEspecialidad.Text = "";
			//	buttonModificarMedico.IsEnabled = false;
			//}
		}

		//----------------------EventosRefresh-------------------//
		private void listViewTurnos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			//SelectedTurno = (Turno)turnosListView.SelectedItem;
			//UpdatePacienteUI();
			//UpdateTurnoUI();
			//UpdateMedicoUI();
		}

		private void pacientesListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			//SelectedPaciente = pacientesListView.SelectedItem as Result<Paciente2025EnDb>?;
			//UpdateMedicoUI();
			//UpdateTurnoUI();
			//UpdatePacienteUI();
		}

		//---------------------botonesDeModificar-------------------//
		private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
			//SelectedPaciente?.Match(
			//	ok => this.AbrirComoDialogo<PacientesModificar>(ok),
			//	error => MessageBox.Show($"Paciente inválido: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning)
			//);
		}
		private void ButtonModificarTurno(object sender, RoutedEventArgs e) {
			//if (SelectedTurno != null) {
			//	this.AbrirComoDialogo<TurnosModificar>(SelectedTurno);
			//}
		}
		private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
			//this.AbrirComoDialogo<PacientesModificar>(); // this.NavegarA<PacientesModificar>(); } private void ButtonAgregarTurno(object sender, RoutedEventArgs e) { this.AbrirComoDialogo<MedicosModificar>(); }
		}
		private void ButtonModificarMedico(object sender, RoutedEventArgs e) {
			//if (SelectedTurno?.MedicoRelacionado != null) {
			//	this.AbrirComoDialogo<MedicosModificar>(SelectedTurno?.MedicoRelacionado!);
			//}
		}

		//---------------------botonesDeVolver-------------------//
		private void ButtonSalir(object sender, RoutedEventArgs e) {
			this.Salir();
		}
		private void ButtonHome(object sender, RoutedEventArgs e) {
			this.VolverAHome();
		}



		private void Window_Activated(object sender, EventArgs e) {
			//App.UpdateLabelDataBaseModo(this.labelBaseDeDatosModo);
			//UpdatePacienteUI();
			//UpdateTurnoUI();
			//UpdateMedicoUI();
		}
		//------------------------Fin.Medicos----------------------//
	}
}
