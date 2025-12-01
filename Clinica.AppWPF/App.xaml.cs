using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF;

public partial class App : Application {
	public static BaseDeDatosInterface BaseDeDatos;
	public static ApiHelper Api;


	protected override void OnStartup(StartupEventArgs e) {
		base.OnStartup(e);

		// Captura cualquier error de binding
		PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;
		System.Diagnostics.PresentationTraceSources.Refresh();
		System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
		Api = new ApiHelper();
		BaseDeDatos = new BaseDeDatosWebAPI();


	}

	public static void UpdateLabelDataBaseModo(Label label) {
		if (App.BaseDeDatos is BaseDeDatosWebAPI) {
			//if (App.BaseDeDatos is BaseDeDatosJSON) {
			//label.Content = "Modo JSON";
			//} else if (App.BaseDeDatos is BaseDeDatosWebAPI) {
			label.Content = "Modo API";
		} else {
			label.Content = "Elegir DB Modo";
		}
	}




	public void StyleButton_MouseEnter(object sender, RoutedEventArgs e) {
		SoundsService.PlayHoverSound();
	}
}
