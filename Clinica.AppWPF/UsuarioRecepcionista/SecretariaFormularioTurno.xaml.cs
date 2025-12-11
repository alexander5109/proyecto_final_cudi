using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class SecretariaFormularioTurno : Window {
	internal MyViewModel VM { get; }

	public SecretariaFormularioTurno(PacienteDbModel paciente) {
		InitializeComponent();
		VM = new MyViewModel(paciente);
		DataContext = VM;
	}

	private async void ButtonConsultar(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		await VM.RefreshDisponibilidadesAsync();
	}

	async private void Click_AgendarNuevoTurno(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		if (VM.SelectedDisponibilidad is null) return;
		DisponibilidadEspecialidadModelView d = VM.SelectedDisponibilidad;
		MessageBox.Show($"Reservando turno: {d.Fecha} {d.Hora}", "Reservar");
		Close();
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