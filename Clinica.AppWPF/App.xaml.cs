using System.Diagnostics;
using System.Windows;
//using Clinica.AppWPF.UsuarioSuperadmin;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.Infrastructure.IRepositorios;
using Clinica.AppWPF.Infrastructure.Repositorio;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF;




public partial class App : Application {
	public static ApiHelper Api = new();
	public static IRepositorioWPF Repositorio { get; } = new RepositorioWPF();

	public static UsuarioDbModel? UsuarioActivo = null;
	//public static Repositorio2024 BaseDeDatos;

	protected override void OnStartup(StartupEventArgs e) {
		base.OnStartup(e);

		// Captura cualquier error de binding
		PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;
		System.Diagnostics.PresentationTraceSources.Refresh();
		//System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
		//Api = new ApiHelper();
		//Repositorio = new WPFRepositorio(Api);


	}
	/*
	public static void UpdateLabelDataBaseModo(Label label) {
		if (App.BaseDeDatos is WPFRepositorio) {
			//if (App.Repositorio is BaseDeDatosJSON) {
			//label.Content = "Modo JSON";
			//} else if (App.Repositorio is RepositorioApi) {
			label.Content = "Modo API";
		} else {
			label.Content = "Elegir DB Modo";
		}
	}
	*/



	public void StyleButton_MouseEnter(object sender, RoutedEventArgs e) {
		SoundsService.PlayHoverSound();
	}
}
