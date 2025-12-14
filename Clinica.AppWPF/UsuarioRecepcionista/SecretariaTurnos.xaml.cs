using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using Microsoft.VisualBasic;

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
	// METHODS
	// ==========================================================
	private async Task EjecutarAccionAsync(Func<Task<ResultWpf<UnitWpf>>> accion) {
		ResultWpf<UnitWpf> result = await accion();
		result.MatchAndDo(
			ok => _ = VM.RefrescarTurnosAsync(),
			error => error.ShowMessageBox()
		);
	}
	private static string? PedirComentario(string titulo) {
		string comentario = Interaction.InputBox(titulo, "Comentario requerido", "");
		if (string.IsNullOrWhiteSpace(comentario)) return null;
		if (comentario.Length < 10) {
			MessageBox.Show("Debe completar un comentario válido.");
			return null;
		}
		return comentario;
	}


	// ==========================================================
	// BOTONES: DOMINIO
	// ==========================================================
	private async void Button_ConfirmarTurnoAsistencia(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		DateTime hoy = DateTime.Now;

		if (MessageBox.Show(
			$"¿Confirma que el paciente se presentó el día {hoy:d} a las {hoy:t}?",
			"Confirmación",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning
		) != MessageBoxResult.Yes)
			return;

		await EjecutarAccionAsync(() => VM.ConfirmarAsistenciaAsync(hoy));
	}


	private void Button_ReprogramarTurno(object sender, RoutedEventArgs e) {
		if (VM.SelectedTurno is null) {
			MessageBox.Show("No hay turno seleccionado.");
			return;
		}

		if (VM.SelectedTurno.PacienteRelacionado is null) {
			MessageBox.Show("Paciente no encontrado.");
			return;
		}

		this.NavegarA<SecretariaTurnosSacar>(VM.SelectedTurno);
	}

	private async void Button_CancelarTurno(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		string? comentario = PedirComentario("Ingrese la razón de la cancelación del turno:");
		if (comentario is null) return;

		await EjecutarAccionAsync(() => VM.CancelarTurnoAsync(comentario, DateTime.Now));
	}



	private async void Button_MarcarTurnoAusente(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();

		string? comentario = PedirComentario(
			"Ingrese algún comentario mínimo sobre este turno. ¿El paciente nunca se volvió a contactar?"
		);
		if (comentario is null) return;

		await EjecutarAccionAsync(() => VM.MarcarAusenteAsync(comentario, DateTime.Now));
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
	// BOTONES: ORDENAR
	// ==========================================================
	private GridViewColumnHeader? _lastHeaderClicked = null;
	private ListSortDirection _lastDirection = ListSortDirection.Ascending;

	private void ClickCabecera_OrdenarFilas(object sender, RoutedEventArgs e) {
		if (sender is not GridViewColumnHeader header || header.Tag == null) return;

		string sortBy = header.Tag.ToString()!;
		ListSortDirection direction = ListSortDirection.Ascending;

		if (_lastHeaderClicked == header && _lastDirection == ListSortDirection.Ascending)
			direction = ListSortDirection.Descending;

		VM.TurnosView.SortDescriptions.Clear();
		VM.TurnosView.SortDescriptions.Add(new SortDescription(sortBy, direction));
		VM.TurnosView.Refresh();

		_lastHeaderClicked = header;
		_lastDirection = direction;
	}


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ClickBoton_Home(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}
