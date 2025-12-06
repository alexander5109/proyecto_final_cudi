using Clinica.AppWPF.Dtos;
using Clinica.AppWPF.Ventanas;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Comun;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class MedicoModificar : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	public MedicoDto _selectedMedico = MedicoDto.NewEmpty();
	public MedicoDto SelectedMedico { get => _selectedMedico; set { _selectedMedico = value; OnPropertyChanged(nameof(SelectedMedico)); } }

	//---------------------constructor-------------------//
	public MedicoModificar() {
		InitializeComponent();
		DataContext = this;
	}

	public MedicoModificar(MedicoDto medicoDto) {

		InitializeComponent();
		//SelectedMedico = medicoDto.ToDto();
		DataContext = this;
	}
	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		Result<Medico2025> resultado = SelectedMedico.ToDomain();

		resultado.Switch(
			ok => {
				bool exito = false;
				if (SelectedMedico.Id is null) {
					// _ValidarRepositorios nuevo médico
					//exito = App.BaseDeDatos.CreateMedico(MedicoRelacionado);
				} else {
					// Actualizar médico existente
					//exito = App.BaseDeDatos.UpdateMedico(MedicoRelacionado);
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
		SoundsService.PlayClickSound();
		if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedMedico.Name}", "Confirmar Eliminación", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK) {
			return;
		}
		//if (App.BaseDeDatos.DeleteMedico(MedicoRelacionado)) {
		//	this.Cerrar(); // this.NavegarA<WindowListarMedicos>();
		//}
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
        object? selected = GetSelectedTreeItem();

		DiaDeSemanaDto dia;

		if (selected is DtoHorariosAgrupados grupo) {
			dia = grupo.DiaSemana;
		} else if (selected is HorarioMedicoDto horario) {
			dia = horario.DiaSemana;
		} else {
			MessageBox.Show("Seleccione un día en el árbol para agregar un horario.");
			return;
		}

		HorarioMedicoDto nuevoHorario = new() {
			DiaSemana = dia,
			Desde = new TimeOnly(8, 0),
			Hasta = new TimeOnly(12, 0)
		};

		WindowModificarHorario win = new(SelectedMedico, nuevoHorario, esNuevo: true);

		if (win.ShowDialog() == true) {
			// Se agregó realmente dentro de WindowModificarHorario
			// Ahora refrescamos los agrupados (INotifyPropertyChanged se encarga)
			OnPropertyChanged(nameof(SelectedMedico.HorariosAgrupados));
		}
	}

	private void BtnEditarHorario_Click(object sender, RoutedEventArgs e) {
        object? selected = GetSelectedTreeItem();

		if (selected is not HorarioMedicoDto horario) {
			MessageBox.Show("Seleccione un horario para editar.");
			return;
		}

        WindowModificarHorario win = new(SelectedMedico, horario, esNuevo: false);

		if (win.ShowDialog() == true) {
			// El horario ya está modificado (data binding)
			OnPropertyChanged(nameof(SelectedMedico.HorariosAgrupados));
		}
	}

	private void BtnEliminarHorario_Click(object sender, RoutedEventArgs e) {
        object? selected = GetSelectedTreeItem();

		if (selected is not HorarioMedicoDto horario) {
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
