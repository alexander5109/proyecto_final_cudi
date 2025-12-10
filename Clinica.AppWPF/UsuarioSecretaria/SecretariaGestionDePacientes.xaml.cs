using System.Windows;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaGestionDePacientes : Window {
	public SecretariaGestionDePacientesViewModel VM { get; }

	public SecretariaGestionDePacientes() {
		InitializeComponent();
		VM = new SecretariaGestionDePacientesViewModel();
		DataContext = VM;

		Loaded += async (_, __) => await CargaInicialAsync();
	}

	private bool MostrarErrorSiCorresponde<T>(ResultWpf<T> result)
		=> result.MatchAndSet(
			ok => true,
			error => {
				error.ShowMessageBox();
				return false;
			}
		);



	private async Task CargaInicialAsync() {
		await RefrescarPacientesAsync();
		await RefrescarTurnosAsync();
	}
	private async Task RefrescarPacientesAsync() {
		try {
			List<PacienteDbModel> pacientes = await App.Repositorio.SelectPacientes();
			VM.PacientesList = [.. pacientes];
		} catch (Exception ex) {
			MessageBox.Show("Error cargando pacientes: " + ex.Message);
		}
	}
	private async Task RefrescarTurnosAsync() {
		try {
			List<TurnoDbModel> turnosDto = await App.Repositorio.SelectTurnos();

			VM.TurnosList = [.. turnosDto.Select(dto => new TurnoViewModel(dto))];
		} catch (Exception ex) {
			MessageBox.Show("Error cargando turnos: " + ex.Message);
		}
	}


	private void ButtonHome(object sender, RoutedEventArgs e) => this.VolverARespectivoHome();
	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();

	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<SecretariaPacienteFormulario>();
		//_ = VM.RefrescarPacientesAsync();
	}
	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is null) return;
		this.AbrirComoDialogo<SecretariaPacienteFormulario>(VM.SelectedPaciente.Id);
		//_ = VM.RefrescarPacientesAsync();
	}
	private void ButtonBuscarDisponibilidades(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is null) return;
		this.AbrirComoDialogo<SecretariaDisponibilidades>(VM.SelectedPaciente.Id);
		//_ = VM.RefrescarTurnosAsync();
	}
}
