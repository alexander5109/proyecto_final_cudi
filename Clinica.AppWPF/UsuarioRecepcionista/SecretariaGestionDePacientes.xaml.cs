using System.Windows;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class RecepcionistaGestionDePacientes : Window {
	public RecepcionistaGestionDePacientesViewModel VM { get; }

	public RecepcionistaGestionDePacientes() {
		InitializeComponent();
		VM = new RecepcionistaGestionDePacientesViewModel();
		DataContext = VM;

		Loaded += async (_, __) => await CargaInicialAsync();
	}

	private bool MostrarErrorSiCorresponde<T>(ResultWpf<T> result)
		=> result.MatchAndSet(
			ok => true,
			error => {
				error.ShowMessageBox();
				return false;
			}
		);



	private async Task CargaInicialAsync() {
		await VM.RefrescarPacientesAsync();
	}


	private void ButtonHome(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ButtonSalir(object sender, RoutedEventArgs e) => this.Salir();

	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<RecepcionistaPacienteFormulario>();
		_ = VM.RefrescarPacientesAsync();
	}
	private void ButtonModificarPaciente(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is null) return;
		this.AbrirComoDialogo<RecepcionistaPacienteFormulario>(VM.SelectedPaciente.Id);
		_ = VM.RefrescarPacientesAsync();
	}
	private void ButtonBuscarDisponibilidades(object sender, RoutedEventArgs e) {
		if (VM.SelectedPaciente is not null) {
			this.AbrirComoDialogo<SecretariaFormularioTurno>(VM.SelectedPaciente);
		} else {
			MessageBox.Show("No hay paciente seleecionado");
		}
	}

}
