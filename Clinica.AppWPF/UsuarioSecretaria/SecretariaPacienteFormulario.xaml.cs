using System.Windows;
using Clinica.AppWPF;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioSecretaria;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaPacienteFormulario : Window {
	public SecretariaPacienteFormularioViewModel ViewModel { get; private set; } = new();
	//private PacienteId? SelectedPacienteId = null;

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
		if (ViewModel.Id is PacienteId pacienteId) {
			//do update:
			Result<Paciente2025> pacienteUpdateValidation = ViewModel.ToDomain();
			if (pacienteUpdateValidation.IsError) {
				MessageBox.Show(pacienteUpdateValidation.UnwrapAsError(), "Error", MessageBoxButton.OK);
				return;
			}
            Paciente2025Agg aggregate = new(pacienteId, pacienteUpdateValidation.UnwrapAsOk());
			Result<Unit> persistenciaValidation = await App.Repositorio.UpdatePacienteWhereId(aggregate);
			if (persistenciaValidation.IsError) {
				MessageBox.Show("No se pudo guardar.", "Error", MessageBoxButton.OK);
				return;
			}
		} else {
			// create new:
			Result<Paciente2025> pacienteCreateValidation = ViewModel.ToDomain();
			if (pacienteCreateValidation.IsError) {
				MessageBox.Show(pacienteCreateValidation.UnwrapAsError(), "Error", MessageBoxButton.OK);
				return;
			}
			Paciente2025 domainEntity = pacienteCreateValidation.UnwrapAsOk();
            Result<PacienteId> resultado = await App.Repositorio.InsertPacienteReturnId(domainEntity);
			resultado.Match(
				ok => ViewModel.Id = ok,
				error => MessageBox.Show("No se pudo guardar: " + error, "Error", MessageBoxButton.OK)
			);
		}
		MessageBox.Show("Cambios guardados.");
		Close();
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
		//	this.AbrirComoDialogo<SecretariaBuscadorDeDisponibilidades>(SelectedPacienteId.Crear(notNullId));
		//}
	}
}
