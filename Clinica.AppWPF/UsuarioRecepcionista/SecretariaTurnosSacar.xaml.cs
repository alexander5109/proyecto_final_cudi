using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using Microsoft.VisualBasic;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class SecretariaTurnosSacar : Window {
	internal SecretariaTurnosSacarViewModel VM { get; }
	//bool EsReprogramacion = false;
	readonly TurnoViewModel? TurnoOriginal;



	// ==========================================================
	// CONSTRUCTORES
	// ==========================================================
	public SecretariaTurnosSacar(PacienteDbModel paciente) {
		InitializeComponent();
		VM = new SecretariaTurnosSacarViewModel(paciente);
		DataContext = VM;
		TurnoOriginal = null;
	}

	public SecretariaTurnosSacar(TurnoViewModel turnoOriginal) {
		InitializeComponent();
		if (turnoOriginal.PacienteRelacionado == null) {
			MessageBox.Show("por que nulo el paciente?");
			throw new Exception("En realidad este scenario es imposbiel porque quien se encarga de llamar a este constructor valida al paciente tambien");
		}
		VM = new SecretariaTurnosSacarViewModel(turnoOriginal.PacienteRelacionado, turnoOriginal.Original.EspecialidadCodigo);
		DataContext = VM;
		TurnoOriginal = turnoOriginal;

	}



	// ==========================================================
	// BOTONES: REFRESH
	// ==========================================================

	private bool _enCooldown;
	private async void ClickBoton_Consultar(object sender, RoutedEventArgs e) {
		if (_enCooldown)
			return;
		try {
			_enCooldown = true;
			if (sender is Button btn)
				btn.IsEnabled = false;
			await VM.RefreshDisponibilidadesAsync();
		} finally {
			await Task.Delay(2000);
			if (sender is Button btn)
				btn.IsEnabled = true;

			_enCooldown = false;
		}
	}


	// ==========================================================
	// BOTONES: PERSISTENCIA
	// ==========================================================
	private static bool MatchAndSetBooleano<T>(ResultWpf<T> result)
		=> result.MatchAndSet(
			ok => true,
			error => {
				error.ShowMessageBox();
				return false;
			}
		);

	async private void ClickBoton_NuevoTurno(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		if (VM.SelectedDisponibilidad is null) {
			MessageBox.Show("No hay disponibilidad seleccionada");
			return;
		}

		if (TurnoOriginal is not null) {

			string comentario = Interaction.InputBox(
				"Ingrese la razón de la reprogramacion del turno:",
				"Continuar con la reprogramación del turno",
				""
			);

			if (comentario == "") {
				return;
			}
			if (comentario.Length < 7) {
				MessageBox.Show("Debe completar un comentario para cancelar el turno.");
				return;
			}

			ResultWpf<UnitWpf> resultt = await App.Repositorio.ReprogramarTurno(
				TurnoOriginal.Original.Id,
				DateTime.Now,
				comentario
			);
			if (!MatchAndSetBooleano(resultt)) {
				//this.NavegarA<SecretariaPacientes>();
				this.IrARespectivaHome();
				return;
			}
		}

		ResultWpf<UnitWpf> result = await App.Repositorio.AgendarNuevoTurno(
			VM.SelectedPaciente.Id,
			DateTime.Now,
			VM.SelectedDisponibilidad.Original
		);
		result.MatchAndDo(
			caseOk => {
				MessageBox.Show("Turno reservado exitosamente.", "Éxito", MessageBoxButton.OK);
				//this.NavegarA<SecretariaPacientes>();
				this.IrARespectivaHome();
			},
			caseError => {
				caseError.ShowMessageBox();
				MessageBox.Show($"VM.SelectedDisponibilidad.Original: {VM.SelectedDisponibilidad.Original}");
			}
		);

		//App.Repositorio.Refres();

	}

	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.NavegarA<SecretariaPacientes>();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}