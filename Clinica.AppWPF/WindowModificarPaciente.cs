using Clinica.AppWPF.ViewModels;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.AppWPF; 
public partial class WindowModificarPaciente : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	public WindowModificarPacienteViewModel _selectedView = WindowModificarPacienteViewModel.NewEmpty();
	public WindowModificarPacienteViewModel SelectedPaciente { get => _selectedView; set { _selectedView = value; OnPropertyChanged(nameof(SelectedPaciente)); } }
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


	public WindowModificarPaciente(){
		InitializeComponent();
		DataContext = this;
	}

	public WindowModificarPaciente(WindowModificarPacienteViewModel selectedPaciente){
		InitializeComponent();
		SelectedPaciente = selectedPaciente;
		DataContext = this;
	}


	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		Result<Paciente2025> resultado = SelectedPaciente.ToDomain();
		resultado.Switch(
			ok => {
				bool exito = false;
				if (SelectedPaciente.Id is null) {
					// _ValidarRepositorios nuevo paciente
					//exito = App.BaseDeDatos.CreatePaciente(SelectedPaciente);
				} else {
					// Actualizar existente
					//exito = App.BaseDeDatos.UpdatePaciente(SelectedPaciente);
				}
				if (exito)
					this.Cerrar();
			},
			error => {
				MessageBox.Show(
					$"No se puede guardar el paciente: {error}",
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
		if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedPaciente.Nombre}",
			"Confirmar Eliminación",
			MessageBoxButton.OKCancel,
			MessageBoxImage.Warning
		) != MessageBoxResult.OK) {
			return;
		}
		//if (App.BaseDeDatos.DeletePaciente(SelectedPaciente)) {
			//this.Cerrar(); // this.NavegarA<WindowListarMedicos>();
		//}
	}
	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); // this.NavegarA<WindowListarPacientes>();
	}

	//------------------------Fin----------------------//
}
