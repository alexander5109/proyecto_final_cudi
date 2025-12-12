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

	public SecretariaFormularioTurno(TurnoViewModel turnoOriginal) {
		InitializeComponent();
		VM = new MyViewModel(turnoOriginal.PacienteRelacionado, turnoOriginal.EspecialidadCodigo);
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
			this.AbrirComoDialogo<RecepcionistaPacienteFormulario>(VM.SelectedPaciente.Id);
		}
	}

	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e)
		=> this.Salir();
}