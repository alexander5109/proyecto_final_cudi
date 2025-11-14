using Clinica.AppWPF.Entidades;
using Clinica.AppWPF.ModelViews;
using Clinica.AppWPF.Ventanas;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Clinica.AppWPF;


public partial class MedicosModificar : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	public MedicoView _selectedMedico = MedicoView.NewEmpty();
	public MedicoView SelectedMedico { get => _selectedMedico; set { _selectedMedico = value; OnPropertyChanged(nameof(SelectedMedico)); } }
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	public MedicosModificar() {
		InitializeComponent();
		DataContext = this;
	}

	public MedicosModificar(MedicoView selectedMedico) {
		InitializeComponent();
		SelectedMedico = selectedMedico;
		DataContext = this;
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
		if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedMedico.Name}", "Confirmar Eliminación", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK) {
			return;
		}
		if (App.BaseDeDatos.DeleteMedico(SelectedMedico)) {
			this.Cerrar(); // this.NavegarA<Medicos>();
		}
	}

	private object? GetSelectedTreeItem() {
		return (treeViewHorarios.SelectedItem ??
				(this.FindName("treeViewHorarios") as TreeView)?.SelectedItem);
	}

	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); // this.NavegarA<Medicos>();
	}

	private void BtnAgregarHorario_Click(object sender, RoutedEventArgs e) {
		var selected = GetSelectedTreeItem();

		if (selected is not HorarioMedicoView dia) {
			MessageBox.Show("Seleccioná un día para agregar una franja.",
				"Info", MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		var dialog = new EditarFranjaHorarioDialog();
		dialog.Owner = this;

		if (dialog.ShowDialog() == true) {
			dia.FranjasHora.Add(new HorarioMedicoTimeSpanView {
				Desde = dialog.Desde,
				Hasta = dialog.Hasta
			});
		}
	}


	private void BtnEditarHorario_Click(object sender, RoutedEventArgs e) {
		var selected = GetSelectedTreeItem();

		if (selected is not HorarioMedicoTimeSpanView franja) {
			MessageBox.Show("Seleccioná una franja para editar.",
				"Info", MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		var dialog = new EditarFranjaHorarioDialog(franja);
		dialog.Owner = this;

		if (dialog.ShowDialog() == true) {
			franja.Desde = dialog.Desde;
			franja.Hasta = dialog.Hasta;
		}
	}
	private void BtnEliminarHorario_Click(object sender, RoutedEventArgs e) {
	}

	//------------------------Fin---------------------------//

}
