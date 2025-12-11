using System.Windows;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class RecepcionistaGestionDeTurnos : Window {
	public RecepcionistaGestionDeTurnosViewModel VM { get; }

	public RecepcionistaGestionDeTurnos() {
		InitializeComponent();
		VM = new RecepcionistaGestionDeTurnosViewModel();
		DataContext = VM;

		Loaded += async (_, __) => await CargaInicialAsync();
	}



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


	private void ButtonHome(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();


	private static bool MostrarErrorSiCorresponde<T>(ResultWpf<T> result)
		=> result.MatchAndSet(
			ok => true,
			error => {
				error.ShowMessageBox();
				return false;
			}
		);


	private async void Button_ConfirmarTurnoAsistencia(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is not null) {
			//MARCAR EL ANTERIOR COMO REPROGRAMADO

			VM.IndicarAccionRequiereComentario(false);

			ResultWpf<TurnoDbModel> result = await App.Repositorio.MarcarTurnoComoConcretado(
				VM.SelectedTurno.Id,
				DateTime.Now,
				VM.SelectedTurno.OutcomeComentario
			);
			if (!MostrarErrorSiCorresponde(result))
				return;


		}

		await RefrescarTurnosAsync();
	}


	private async void Button_MarcarTurnoAusente(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

		VM.IndicarAccionRequiereComentario(false);

		ResultWpf<TurnoDbModel> result = await App.Repositorio.MarcarTurnoComoAusente(
			VM.SelectedTurno.Id,
			DateTime.Now,
			VM.SelectedTurno.OutcomeComentario
		);
		if (!MostrarErrorSiCorresponde(result))
			return;

		await RefrescarTurnosAsync();
	}

	private async void Button_ReprogramarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;
		if (VM.SelectedPaciente is null) return;

		VM.IndicarAccionRequiereComentario(true);

		if (string.IsNullOrWhiteSpace(VM.SelectedTurno.OutcomeComentario)) {
			MessageBox.Show("Debe completar un comentario para reprogramar el turno.");
			return;
		}

		this.AbrirComoDialogo<SecretariaFormularioTurno>(VM.SelectedPaciente, VM.SelectedTurno);

		await RefrescarTurnosAsync();
		VM.SelectedTurno = null;
	}


	private async void Button_CancelarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

		VM.IndicarAccionRequiereComentario(true);

		if (string.IsNullOrWhiteSpace(VM.SelectedTurno.OutcomeComentario)) {
			MessageBox.Show("Debe completar un comentario para cancelar el turno.");
			return;
		}

		if (MessageBox.Show(
				"¿Desea cancelar el turno seleccionado?",
				"Confirmación",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning) != MessageBoxResult.Yes)
			return;

		var result = await App.Repositorio.CancelarTurno(
			VM.SelectedTurno.Id,
			DateTime.Now,
			VM.SelectedTurno.OutcomeComentario
		);

		if (!MostrarErrorSiCorresponde(result))
			return;

		await RefrescarTurnosAsync();
		VM.SelectedTurno = null;
	}



	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<RecepcionistaPacienteFormulario>();
		//_ = VM.RefrescarPacientesAsync();
	}
	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is null) return;
		this.AbrirComoDialogo<RecepcionistaPacienteFormulario>(VM.SelectedPaciente.Id);
		//_ = VM.RefrescarPacientesAsync();
	}
	private void ButtonBuscarDisponibilidades(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is null) return;
		this.AbrirComoDialogo<SecretariaFormularioTurno>(VM.SelectedPaciente.Id);
		//_ = VM.RefrescarTurnosAsync();
	}

	private void PuedeCancelarTurno(object sender, RoutedEventArgs e) {

	}
}
