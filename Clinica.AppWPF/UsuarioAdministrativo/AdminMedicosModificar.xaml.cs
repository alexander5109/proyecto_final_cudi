using System.Collections.ObjectModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;


//public record HorarioDb(int Id, int MedicoId, int DiaSemana, TimeSpan HoraDesde, TimeSpan HoraHasta);



public partial class AdminMedicosModificar : Window {
	public AdminMedicosModificarViewModel VM { get; }

	public AdminMedicosModificar() {
		InitializeComponent();
		VM = new AdminMedicosModificarViewModel(new MedicoDbModel());
		DataContext = VM;
	}

	public AdminMedicosModificar(MedicoDbModel model) {
		InitializeComponent();
		VM = new AdminMedicosModificarViewModel(model);
		DataContext = VM;
		Loaded += async (_, _) => await VM.CargarHorariosAsync();
	}

    private void ClickBoton_GuardarCambios(object sender, RoutedEventArgs e) {
		MessageBox.Show("Falta implementar el guardado de cambios");
	}

    private void ClickBoton_EliminarMedico(object sender, RoutedEventArgs e) {
		MessageBox.Show("Falta implementar la eliminacion de medico");

	}

	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();

	/*

	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		ResultWpf<Medico2025> resultado = SelectedMedico.ToDomain();

		resultado.Switch(
			ok => {
				bool exito = false;
				if (SelectedMedico.Id is null) {
					// _ValidarRepositorios nuevo médico
					//exito = App.BaseDeDatos.CreateMedico(RelatedMedico);
				} else {
					// Actualizar médico existente
					//exito = App.BaseDeDatos.UpdateMedico(RelatedMedico);
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
		//if (App.BaseDeDatos.DeleteMedico(RelatedMedico)) {
		//	this.Cerrar(); // this.NavegarA<WindowListarMedicos>();
		//}
	}

	//---------------------botones.Salida-------------------//
	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();

	//---------------------botones.HorariosViewModelList-------------------//
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

		AdminMedicosModificarHorario win = new(SelectedMedico, nuevoHorario, esNuevo: true);

		if (win.ShowDialog() == true) {
			// Se agregó realmente dentro de AdminMedicosModificarHorario
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

		AdminMedicosModificarHorario win = new(SelectedMedico, horario, esNuevo: false);

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

		SelectedMedico.HorariosViewModelList.Remove(horario);

		// Forzar refresco para que se actualicen los días vacíos
		OnPropertyChanged(nameof(SelectedMedico.HorariosAgrupados));
	}
	*/

	//------------------------Fin---------------------------//

}

public class ViewModelHorarioAgrupado(DayOfWeek dia, List<HorarioDbModel> horarios) {
	public string DiaSemanaNombre { get; } = dia.ATexto();
	//public string DiaSemanaNombre { get; } = CultureInfo.GetCultureInfo("es-AR").DateTimeFormat.DayNames[dia];
	public ObservableCollection<HorarioMedicoViewModel> Horarios { get; } = new ObservableCollection<HorarioMedicoViewModel>(
			horarios.Select(h => new HorarioMedicoViewModel(h))
		);
}

public class HorarioMedicoViewModel(HorarioDbModel h) {
	public string Desde { get; } = h.HoraDesde.ToString(@"hh\:mm");
	public string Hasta { get; } = h.HoraHasta.ToString(@"hh\:mm");
}
