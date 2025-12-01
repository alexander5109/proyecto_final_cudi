using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Clinica.AppWPF.Infrastructure;

namespace Clinica.AppWPF;

public static class SoundsService {
	private static bool SoundOn = true;
	//private static MediaPlayer Sonidito = new();

	public static void ToggleSound(bool? toggleOn) {
		if (toggleOn == true) {
			SoundOn = true;
			PlayClickSound();
		} else {
			SoundOn = false;
		}
	}

	private static string GetSoundPath(string fileName) {
		string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PublishedResources", "sonidos", fileName);

		if (!File.Exists(fullPath))
			throw new FileNotFoundException($"Sound file not found: {fullPath}");

		return fullPath;
	}

	private static void PlaySystemSound(string fileName) {
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
								   "Media", fileName);
		try {
			using SoundPlayer player = new(path);
			player.Play();
		} catch (Exception ex) {
			MessageBox.Show($"Error reproduciendo sonido del sistema: {ex.Message}");
		}
	}
	public static void PlayClickSound() {
		if (SoundOn)
			//PlaySound("uclick.wav");
			//PlaySound("uclick_jewel.wav");
			PlaySound("uclick_jewel.wav");

		//PlaySystemSound("Windows Notify System Generic.wav");
		//System.Media.SystemSounds.Asterisk.Play();

	}
	public static void PlayHoverSound() {
		if (SoundOn)
			PlaySound("uclicknofun.wav");

	}

	private static void PlaySound(string fileName) {
		try {
			string path = GetSoundPath(fileName);
			using SoundPlayer player = new(path);
			player.Load();  // forces immediate load (optional)
			player.Play();  // PlaySync() blocks until done, Play() is async
		} catch (Exception ex) {
			MessageBox.Show($"Error reproduciendo sonido {fileName}: {ex.Message}",
							"Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

}
