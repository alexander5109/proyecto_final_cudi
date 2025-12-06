using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSuperadmin;
public partial class TurnosModificar2025 : Window, INotifyPropertyChanged {



	public event PropertyChangedEventHandler? PropertyChanged;
	public TurnoDto _selectedTurnoView = new TurnoDto();
	public TurnoDto SelectedTurno { get => _selectedTurnoView; set { _selectedTurnoView = value; OnPropertyChanged(nameof(SelectedTurno)); } }
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	// Colecciones para los ComboBoxes
	public IEnumerable<PacienteDto> PacientesDisponibles { get; private set; } = System.Linq.Enumerable.Empty<PacienteDto>();
	public IEnumerable<MedicoDto> MedicosDisponibles { get; private set; } = System.Linq.Enumerable.Empty<MedicoDto>();

	//public List<EspecialidadMedicaDto> EspecialidadesDisponibles { get; } = App.BaseDeDatos.ReadDistinctEspecialidades();



	//---------------------public.constructors-------------------//
	public TurnosModificar2025() //Constructor vacio ==> _ValidarRepositorios.
{
		InitializeComponent();
		DataContext = this;
		LLenarComboBoxes();
	}

	public TurnosModificar2025(TurnoDto selectedTurno) //Constructor con un objeto como parametro ==> Modificarlo.
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
			PacientesDisponibles = System.Linq.Enumerable.Empty<PacienteDto>();
			MedicosDisponibles = System.Linq.Enumerable.Empty<MedicoDto>();
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
		//txtMedicos.SelectedValuePath = "Codigo";
		//txtMedicos.DisplayMemberPath = "Displayear"; //Property de cada Objeto para mostrarse como una union de dni nombre y apellido. 
		//txtMedicos.ItemsSource = App.BaseDeDatos.ReadMedicosWhereEspecialidad(txtEspecialidades.SelectedItem.ToString());
	//}
	//---------------------botones.GuardarCambios-------------------//

	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		Result<Turno2025> resultado = SelectedTurno.ToDomain();

		resultado.Switch(
			ok => {
				bool exito = false;

				if (SelectedTurno is null) {
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
		SoundsService.PlayClickSound();
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
