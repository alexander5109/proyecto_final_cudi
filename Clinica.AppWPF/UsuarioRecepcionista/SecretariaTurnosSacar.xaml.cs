using System.Windows;
using Clinica.AppWPF.Infrastructure;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class SecretariaTurnosSacar : Window {
	internal SecretariaTurnosSacarViewModel VM { get; }
	//bool EsReprogramacion = false;
	readonly TurnoViewModel? TurnoOriginal;
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
		VM = new SecretariaTurnosSacarViewModel(turnoOriginal.PacienteRelacionado, turnoOriginal.EspecialidadCodigo);
		DataContext = VM;
		TurnoOriginal = turnoOriginal;

	}

	private async void ClickBoton_Consultar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		await VM.RefreshDisponibilidadesAsync();
	}



	private static bool MostrarErrorSiCorresponde<T>(ResultWpf<T> result)
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
			ResultWpf<TurnoDbModel> resultt = await App.Repositorio.ReprogramarTurno(
				TurnoOriginal.Id,
				DateTime.Now,
				TurnoOriginal.OutcomeComentario
			);
			if (!MostrarErrorSiCorresponde(resultt)) {
				this.Close();
			}
		}
		ResultWpf<TurnoDbModel> result = await App.Repositorio.AgendarNuevoTurno(
			VM.SelectedPaciente.Id,
			DateTime.Now,
			VM.SelectedDisponibilidad.Original
		);
		result.MatchAndDo(
			caseOk => {
				MessageBox.Show("Turno reservado exitosamente.", "Éxito", MessageBoxButton.OK);
				this.Close();
			},
			caseError => caseError.ShowMessageBox()
		);


	}

	private void ClickBoton_ModificarPaciente(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		if (VM.SelectedPaciente != null) {
			this.AbrirComoDialogo<SecretariaPacientesModificar>(VM.SelectedPaciente.Id);
		}
	}

	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e)
		=> this.Salir();
}