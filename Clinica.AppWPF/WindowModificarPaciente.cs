using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using System.ComponentModel;
using System.Windows;

namespace Clinica.AppWPF; 
public partial class WindowModificarPaciente : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	public ModelViewPaciente _selectedView = ModelViewPaciente.NewEmpty();
	public ModelViewPaciente SelectedPaciente { get => _selectedView; set { _selectedView = value; OnPropertyChanged(nameof(SelectedPaciente)); } }
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


	public WindowModificarPaciente(){
		InitializeComponent();
		DataContext = this;
	}

	public WindowModificarPaciente(ModelViewPaciente selectedPaciente){
		InitializeComponent();
		SelectedPaciente = selectedPaciente;
		DataContext = this;
	}


	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		App.PlayClickJewel();
		Result<Paciente2025> resultado = SelectedPaciente.ToDomain();
		resultado.Switch(
			ok => {
				bool exito;
				if (SelectedPaciente.Id is null) {
					// Crear nuevo paciente
					exito = App.BaseDeDatos.CreatePaciente(ok, SelectedPaciente);
				} else {
					// Actualizar existente
					exito = App.BaseDeDatos.UpdatePaciente(ok, SelectedPaciente.Id);
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
		App.PlayClickJewel();
		if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedPaciente.Name}",
			"Confirmar Eliminación",
			MessageBoxButton.OKCancel,
			MessageBoxImage.Warning
		) != MessageBoxResult.OK) {
			return;
		}
		if (App.BaseDeDatos.DeletePaciente(SelectedPaciente)) {
			this.Cerrar(); // this.NavegarA<WindowListarMedicos>();
		}
	}
	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); // this.NavegarA<WindowListarPacientes>();
	}

	//------------------------Fin----------------------//
}
