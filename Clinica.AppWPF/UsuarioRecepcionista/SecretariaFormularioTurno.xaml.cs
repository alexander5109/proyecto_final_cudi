using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeEnum;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class SecretariaFormularioTurno : Window {
	internal MyViewModel VM { get; }
    //bool EsReprogramacion = false;
    readonly TurnoViewModel? TurnoOriginal;
	public SecretariaFormularioTurno(PacienteDbModel paciente) {
		InitializeComponent();
		VM = new MyViewModel(paciente);
		DataContext = VM;
		TurnoOriginal = null;
	}

	public SecretariaFormularioTurno(PacienteDbModel paciente, TurnoViewModel turnoOriginal) {
		InitializeComponent();
		VM = new MyViewModel(paciente, turnoOriginal.EspecialidadCodigo);
		DataContext = VM;
		TurnoOriginal = turnoOriginal;

	}

	private async void ButtonConsultar(object sender, RoutedEventArgs e) {
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


	async private void Click_AgendarNuevoTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedDisponibilidad is null) return;

		SoundsService.PlayClickSound();
		if (TurnoOriginal is not null) {
			ResultWpf<TurnoDbModel> resultt = await App.Repositorio.ReprogramarTurno(
				TurnoOriginal.Id,
				DateTime.Now,
				TurnoOriginal.OutcomeComentario
			);
			if (MostrarErrorSiCorresponde(resultt)) {
				this.Close();
			}
		}
		ResultWpf<TurnoDbModel> result = await App.Repositorio.AgendarNuevoTurno(
			VM.SelectedPaciente.Id,
			DateTime.Now,
			VM.SelectedDisponibilidad.Original
		);
		result.MatchAndDo(
			caseOk => MessageBox.Show("Turno reservado exitosamente.", "Éxito", MessageBoxButton.OK),
			caseError => caseError.ShowMessageBox()
		);


	}

	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		if (VM.SelectedPaciente != null) {
			this.AbrirComoDialogo<RecepcionistaPacienteFormulario>(VM.SelectedPaciente.Id);
		}
	}

	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		Close();
	}

	private void ButtonSalir(object sender, RoutedEventArgs e)
		=> this.Salir();
}