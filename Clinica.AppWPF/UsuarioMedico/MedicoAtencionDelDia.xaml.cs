using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.AppWPF.UsuarioMedico;

public partial class MedicoAtencionDelDia : Window {
	public MedicoAtencionDelDiaVM VM { get; }

	public MedicoAtencionDelDia(MedicoId2025 medicoId) {
		InitializeComponent();
		VM = new MedicoAtencionDelDiaVM(medicoId);
		DataContext = VM;
		Loaded += async (_, __) => await VM.RefrescarTodoAsync();
	}



	// ==========================================================
	// BOTONES: PERSISTENCIA
	// ==========================================================

	async private void ClickBoton_ConfirmarObservacion(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		ResultWpf<UnitWpf> result = await VM.ConfirmarDiagnosticoAsync();
		result.MatchAndDo(
			async _ => {
				MessageBox.Show("Cambios guardados.", "Éxito");
			},
			err => err.ShowMessageBox()
		);


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
			await VM.RefrescarTodoAsync();
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
	private void ClickBoton_Home(object sender, RoutedEventArgs e) => this.IrARespectivaHome();
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
}
