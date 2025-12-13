using System.Windows;
using System.Windows.Controls;
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

		Loaded += async (_, __) => await VM.RefrescarTurnosAsync();
	}

	// ==========================================================
	// BOTONES: SELECTED ITEM ACTIONS
	// ==========================================================
	private static bool MostrarErrorSiCorresponde<T>(ResultWpf<T> result) =>
		result.MatchAndSet(
			ok => true,
			error => {
				error.ShowMessageBox();
				return false;
			}
		);


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
			VM.SelectedTurno.Original.Id,
			hoy,
			VM.SelectedTurno.Original.OutcomeComentario
		);

		if (!MostrarErrorSiCorresponde(result)) return;

		await VM.RefrescarTurnosAsync();
	}

	private async void Button_MarcarTurnoAusente(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (VM.SelectedTurno is null) {
			MessageBox.Show("Esto no deberia aparecer nunca. Por que no hay selectedturno seleciconado?");
			return;
		}
		string comentario = Interaction.InputBox("Ingrese algun comentario minimo sobre este turno. El paciente nunca se volvio a contactar?", "Comentario requerido", "");
		if (comentario == "") {
			return;
		}
		if (comentario.Length < 10) {
			MessageBox.Show("Debe completar un comentario para marcar como ausente el turno.");
		} else {
			ResultWpf<TurnoDbModel> result = await App.Repositorio.MarcarTurnoComoAusente(
			VM.SelectedTurno.Original.Id,
			DateTime.Now,
			comentario
		);
			if (MostrarErrorSiCorresponde(result)) {
				await VM.RefrescarTurnosAsync();
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

		await VM.RefrescarTurnosAsync();
	}

	private async void Button_CancelarTurno(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (VM.SelectedTurno is null) {
			MessageBox.Show("Esto no deberia aparecer nunca. Por que no hay selectedturno seleciconado?");
			return;
		}

		string comentario = Interaction.InputBox("Ingrese la razón de la cancelación del turno:", "Comentario requerido", "");
		if (comentario == "") {
			return;
		}
		if (comentario.Length < 10) {
			MessageBox.Show("Debe completar un comentario para cancelar el turno.");
			return;
		} else {
			ResultWpf<TurnoDbModel> result = await App.Repositorio.CancelarTurno(
				VM.SelectedTurno.Original.Id,
				DateTime.Now,
				comentario
			);
			if (!MostrarErrorSiCorresponde(result)) return;
			await VM.RefrescarTurnosAsync();
		}

	}

	// ==========================================================
	// BOTONES: REFRESH
	// ==========================================================

	private bool _enCooldown;
	private async void ClickBoton_Refrescar(object sender, RoutedEventArgs e) {
		if (_enCooldown)
			return;
		try {
			_enCooldown = true;
			if (sender is Button btn)
				btn.IsEnabled = false;
			await VM.RefrescarTurnosAsync();
		} finally {
			await Task.Delay(2000);
			if (sender is Button btn)
				btn.IsEnabled = true;

			_enCooldown = false;
		}
	}


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================

	private void ClickBoton_Home(object sender, RoutedEventArgs e)
		=> this.IrARespectivaHome();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e)
		=> this.Salir();






}
