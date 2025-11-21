using Clinica.AppWPF.ViewModels;
using Clinica.AppWPF.Ventanas;
using System.ComponentModel;
using System.Windows;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.AppWPF;

public partial class WindowModificarMedico : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	public ViewModelMedico _selectedMedico = ViewModelMedico.NewEmpty();
	public ViewModelMedico SelectedMedico { get => _selectedMedico; set { _selectedMedico = value; OnPropertyChanged(nameof(SelectedMedico)); } }

	//---------------------constructor-------------------//
	public WindowModificarMedico() {
		InitializeComponent();
		DataContext = this;
	}

	public WindowModificarMedico(ViewModelMedico selectedMedico) {
		InitializeComponent();
		SelectedMedico = selectedMedico;
		DataContext = this;
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
			this.Cerrar(); // this.NavegarA<WindowListarMedicos>();
		}
	}

	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); 
	}

	//---------------------botones.Horarios-------------------//
	private object? GetSelectedTreeItem() {
		return treeViewHorarios.SelectedItem;
	}

	private void BtnAgregarHorarioFranja_Click(object sender, RoutedEventArgs e) {
		var selected = GetSelectedTreeItem();

		DayOfWeek dia;

		if (selected is ViewModelHorariosAgrupados grupo) {
			dia = grupo.DiaSemana;
		} else if (selected is ViewModelHorario horario) {
			dia = horario.DiaSemana;
		} else {
			MessageBox.Show("Seleccione un día en el árbol para agregar un horario.");
			return;
		}

		var nuevoHorario = new ViewModelHorario {
			DiaSemana = dia,
			Desde = new TimeOnly(8, 0),
			Hasta = new TimeOnly(12, 0)
		};

		var win = new WindowModificarHorario(SelectedMedico, nuevoHorario, esNuevo: true);

		if (win.ShowDialog() == true) {
			// Se agregó realmente dentro de WindowModificarHorario
			// Ahora refrescamos los agrupados (INotifyPropertyChanged se encarga)
			OnPropertyChanged(nameof(SelectedMedico.HorariosAgrupados));
		}
	}

	private void BtnEditarHorario_Click(object sender, RoutedEventArgs e) {
		var selected = GetSelectedTreeItem();

		if (selected is not ViewModelHorario horario) {
			MessageBox.Show("Seleccione un horario para editar.");
			return;
		}

		var win = new WindowModificarHorario(SelectedMedico, horario, esNuevo: false);

		if (win.ShowDialog() == true) {
			// El horario ya está modificado (data binding)
			OnPropertyChanged(nameof(SelectedMedico.HorariosAgrupados));
		}
	}

	private void BtnEliminarHorario_Click(object sender, RoutedEventArgs e) {
		var selected = GetSelectedTreeItem();

		if (selected is not ViewModelHorario horario) {
			MessageBox.Show("Seleccione un horario para eliminar.");
			return;
		}

		if (MessageBox.Show("¿Eliminar este horario?", "Confirmar",
			MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			return;

		SelectedMedico.Horarios.Remove(horario);

		// Forzar refresco para que se actualicen los días vacíos
		OnPropertyChanged(nameof(SelectedMedico.HorariosAgrupados));
	}


	//------------------------Fin---------------------------//

}
