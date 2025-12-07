using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioSecretaria;

public partial class SecretariaGeneral : Window {
	public SecretariaGeneralViewModel VM { get; }

	public SecretariaGeneral() {
		InitializeComponent();
		VM = new SecretariaGeneralViewModel();
		DataContext = VM;

		//Loaded += async (_, __) => await VM.CargaInicialAsync();
	}

	private void ButtonHome(object sender, RoutedEventArgs e)
		=> this.VolverARespectivoHome();

	private void ButtonSalir(object sender, RoutedEventArgs e)
		=> this.Salir();

	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<SecretariaPacientesModificar>();
		//_ = VM.RefrescarPacientesAsync();
	}

	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is null) return;
		this.AbrirComoDialogo<SecretariaPacientesModificar>(VM.SelectedPaciente.Id);
		//_ = VM.RefrescarPacientesAsync();
	}

	private void ButtonReservarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is null) return;
		this.AbrirComoDialogo<SecretariaConsultaDisponibilidades>(VM.SelectedPaciente);
		//_ = VM.RefrescarTurnosAsync();
	}

    private void Button_ConfirmarTurnoAsistencia(object sender, RoutedEventArgs e) {

	}

    private void Button_MarcarTurnoAusente(object sender, RoutedEventArgs e) {

	}

    private void Button_ReprogramarTurno(object sender, RoutedEventArgs e) {

		VM.IndicarAccionRequiereComentario(true);

		if (VM.ComentarioObligatorio && string.IsNullOrWhiteSpace(comentarioTextBox.Text)) {
			MessageBox.Show("Debe completar un comentario para reprogramar el turno.");
			return;
		}

		// Lógica API:
		// await App.Repositorio.ReprogramarTurno(...);

		//await ActualizarTurnosUIAsync();
	}

    private void Button_CancelarTurno(object sender, RoutedEventArgs e) {

	}
}
