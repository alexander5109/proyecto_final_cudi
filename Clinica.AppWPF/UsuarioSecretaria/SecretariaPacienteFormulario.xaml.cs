using System.Windows;
using Clinica.AppWPF;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioSecretaria;
using Clinica.Dominio.Entidades;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaPacienteFormulario : Window {
	public SecretariaPacienteFormularioViewModel ViewModel { get; private set; } = new();

	public SecretariaPacienteFormulario() {
		InitializeComponent();
		DataContext = ViewModel;
	}

	public SecretariaPacienteFormulario(PacienteId id) {
		InitializeComponent();
		DataContext = ViewModel;
		_ = CargaInicialAsync(id);
	}

	private async Task CargaInicialAsync(PacienteId id) {
		PacienteDto? dto = await App.Repositorio.SelectPacienteWhereId(id);
		//MessageBox.Show(dto.ToString());
		if (dto == null) {
			MessageBox.Show("Paciente no encontrado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			Close();
			return;
		}
		//SelectedPacienteId = dto.Id;

		ViewModel = dto.ToViewModel();
		DataContext = ViewModel;
	}

	private async void ButtonGuardar(object sender, RoutedEventArgs e) {
		ResultWpf<UnitWpf> result = await ViewModel.GuardarAsync();
		result.MatchAndDo(
			caseOk => MessageBox.Show("Cambios guardados.", "Éxito", MessageBoxButton.OK),
			caseError => caseError.ShowMessageBox()
		);
	}

	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		Close();
	}

	private async void ButtonEliminar(object sender, RoutedEventArgs e) {
		if (MessageBox.Show("¿Eliminar paciente?",
			"Confirmación", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			return;

		//await App.Repositorio.DeletePacienteWhereId(ViewModel.Id);
		Close();
	}

	private void ButtonSolicitarTurno(object sender, RoutedEventArgs e) {
		//if (ViewModel.Id is int notNullId) {
		//	this.AbrirComoDialogo<SecretariaDisponibilidades>(SelectedPacienteId.Crear(notNullId));
		//}
	}
}
