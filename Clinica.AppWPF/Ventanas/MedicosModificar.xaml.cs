using Clinica.AppWPF.Entidades;
using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace Clinica.AppWPF;


public partial class MedicosModificar : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	private MedicoView _selectedMedico;
	public List<string> EspecialidadesDisponibles { get; } = MedicoEspecialidad2025.EspecialidadesDisponibles;

	public MedicoView SelectedMedico {
		get => _selectedMedico;
		set {
			_selectedMedico = value;
			OnPropertyChanged(nameof(SelectedMedico));
		}
	}
	//---------------------public.constructors-------------------//
	public MedicosModificar() {
		InitializeComponent();
		DataContext = this;
		SelectedMedico = MedicoView.NewEmpty(); // instancia vacía lista para bindear
	}

	// Constructor para editar un médico existente
	public MedicosModificar(MedicoView selectedMedico) {
		InitializeComponent();
		DataContext = this;
		SelectedMedico = selectedMedico;
		//MessageBox.Show(
		//	$"Cargando datos del médico seleccionado: {SelectedMedico.Especialidad}\n" +
		//	$"Disponibles: {string.Join(", ", EspecialidadesDisponibles)}",
		//	"Editar Médico",
		//	MessageBoxButton.OK,
		//	MessageBoxImage.Information
		//);
	}
	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		App.PlayClickJewel();

		if (SelectedMedico is null) {
			MessageBox.Show("No hay médico seleccionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		Result<Medico2025> resultado = SelectedMedico.ToDomain();

		resultado.Switch(
			ok => {
				bool exito;

				if (SelectedMedico.Id is null) {
					// Crear nuevo médico
					exito = App.BaseDeDatos.CreateMedico(ok, SelectedMedico);
				} else {
					// Actualizar médico existente
					exito = App.BaseDeDatos.UpdateMedico(ok, SelectedMedico.Id);
				}

				if (exito)
					this.Cerrar();
			},
			error => {
				MessageBox.Show(
					$"No se puede guardar el médico: {error}",
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
		if (SelectedMedico is null || SelectedMedico.Dni is null) {
			MessageBox.Show($"No hay item seleccionado.");
			return;
		}
		//---------confirmacion-----------//
		if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedMedico.Name}", "Confirmar Eliminación", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK) {
			return;
		}
		//---------Eliminar-----------//
		if (App.BaseDeDatos.DeleteMedico(SelectedMedico)) {
			this.Cerrar(); // this.NavegarA<Medicos>();
		}
	}



	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); // this.NavegarA<Medicos>();
	}




	//------------------botonesParaCrear------------------//

	private void BtnAgregarHorario_Click(object sender, RoutedEventArgs e) {
		HorarioEditor editor = new HorarioEditor();
	}

	protected void OnPropertyChanged(string propertyName)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	private void BtnEditarHorario_Click(object sender, RoutedEventArgs e) {

	}
	private void BtnEliminarHorario_Click(object sender, RoutedEventArgs e) {

	}
	//------------------------Fin---------------------------//
}
