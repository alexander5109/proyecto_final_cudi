using Clinica.AppWPF.Entidades;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Clinica.AppWPF;
public partial class TurnosModificar : Window, INotifyPropertyChanged {



	public event PropertyChangedEventHandler? PropertyChanged;
	public TurnoView _selectedTurnoView = TurnoView.NewEmpty();
	public TurnoView SelectedTurno { get => _selectedTurnoView; set { _selectedTurnoView = value; OnPropertyChanged(nameof(SelectedTurno)); } }
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


	public List<string> EspecialidadesDisponibles { get; } = App.BaseDeDatos.ReadDistinctEspecialidades();



	//---------------------public.constructors-------------------//
	public TurnosModificar() //Constructor vacio ==> Crear.
{
		InitializeComponent();
		DataContext = this;
		LLenarComboBoxes();
	}

	public TurnosModificar(TurnoView selectedTurno) //Constructor con un objeto como parametro ==> Modificarlo.
	{
		InitializeComponent();
		LLenarComboBoxes();
		SelectedTurno = selectedTurno;
	}


	//---------------------Visualizacion-comboboxes-------------------//
	private void LLenarComboBoxes()  //por defecto llenamos todos los comboboxes
	{
		//txtEspecialidades.ItemsSource = App.BaseDeDatos.ReadDistinctEspecialidades();

		//txtPacientes.ItemsSource = App.BaseDeDatos.ReadPacientes();
		//txtPacientes.DisplayMemberPath = "Displayear";  //Property de cada Objeto para mostrarse como una union de dni name y lastName. 

		//txtMedicos.ItemsSource = App.BaseDeDatos.ReadMedicos();
		//txtMedicos.DisplayMemberPath = "Displayear";    //Property de cada Objeto para mostrarse como una union de dni name y lastName. 
	}
	private void txtEspecialidades_SelectionChanged(object sender, SelectionChangedEventArgs e) {
		//txtMedicos.SelectedValuePath = "Id";
		//txtMedicos.DisplayMemberPath = "Displayear";    //Property de cada Objeto para mostrarse como una union de dni name y lastName. 
		//txtMedicos.ItemsSource = App.BaseDeDatos.ReadMedicosWhereEspecialidad(txtEspecialidades.SelectedItem.ToString());
	}
	//---------------------botones.GuardarCambios-------------------//

	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		App.PlayClickJewel();

		Result<Turno2025> resultado = SelectedTurno.ToDomain();

		resultado.Switch(
			ok => {
				bool exito;

				if (SelectedTurno.Id is null) {
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
		this.Cerrar(); // this.NavegarA<Turnos>();
	}
	//------------------------Fin---------------------------//
}
