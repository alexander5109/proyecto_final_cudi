using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Clinica.AppWPF;
public partial class App : Application {
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
	}

	public static void PlayClickJewel() {
		if (SoundOn)
			PlaySound("uclick_jewel.wav");
	}

	private static void PlaySound(string fileName) {
		try {
			string path = GetSoundPath(fileName);
			Sonidito.Open(new Uri(path, UriKind.Absolute));
			Sonidito.Play();
		} catch (Exception ex) {
			MessageBox.Show($"Error reproduciendo sonido: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
		if (App.BaseDeDatos is BaseDeDatosJSON) {
			label.Content = "Modo JSON";
		} else if (App.BaseDeDatos is BaseDeDatosSQL) {
			label.Content = "Modo SQL";
		} else {
			label.Content = "Elegir DB Modo";
		}
	}
}
