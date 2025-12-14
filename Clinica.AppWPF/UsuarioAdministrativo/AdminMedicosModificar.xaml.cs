using System.Windows;
using Clinica.AppWPF.Infrastructure;
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

	// ==========================================================
	// BOTONES: CRUD HORARIOS
	// ==========================================================

	private void ClickBoton_EditarHorario(object sender, RoutedEventArgs e) {

	}

	private void ClickBoton_AgregarHorario(object sender, RoutedEventArgs e) {

	}

	private void ClickBoton_EliminarHorario(object sender, RoutedEventArgs e) {

	}


	/*
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