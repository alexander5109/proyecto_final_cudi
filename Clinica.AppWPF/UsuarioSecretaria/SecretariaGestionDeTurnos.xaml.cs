using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.Comun;
using Clinica.Shared.Dtos;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaGestionDeTurnos : Window {
	public SecretariaGestionDeTurnosViewModel VM { get; }

	public SecretariaGestionDeTurnos() {
		InitializeComponent();
		VM = new SecretariaGestionDeTurnosViewModel();
		DataContext = VM;

		Loaded += async (_, __) => await CargaInicialAsync();
	}


	private bool MostrarErrorSiCorresponde<T>(Result<T> result)
	=> result.Match(
		ok => true,
		error => {
			MessageBox.Show(error ?? "Error desconocido.", "Error");
			return false;
		}
	);


	private async Task CargaInicialAsync() {
		await RefrescarPacientesAsync();
		await RefrescarTurnosAsync();
	}
	private async Task RefrescarPacientesAsync() {
		try {
            List<PacienteDto> pacientes = await App.Repositorio.SelectPacientes();
			VM.PacientesList = pacientes.ToList();
		} catch (Exception ex) {
			MessageBox.Show("Error cargando pacientes: " + ex.Message);
		}
	}
	private async Task RefrescarTurnosAsync() {
		try {
			List<TurnoDto> turnosDto = await App.Repositorio.SelectTurnos();

			VM.TurnosList = turnosDto
				.Select(dto => new TurnoVM(dto))
				.ToList();
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
	private void ButtonReservarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is null) return;
		this.AbrirComoDialogo<SecretariaBuscadorDeDisponibilidades>(VM.SelectedPaciente);
		//_ = VM.RefrescarTurnosAsync();
	}
	private async void Button_ConfirmarTurnoAsistencia(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

        TurnoVM turno = VM.SelectedTurno;

		VM.IndicarAccionRequiereComentario(false);

        Result<TurnoDto> result = await App.Repositorio.MarcarTurnoComoConcretado(
			turno.Id,
			DateTime.Now
		);

		if (MostrarErrorSiCorresponde(result))
			return;

		await RefrescarTurnosAsync();
	}


	private async void Button_MarcarTurnoAusente(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

		VM.IndicarAccionRequiereComentario(false);

        //Aca no es obligatorio el comentario.

        Result<TurnoDto> result = await App.Repositorio.MarcarTurnoComoAusente(
			VM.SelectedTurno.Id,
			DateTime.Now,
			comentarioTextBox.Text
		);

		if (MostrarErrorSiCorresponde(result))
			return;

		await RefrescarTurnosAsync();
	}


	private async void Button_ReprogramarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

		VM.IndicarAccionRequiereComentario(true);

		if (string.IsNullOrWhiteSpace(comentarioTextBox.Text)) {
			MessageBox.Show("Debe completar un comentario para reprogramar el turno.");
			return;
		}

        Result<TurnoDto> result = await App.Repositorio.ReprogramarTurno(
			VM.SelectedTurno.Id,
			DateTime.Now,
			comentarioTextBox.Text
		);

		if (MostrarErrorSiCorresponde(result))
			return;

		await RefrescarTurnosAsync();
	}


	private async void Button_CancelarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

		VM.IndicarAccionRequiereComentario(true);

		if (string.IsNullOrWhiteSpace(comentarioTextBox.Text)) {
			MessageBox.Show("Debe completar un comentario para cancelar el turno.");
			return;
		}

        Result<TurnoDto> result = await App.Repositorio.CancelarTurno(
			VM.SelectedTurno.Id,
			DateTime.Now,
			comentarioTextBox.Text
		);

		if (MostrarErrorSiCorresponde(result))
			return;

		await RefrescarTurnosAsync();
	}

	private void ButtonSolicitarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is not null) {
			this.AbrirComoDialogo<SecretariaBuscadorDeDisponibilidades>(VM.SelectedPaciente);
		}
	}
}
