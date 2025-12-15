using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using Microsoft.VisualBasic;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioRecepcionista;

public partial class DialogoTurnoProgramar : Window {
	internal DialogoTurnoProgramarVM VM { get; }
	//bool EsReprogramacion = false;



	// ==========================================================
	// CONSTRUCTORES
	// ==========================================================
	public DialogoTurnoProgramar(PacienteDbModel paciente) {
		InitializeComponent();
		VM = new DialogoTurnoProgramarVM(paciente);
		DataContext = VM;
	}

	public DialogoTurnoProgramar(TurnoViewModel turnoAReprogramar) {
		InitializeComponent();
		if (turnoAReprogramar.PacienteRelacionado == null) {
			MessageBox.Show("por que nulo el paciente?");
			throw new Exception("En realidad este scenario es imposbiel porque quien se encarga de llamar a este constructor valida al paciente tambien");
		}
		VM = new DialogoTurnoProgramarVM(turnoAReprogramar.PacienteRelacionado, turnoAReprogramar.Original);
		DataContext = VM;

	}



	// ==========================================================
	// BOTONES: REFRESH
	// ==========================================================

	private async void ClickBoton_Consultar(object sender, RoutedEventArgs e) {
		if (!VM.BotonBuscar_Enabled)
			return;

		try {
			VM.SetCooldown(true);
			await VM.RefreshDisponibilidadesAsync();
		} finally {
			await Task.Delay(2000);
			VM.SetCooldown(false);
		}
	}



	// ==========================================================
	// METHODS
	// ==========================================================

	private static string? PedirComentario(string titulo) {
		string comentario = Interaction.InputBox(titulo, "Comentario requerido", "");
		if (string.IsNullOrWhiteSpace(comentario)) return null;
		if (comentario.Length < 10) {
			MessageBox.Show("Debe completar un comentario válido.");
			return null;
		}
		return comentario;
	}

	private async Task EjecutarAccionAsync(Func<Task<ResultWpf<UnitWpf>>> accion) {
		ResultWpf<UnitWpf> result = await accion();
		result.MatchAndDo(
			ok => { this.Close(); },
			error => error.ShowMessageBox()
		);
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

		if (VM.HayQueMarcarComoReprogramado) {
			string? comentario = PedirComentario("Ingrese la razón de la reprogramacion del turno:");
			if (comentario is null) return;

			DateTime hoy = DateTime.Now;

			await EjecutarAccionAsync(() => VM.ConfirmarReprogramacionAsync(hoy, comentario));

		}
		ResultWpf<UnitWpf> result = await App.Repositorio.AgendarNuevoTurno(
			VM.SelectedPaciente.Id,
			DateTime.Now,
			VM.SelectedDisponibilidad.Original
		);
		result.MatchAndDo(
			caseOk => {
				MessageBox.Show("Turno reservado exitosamente.", "Éxito", MessageBoxButton.OK);
				//this.NavegarA<GestionPacientes>();
				//this.IrARespectivaHome();
				this.Cerrar();
			},
			caseError => {
				caseError.ShowMessageBox();
				//MessageBox.Show($"VM.SelectedDisponibilidad.Original: {VM.SelectedDisponibilidad.Original}");
			}
		);





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

		VM.DisponibilidadesView?.SortDescriptions.Clear();
		VM.DisponibilidadesView?.SortDescriptions.Add(new SortDescription(sortBy, direction));
		VM.DisponibilidadesView?.Refresh();

		_lastHeaderClicked = header;
		_lastDirection = direction;
	}




	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	//private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.NavegarA<GestionPacientes>();
	private void ClickBoton_Cancelar(object sender, RoutedEventArgs e) => this.Cerrar();

	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}