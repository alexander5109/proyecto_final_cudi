using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Clinica.AppWPF.Infrastructure;
using GestorDeTecnicatura;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaPacientes : Window {
	private TurnoDto? SelectedTurno;
	private PacienteDto? SelectedPaciente;
	private MedicoDto? MedicoRelacionado;
	private ICollectionView __pacientesView;

	public SecretariaPacientes() {
		InitializeComponent();
		_ = CargaInicialAsync();
	}

	private void FilterTextChanged(object sender, TextChangedEventArgs e) {
		FormMapper.ApplyFilters<PacienteDto>(__pacientesView, this, "pacientesFilterBy");
	}


	private async Task CargaInicialAsync() {
		pacientesListView.ItemsSource = await App.Repositorio.SelectPacientes();
	}

	private void ActualizarPacienteUI() {
		buttonModificarPaciente.IsEnabled = SelectedPaciente != null;
	}

	private async Task ActualizarTurnosUIAsync() {
		if (SelectedPaciente != null) {
			turnosListView.ItemsSource = CollectionViewSource.GetDefaultView(await App.Repositorio.SelectTurnosWherePacienteId(SelectedPaciente.Id));
		} else {
			turnosListView.ItemsSource = null;
		}
	}

	private async void Window_ActivatedAsync(object sender, EventArgs e) {
		//App.UpdateLabelDataBaseModo(labelBaseDeDatosModo);
		ActualizarPacienteUI();
		await ActualizarTurnosUIAsync();
	}

	private async void ListViewPacientes_SelectionChangedAsync(object sender, SelectionChangedEventArgs e) {
		SelectedPaciente = pacientesListView.SelectedItem as PacienteDto;
		await ActualizarTurnosUIAsync();
		ActualizarPacienteUI();
	}



	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (SelectedPaciente != null) {
			this.AbrirComoDialogo<SecretariaPacientesModificar>(SelectedPaciente.Id);
		}
	}

	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) => this.AbrirComoDialogo<SecretariaPacientesModificar>();
	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();
	private void ButtonHome(object sender, RoutedEventArgs e) => this.VolverARespectivoHome();

	private void ButtonReservarTurno(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (SelectedPaciente is null)
			return;

		//MessageBox.Show($"Gestionando turno para el paciente: {SelectedPaciente.Nombre} {SelectedPaciente.Apellido}", "Gestión de Turno", MessageBoxButton.OK, MessageBoxImage.Information);

		this.AbrirComoDialogo<WindowGestionTurno>(SelectedPaciente);
	}

	private void ButtonReservarTurn2(object sender, RoutedEventArgs e) {

	}
}
