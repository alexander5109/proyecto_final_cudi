using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using Clinica.Dominio.TiposDeEnum;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;


public record HorarioDb(int Id, int MedicoId, int DiaSemana, TimeSpan HoraDesde, TimeSpan HoraHasta);

public class ViewModelHorarioAgrupado(int dia, List<HorarioDb> horarios) {
	public string DiaSemanaNombre { get; } = CultureInfo.GetCultureInfo("es-AR").DateTimeFormat.DayNames[dia];
	public ObservableCollection<HorarioMedicoViewModel> Horarios { get; } = new ObservableCollection<HorarioMedicoViewModel>(
			horarios.Select(h => new HorarioMedicoViewModel(h))
		);
}

public class HorarioMedicoViewModel(HorarioDb h) {
	public string Desde { get; } = h.HoraDesde.ToString(@"hh\:mm");
	public string Hasta { get; } = h.HoraHasta.ToString(@"hh\:mm");
}



public partial class DialogoModificarMedico : Window {
	public MedicoFormularioViewModel VM { get; }

	public DialogoModificarMedico(MedicoDbModel model, IEnumerable<EspecialidadCodigo> especialidades) {
		InitializeComponent();
		VM = new MedicoFormularioViewModel(model, especialidades);
		DataContext = VM;
	}

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

		DialogoModificarHorario win = new(SelectedMedico, nuevoHorario, esNuevo: true);

		if (win.ShowDialog() == true) {
			// Se agregó realmente dentro de DialogoModificarHorario
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

		DialogoModificarHorario win = new(SelectedMedico, horario, esNuevo: false);

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
	*/

	//------------------------Fin---------------------------//

}
