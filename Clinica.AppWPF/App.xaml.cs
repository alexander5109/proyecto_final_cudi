using System.Diagnostics;
using System.Windows;
using Clinica.AppWPF.Clinica2024;
using Clinica.AppWPF.Infrastructure;
using static Clinica.AppWPF.Infrastructure.IWPFRepositorioInterfaces;

namespace Clinica.AppWPF;

public partial class App : Application {
	public static IWPFRepositorio Repositorio;
	public static ApiHelper Api;
	public static Repositorio2024 BaseDeDatos;

	protected override void OnStartup(StartupEventArgs e) {
		base.OnStartup(e);

		// Captura cualquier error de binding
		PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;
		System.Diagnostics.PresentationTraceSources.Refresh();
		System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
		Api = new ApiHelper();
		Repositorio = new WPFRepositorioApi(Api);


	}
	/*
	public static void UpdateLabelDataBaseModo(Label label) {
		if (App.BaseDeDatos is WPFRepositorioApi) {
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
