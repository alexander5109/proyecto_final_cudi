using System.Windows;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class SecretariaTurnos : Window {
	public SecretariaTurnosViewModel VM { get; }

	public SecretariaTurnos() {
		InitializeComponent();
		VM = new SecretariaTurnosViewModel();
		DataContext = VM;

		Loaded += async (_, __) => await CargaInicialAsync();
	}

	// ==========================================================
	// CARGA INICIAL
	// ==========================================================

	private async Task CargaInicialAsync() {
		await RefrescarTurnosAsync();
		//await CargarPacientesYMedicosOnce();



	}

	private async Task RefrescarTurnosAsync() {
		try {
			List<TurnoDbModel> turnosDto = await App.Repositorio.SelectTurnos();
			var lista = turnosDto.Select(t => new TurnoViewModel(t)).ToList();

			VM.CargarTurnos(lista);
			VM.SelectedTurno = null;
		} catch (Exception ex) {
			MessageBox.Show("Error cargando turnos: " + ex.Message);
		}
	}

	// ==========================================================
	// NAV
	// ==========================================================

	private void ButtonHome(object sender, RoutedEventArgs e)
		=> this.IrARespectivaHome();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e)
		=> this.Salir();

	// ==========================================================
	// HELPER PARA MOSTRAR ERRORES
	// ==========================================================

	private static bool MostrarErrorSiCorresponde<T>(ResultWpf<T> result) =>
		result.MatchAndSet(
			ok => true,
			error => {
				error.ShowMessageBox();
				return false;
			}
		);

	// ==========================================================
	// ACCIONES DE TURNOS
	// ==========================================================

	private async void Button_ConfirmarTurnoAsistencia(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

		var result = await App.Repositorio.MarcarTurnoComoConcretado(
			VM.SelectedTurno.Id,
			DateTime.Now,
			VM.SelectedTurno.OutcomeComentario
		);

		if (!MostrarErrorSiCorresponde(result)) return;

		await RefrescarTurnosAsync();
	}

	private async void Button_MarcarTurnoAusente(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

		var result = await App.Repositorio.MarcarTurnoComoAusente(
			VM.SelectedTurno.Id,
			DateTime.Now,
			VM.SelectedTurno.OutcomeComentario
		);

		if (!MostrarErrorSiCorresponde(result)) return;

		await RefrescarTurnosAsync();
	}

	private async void Button_ReprogramarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

		if (VM.SelectedTurno.PacienteRelacionado is null) {
			MessageBox.Show("Paciente no encontrado.");
			return;
		}

		if (string.IsNullOrWhiteSpace(VM.SelectedTurno.OutcomeComentario)) {
			MessageBox.Show("Debe completar un comentario para reprogramar el turno.");
			return;
		}

		// abre el formulario de reprogramación
		this.AbrirComoDialogo<SecretariaTurnosSacar>(VM.SelectedTurno);

		await RefrescarTurnosAsync();
	}

	private async void Button_CancelarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) return;

		if (string.IsNullOrWhiteSpace(VM.SelectedTurno.OutcomeComentario)) {
			MessageBox.Show("Debe completar un comentario para cancelar el turno.");
			return;
		}

		if (MessageBox.Show(
			"¿Desea cancelar el turno seleccionado?",
			"Confirmación",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning
		) != MessageBoxResult.Yes) return;

		var result = await App.Repositorio.CancelarTurno(
			VM.SelectedTurno.Id,
			DateTime.Now,
			VM.SelectedTurno.OutcomeComentario
		);

		if (!MostrarErrorSiCorresponde(result)) return;

		await RefrescarTurnosAsync();
	}
}
