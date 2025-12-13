using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Microsoft.VisualBasic;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class SecretariaTurnos : Window {
	public SecretariaTurnosViewModel VM { get; }

	public SecretariaTurnos() {
		InitializeComponent();
		VM = new SecretariaTurnosViewModel();
		DataContext = VM;

		Loaded += async (_, __) => await RefrescarTurnosAsync();
	}

	private async void ClickBoton_Refrescar(object sender, RoutedEventArgs e) {
		await RefrescarTurnosAsync();

	}

	private async Task RefrescarTurnosAsync() {
		try {
			List<TurnoDbModel> turnosDto = await App.Repositorio.SelectTurnos();
            List<TurnoViewModel> lista = turnosDto.Select(t => new TurnoViewModel(t)).ToList();

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
		SoundsService.PlayClickSound();
		if (VM.SelectedTurno is null) {
			MessageBox.Show("Esto no deberia aparecer nunca. Por que no hay selectedturno seleciconado?");
			return;
		}
		DateTime hoy = DateTime.Now;

		if (MessageBox.Show(
			$"¿Confirma que el paciente se presentó en el dia de la fecha {hoy:d} a las {hoy:t}?",
			"Confirmación",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning
		) != MessageBoxResult.Yes) return;

		ResultWpf<TurnoDbModel> result = await App.Repositorio.MarcarTurnoComoConcretado(
			VM.SelectedTurno.Id,
			hoy,
			VM.SelectedTurno.OutcomeComentario
		);

		if (!MostrarErrorSiCorresponde(result)) return;

		await RefrescarTurnosAsync();
	}

	private async void Button_MarcarTurnoAusente(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (VM.SelectedTurno is null) {
			MessageBox.Show("Esto no deberia aparecer nunca. Por que no hay selectedturno seleciconado?");
			return;
		}
		string comentario = Interaction.InputBox("Ingrese algun comentario minimo sobre este turno. El paciente nunca se volvio a contactar?", "Cancelar turno", "");
		if (comentario == null || comentario.Length < 10) {
			MessageBox.Show("Debe completar un comentario para marcar como ausente el turno.");
		} else {
			ResultWpf<TurnoDbModel> result = await App.Repositorio.MarcarTurnoComoAusente(
			VM.SelectedTurno.Id,
			DateTime.Now,
			comentario
		);
			if (MostrarErrorSiCorresponde(result)) {
				await RefrescarTurnosAsync();
			}

		}
	}

	private async void Button_ReprogramarTurno(object sender, RoutedEventArgs e) {
		//SoundsService.PlayClickSound();
		if (VM.SelectedTurno is null) {
			MessageBox.Show("Esto no deberia aparecer nunca. Por que no hay selectedturno seleciconado?");
			return;
		}

		if (VM.SelectedTurno.PacienteRelacionado is null) {
			MessageBox.Show("Paciente no encontrado.");
			return;
		}



		// abre el formulario de reprogramación
		this.AbrirComoDialogo<SecretariaTurnosSacar>(VM.SelectedTurno); //alreadyplays sound

		await RefrescarTurnosAsync();
	}

	private async void Button_CancelarTurno(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (VM.SelectedTurno is null) {
			MessageBox.Show("Esto no deberia aparecer nunca. Por que no hay selectedturno seleciconado?");
			return;
		}

		string comentario = Interaction.InputBox("Ingrese la razón de la cancelación del turno:", "Cancelar turno", "CancelarOperacion");
		if (comentario == "CancelarOperacion") {
			return;
		}
		if (comentario == null || comentario.Length < 10) {
			MessageBox.Show("Debe completar un comentario para cancelar el turno.");
			return;
		} else {
			ResultWpf<TurnoDbModel> result = await App.Repositorio.CancelarTurno(
				VM.SelectedTurno.Id,
				DateTime.Now,
				comentario
			);
			if (!MostrarErrorSiCorresponde(result)) return;
			await RefrescarTurnosAsync();
		}

	}
}
