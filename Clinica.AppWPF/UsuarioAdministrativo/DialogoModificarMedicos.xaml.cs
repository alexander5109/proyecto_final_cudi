using System.Linq;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.Ventanas;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public partial class AdminMedicosModificar : Window {
	public DialogoMedicoModificarVM VM { get; }

	// ==========================================================
	// CONSTRUCTORES
	// ==========================================================
	public AdminMedicosModificar() {
		InitializeComponent();
		VM = new DialogoMedicoModificarVM(new MedicoDbModel());
		DataContext = VM;
	}

	public AdminMedicosModificar(MedicoDbModel model) {
		InitializeComponent();
		VM = new DialogoMedicoModificarVM(model);
		DataContext = VM;
		Loaded += async (_, _) => await VM.CargarHorariosAsync();
	}

	private object? GetSelectedTreeItem() => treeViewHorarios.SelectedItem;

	// ==========================================================
	// BOTONES: CRUD HORARIOS
	// ==========================================================

	private void ClickBoton_EditarHorario(object sender, RoutedEventArgs e) {
		object? selected = GetSelectedTreeItem();
		if (selected is not HorarioMedicoViewModel horario) {
			MessageBox.Show("Seleccione un horario para editar.");
			return;
		}
		this.AbrirComoDialogo<DialogoModificarHorarios>(horario);
		//var win = new Clinica.AppWPF.Ventanas.DialogoModificarHorarios(horario);
		//if (win.ShowDialog() == true) {
		// horario object was modified by window binding; notify UI
		// no extra action required
		//}
	}

	private void ClickBoton_AgregarHorario(object sender, RoutedEventArgs e) {
		// Need to ask user which day to add or use selected group
		object? selected = GetSelectedTreeItem();
		DayOfWeek dia = DayOfWeek.Monday;
		if (selected is ViewModelHorarioAgrupado grupo) dia = grupo.DiaSemana;
		else if (selected is HorarioMedicoViewModel h) dia = h.DiaSemana;

		var nuevo = new HorarioMedicoViewModel(dia, new TimeOnly(8, 0), new TimeOnly(12, 0));
		this.AbrirComoDialogo<DialogoModificarHorarios>(nuevo);
		// add to group or create group
		var grupoExistente = VM.HorariosAgrupados.FirstOrDefault(g => g.DiaSemana == nuevo.DiaSemana);
		if (grupoExistente is not null) {
			grupoExistente.Horarios.Add(nuevo);
		} else {
			VM.HorariosAgrupados.Add(new ViewModelHorarioAgrupado(nuevo.DiaSemana, []));
			VM.HorariosAgrupados.Last().Horarios.Add(nuevo);
		}
	}

	private void ClickBoton_EliminarHorario(object sender, RoutedEventArgs e) {
		object? selected = GetSelectedTreeItem();
		if (selected is not HorarioMedicoViewModel horario) {
			MessageBox.Show("Seleccione un horario para eliminar.");
			return;
		}

		if (MessageBox.Show("¿Eliminar este horario?", "Confirmar", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			return;

		var grupo = VM.HorariosAgrupados.FirstOrDefault(g => g.DiaSemana == horario.DiaSemana);
		if (grupo is null) return;
		grupo.Horarios.Remove(horario);
		// if group became empty remove it
		if (!grupo.Horarios.Any()) VM.HorariosAgrupados.Remove(grupo);
	}


	//------------------------Fin---------------------------//






	// ==========================================================
	// BOTONES: PERSISTENCIA
	// ==========================================================

	private async void ClickBoton_GuardarCambios(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		ResultWpf<UnitWpf> result = await VM.GuardarAsync();
		result.MatchAndDo(
			caseOk => MessageBox.Show("Cambios guardados.", "Éxito", MessageBoxButton.OK),
			caseError => caseError.ShowMessageBox()
		);
	}

	private async void ClickBoton_Eliminar(object sender, RoutedEventArgs e) {
		if (
			VM.Id is not MedicoId idGood || (
			MessageBox.Show("¿Esta seguro que desea eliminar este paciente?",
			"Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.No)
		) return;

		ResultWpf<UnitWpf> result = await App.Repositorio.DeleteMedicoWhereId(idGood);
		result.MatchAndDo(
			caseOk => {
				MessageBox.Show("PacienteExtensiones eliminado.", "Éxito", MessageBoxButton.OK);
				Close();
			},
			caseError => caseError.ShowMessageBox()
		);
	}




	// ==========================================================
	// BOTONES: REFRESH
	// ==========================================================

	// I guess i could implement it on the viewmodel and call it through the button here. It's just a matter of calling refresh pacientes and selectonebyid (which we have)





	// ==========================================================
	// BOTONES: NAV
	// ==========================================================

	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}