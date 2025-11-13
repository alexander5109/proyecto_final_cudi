using Clinica.AppWPF.Entidades;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Clinica.AppWPF;
public partial class App : Application {


	protected override void OnStartup(StartupEventArgs e) {
		base.OnStartup(e);

		// Captura cualquier error de binding
		PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;
		System.Diagnostics.PresentationTraceSources.Refresh();
		System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
	}

	public static bool SoundOn = true;
	public static BaseDeDatosAbstracta BaseDeDatos;
	public static bool UsuarioLogueado = false;
	public static MediaPlayer Sonidito = new();

	private static string GetSoundPath(string fileName) {
		string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PublishedResources", "sonidos", fileName);

		if (!File.Exists(fullPath))
			throw new FileNotFoundException($"Sound file not found: {fullPath}");

		return fullPath;
	}

	public void StyleButton_MouseEnter(object sender, RoutedEventArgs e) {
		if (SoundOn)
			PlaySound("uclicknofun.wav");
		//System.Media.SystemSounds.Asterisk.Play();
		//PlaySound("uclicknofun.wav");
		//PlaySound(@"C:\Windows\Media\chimes.wav");
		//PlaySystemSound("Windows Ding.wav");
		//System.Media.SystemSounds.Beep.Play();
		//System.Media.SystemSounds.Exclamation.Play();
		//System.Media.SystemSounds.Hand.Play();
		//System.Media.SystemSounds.Question.Play();
	}

	private static void PlaySystemSound(string fileName) {
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
								   "Media", fileName);
		try {
			using var player = new SoundPlayer(path);
			player.Play();
		} catch (Exception ex) {
			MessageBox.Show($"Error reproduciendo sonido del sistema: {ex.Message}");
		}
	}
	public static void PlayClickJewel() {
		if (SoundOn)
			//PlaySound("uclick.wav");
			//PlaySound("uclick_jewel.wav");
			PlaySound("uclick_jewel.wav");

		//PlaySystemSound("Windows Notify System Generic.wav");
		//System.Media.SystemSounds.Asterisk.Play();

	}

	private static void PlaySound(string fileName) {
		try {
			string path = GetSoundPath(fileName);
			using var player = new SoundPlayer(path);
			player.Load();  // forces immediate load (optional)
			player.Play();  // PlaySync() blocks until done, Play() is async
		} catch (Exception ex) {
			MessageBox.Show($"Error reproduciendo sonido {fileName}: {ex.Message}",
							"Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}


	public static bool TryParseHoraField(string campo) {
		if (TimeOnly.TryParse(campo, out _)) {
			return true;
		} else {
			return false;
		}
	}


	public static void UpdateLabelDataBaseModo(Label label) {
		if (App.BaseDeDatos is BaseDeDatosSQL) {
			//if (App.BaseDeDatos is BaseDeDatosJSON) {
			//label.Content = "Modo JSON";
			//} else if (App.BaseDeDatos is BaseDeDatosSQL) {
			label.Content = "Modo SQL";
		} else {
			label.Content = "Elegir DB Modo";
		}
	}
}
