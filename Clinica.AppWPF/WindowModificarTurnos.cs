using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Comun;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Clinica.Dominio.Entidades;

namespace Clinica.AppWPF;
public partial class WindowModificarTurnos : Window, INotifyPropertyChanged {



	public event PropertyChangedEventHandler? PropertyChanged;
	public ModelViewTurno _selectedTurnoView = ModelViewTurno.NewEmpty();
	public ModelViewTurno SelectedTurno { get => _selectedTurnoView; set { _selectedTurnoView = value; OnPropertyChanged(nameof(SelectedTurno)); } }
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	// Colecciones para los ComboBoxes
	public IEnumerable<ModelViewPaciente> PacientesDisponibles { get; private set; } = System.Linq.Enumerable.Empty<ModelViewPaciente>();
	public IEnumerable<ModelViewMedico> MedicosDisponibles { get; private set; } = System.Linq.Enumerable.Empty<ModelViewMedico>();

	public List<string> EspecialidadesDisponibles { get; } = App.BaseDeDatos.ReadDistinctEspecialidades();



	//---------------------public.constructors-------------------//
	public WindowModificarTurnos() //Constructor vacio ==> Crear.
{
		InitializeComponent();
		DataContext = this;
		LLenarComboBoxes();
	}

	public WindowModificarTurnos(ModelViewTurno selectedTurno) //Constructor con un objeto como parametro ==> Modificarlo.
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
			PacientesDisponibles = App.BaseDeDatos.ReadPacientes();
			MedicosDisponibles = App.BaseDeDatos.ReadMedicos();
		} catch {
			PacientesDisponibles = System.Linq.Enumerable.Empty<ModelViewPaciente>();
			MedicosDisponibles = System.Linq.Enumerable.Empty<ModelViewMedico>();
		}
		OnPropertyChanged(nameof(PacientesDisponibles));
		OnPropertyChanged(nameof(MedicosDisponibles));

		//txtEspecialidades.ItemsSource = App.BaseDeDatos.ReadDistinctEspecialidades();

		//txtPacientes.ItemsSource = App.BaseDeDatos.ReadPacientes();
		//txtPacientes.DisplayMemberPath = "Displayear"; //Property de cada Objeto para mostrarse como una union de dni name y lastName. 

		//txtMedicos.ItemsSource = App.BaseDeDatos.ReadMedicos();
		//txtMedicos.DisplayMemberPath = "Displayear"; //Property de cada Objeto para mostrarse como una union de dni name y lastName. 
	}
	//private void txtEspecialidades_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		//txtMedicos.SelectedValuePath = "Id";
		//txtMedicos.DisplayMemberPath = "Displayear"; //Property de cada Objeto para mostrarse como una union de dni name y lastName. 
		//txtMedicos.ItemsSource = App.BaseDeDatos.ReadMedicosWhereEspecialidad(txtEspecialidades.SelectedItem.ToString());
	//}
	//---------------------botones.GuardarCambios-------------------//

	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		App.PlayClickJewel();

		Result<Turno2025> resultado = SelectedTurno.ToDomain();

		resultado.Switch(
			ok => {
				bool exito;

				if (string.IsNullOrEmpty(SelectedTurno.Id)) {
					// Crear nuevo turno
					exito = App.BaseDeDatos.CreateTurno(ok, SelectedTurno);
				} else {
					// Actualizar existente
					exito = App.BaseDeDatos.UpdateTurno(ok, SelectedTurno);
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
		if (App.BaseDeDatos.DeleteTurno(SelectedTurno)) {
			this.Cerrar();
		}
	}


	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); // this.NavegarA<WindowListarTurnos>();
	}
	//------------------------Fin---------------------------//
}
