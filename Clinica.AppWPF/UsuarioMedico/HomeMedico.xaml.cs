using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.DbModels;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioMedico;

public partial class HomeMedico : Window {
	private readonly HomeMedicoVM vm;

	public HomeMedico() {
		InitializeComponent();
		vm = new HomeMedicoVM();
		DataContext = vm;

		Loaded += async (_, __) => {
			if (App.UsuarioActivo?.MedicoRelacionadoId is MedicoId2025 medicoId) {
				await vm.CargarMiPerfil(medicoId);
			} else {
				MessageBox.Show("Su usuario no tiene un médico relacionado.\nNo tiene permisos para ver esta sección todavía.\nConsulte al personal administrativo.");
				Close();
			}
		};
	}


	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	private void ClickBoton_Salir(object sender, RoutedEventArgs e) => this.Salir();
	private void ClickBoton_Logout(object sender, RoutedEventArgs e) => this.CerrarSesion();
	private void ClickBoton_AtencionDelDia(object sender, RoutedEventArgs e) {
		SoundsService.PlayClickSound();
		if (App.UsuarioActivo?.MedicoRelacionadoId is not MedicoId2025 medicoId) {
			MessageBox.Show("Su usuario no tiene una entidad de médico relacionada. \nComuniquese con un administrativo.");
			//this.CerrarSesion();
			return;
		}
		this.NavegarA<MedicoAtencionDelDia>(medicoId);



	}
}