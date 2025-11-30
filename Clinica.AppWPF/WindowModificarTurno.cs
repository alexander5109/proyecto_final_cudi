using Clinica.AppWPF.ViewModels;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.AppWPF;
public partial class WindowModificarTurno : Window, INotifyPropertyChanged {



	public event PropertyChangedEventHandler? PropertyChanged;
	public WindowModificarTurnoViewModel _selectedTurnoView = WindowModificarTurnoViewModel.NewEmpty();
	public WindowModificarTurnoViewModel SelectedTurno { get => _selectedTurnoView; set { _selectedTurnoView = value; OnPropertyChanged(nameof(SelectedTurno)); } }
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	// Colecciones para los ComboBoxes
	public IEnumerable<WindowModificarPacienteViewModel> PacientesDisponibles { get; private set; } = System.Linq.Enumerable.Empty<WindowModificarPacienteViewModel>();
	public IEnumerable<WindowModificarMedicoViewModel> MedicosDisponibles { get; private set; } = System.Linq.Enumerable.Empty<WindowModificarMedicoViewModel>();

	//public List<EspecialidadMedicaViewModel> EspecialidadesDisponibles { get; } = App.BaseDeDatos.ReadDistinctEspecialidades();



	//---------------------public.constructors-------------------//
	public WindowModificarTurno() //Constructor vacio ==> _ValidarRepositorios.
{
		InitializeComponent();
		DataContext = this;
		LLenarComboBoxes();
	}

	public WindowModificarTurno(WindowModificarTurnoViewModel selectedTurno) //Constructor con un objeto como parametro ==> Modificarlo.
	{
		InitializeComponent();
		DataContext = this;
		LLenarComboBoxes();
		SelectedTurno = selectedTurno;
	}


	//---------------------Visualizacion-comboboxes-------------------//
	private void LLenarComboBoxes() //por defecto llenamos todos los comboboxes
	{
		try {
			//PacientesDisponibles = App.BaseDeDatos.ReadPacientes();
			//MedicosDisponibles = App.BaseDeDatos.ReadMedicos();
		} catch {
			PacientesDisponibles = System.Linq.Enumerable.Empty<WindowModificarPacienteViewModel>();
			MedicosDisponibles = System.Linq.Enumerable.Empty<WindowModificarMedicoViewModel>();
		}
		OnPropertyChanged(nameof(PacientesDisponibles));
		OnPropertyChanged(nameof(MedicosDisponibles));

		//txtEspecialidades.ItemsSource = App.BaseDeDatos.ReadDistinctEspecialidades();

		//txtPacientes.ItemsSource = App.BaseDeDatos.ReadPacientes();
		//txtPacientes.DisplayMemberPath = "Displayear"; //Property de cada Objeto para mostrarse como una union de dni nombre y apellido. 

		//txtMedicos.ItemsSource = App.BaseDeDatos.ReadMedicos();
		//txtMedicos.DisplayMemberPath = "Displayear"; //Property de cada Objeto para mostrarse como una union de dni nombre y apellido. 
	}
	//private void txtEspecialidades_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		//txtMedicos.SelectedValuePath = "CodigoInterno";
		//txtMedicos.DisplayMemberPath = "Displayear"; //Property de cada Objeto para mostrarse como una union de dni nombre y apellido. 
		//txtMedicos.ItemsSource = App.BaseDeDatos.ReadMedicosWhereEspecialidad(txtEspecialidades.SelectedItem.ToString());
	//}
	//---------------------botones.GuardarCambios-------------------//

	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		App.PlayClickJewel();

		Result<Turno2025> resultado = SelectedTurno.ToDomain();

		resultado.Switch(
			ok => {
				bool exito = false;

				if (SelectedTurno.Id is null) {
					// _ValidarRepositorios nuevo turno
					//exito = App.BaseDeDatos.CreateTurno(SelectedTurno);
				} else {
					// Actualizar existente
					//exito = App.BaseDeDatos.UpdateTurno(SelectedTurno);
				}

				if (exito)
					this.Cerrar();
			},
			error => {
				MessageBox.Show(
					$"No se puede guardar el turno: {error}",
					"Error de ingreso",
					MessageBoxButton.OK,
					MessageBoxImage.Warning
				);
			}
		);
	}

	//---------------------botones.Eliminar-------------------//
	private void ButtonEliminar(object sender, RoutedEventArgs e) {
		App.PlayClickJewel();
		//---------Checknulls-----------//
		if (SelectedTurno is null) {
			MessageBox.Show($"No hay item seleccionado.");
			return;
		}
		//---------confirmacion-----------//
		if (MessageBox.Show($"¿Está seguro que desea eliminar este turno?",
			"Confirmar Eliminación",
			MessageBoxButton.OKCancel,
			MessageBoxImage.Warning
		) != MessageBoxResult.OK) {
			return;
		}
		//---------Eliminar-----------//
		//if (App.BaseDeDatos.DeleteTurno(SelectedTurno)) {
		//	this.Cerrar();
		//}
	}


	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); // this.NavegarA<WindowListarTurnos>();
	}
	//------------------------Fin---------------------------//
}
