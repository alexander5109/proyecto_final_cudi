using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class RecepcionistaPacienteFormulario : Window {
	public RecepcionistaPacienteFormularioViewModel VM { get; private set; } = new();

	public RecepcionistaPacienteFormulario() {
		InitializeComponent();
		DataContext = VM;
	}

	public RecepcionistaPacienteFormulario(PacienteId id) {
		InitializeComponent();
		DataContext = VM;
		_ = CargaInicialAsync(id);
	}

	private async Task CargaInicialAsync(PacienteId id) {
		PacienteDbModel? dto = await App.Repositorio.SelectPacienteWhereId(id);
		//MessageBox.Show(dto.ToString());
		if (dto == null) {
			MessageBox.Show("Paciente no encontrado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			Close();
			return;
		}
		//SelectedPacienteId = dto.Id;

		VM = dto.ToViewModel();
		DataContext = VM;
	}

	private async void ButtonGuardar(object sender, RoutedEventArgs e) {
		ResultWpf<UnitWpf> result = await VM.GuardarAsync();
		result.MatchAndDo(
			caseOk => MessageBox.Show("Cambios guardados.", "Éxito", MessageBoxButton.OK),
			caseError => caseError.ShowMessageBox()
		);
	}

	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		Close();
	}

	private async void ButtonEliminar(object sender, RoutedEventArgs e) {
		if (
			VM.Id is not PacienteId idGood || (
			MessageBox.Show("¿Esta seguro que desea eliminar este paciente?",
			"Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.No)
		) return;

		ResultWpf<UnitWpf> result = await App.Repositorio.DeletePacienteWhereId(idGood);
		result.MatchAndDo(
			caseOk => {
				MessageBox.Show("Paciente eliminado.", "Éxito", MessageBoxButton.OK);
				Close();
			},
			caseError => caseError.ShowMessageBox()
		);
	}

	private void ButtonSolicitarTurno(object sender, RoutedEventArgs e) {
		//if (VM.Id is int notNullId) {
		//	this.AbrirComoDialogo<SecretariaFormularioTurno>(SelectedPacienteId.CrearResult(notNullId));
		//}
	}

}
