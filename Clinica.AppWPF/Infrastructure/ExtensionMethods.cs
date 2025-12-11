using System.Windows;
using Clinica.AppWPF.UsuarioAdministrativo;
using Clinica.AppWPF.UsuarioMedico;
using Clinica.AppWPF.UsuarioRecepcionista;
using Clinica.AppWPF.UsuarioSuperadmin;
using Clinica.Dominio.TiposDeEnum;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;

namespace Clinica.AppWPF.Infrastructure;

public static class ExtensionMethods {
	public static void NavegarA<T>(this Window previousWindow) where T : Window, new() {
		SoundsService.PlayClickSound();
		T nuevaVentana = new();
		Application.Current.MainWindow = nuevaVentana;  // Establecer la nueva ventana como la principal
		nuevaVentana.Show();  // Mostrar la nueva ventana
		previousWindow.Close();  // Cerrar la ventana actual
	}
	public static void NavegarA<T>(this Window previousWindow, object? arg1)
		where T : Window {
		SoundsService.PlayClickSound();

		if (arg1 is null) {
			MessageBox.Show("Error: parámetro nulo al navegar a la nueva ventana.");
			return;
		}

		if (!TryCreateWindow<T>([arg1], out var nuevaVentana))
			return;

		Application.Current.MainWindow = nuevaVentana;
		nuevaVentana?.Show();
		previousWindow.Close();
	}

	public static void AbrirComoDialogo<T>(this Window previousWindow) where T : Window, new() {
		SoundsService.PlayClickSound();
		T nuevaVentana = new();
		Application.Current.MainWindow = nuevaVentana;
		nuevaVentana.ShowDialog();
	}

	//public static void EnsureLogin(this Window previousWindow) {
	//	while (App.Api.UsuarioActual is null) {
	//		previousWindow.AbrirComoDialogo<Login>();
	//	}
	//}

	public static void AbrirComoDialogo<T>(this Window previousWindow, object? arg1)
		where T : Window {
		SoundsService.PlayClickSound();

		if (arg1 is null) {
			MessageBox.Show("Error: parámetro nulo al abrir la ventana.");
			return;
		}

		if (!TryCreateWindow<T>([arg1], out var nuevaVentana))
			return;

		Application.Current.MainWindow = nuevaVentana;
		nuevaVentana?.ShowDialog();
	}

	public static void AbrirComoDialogo<T>(this Window previousWindow, object? arg1, object? arg2)
		where T : Window {
		SoundsService.PlayClickSound();

		if (arg1 is null || arg2 is null) {
			MessageBox.Show("Error: uno o más parámetros son nulos al abrir la ventana.");
			return;
		}

		if (!TryCreateWindow<T>([arg1, arg2], out var nuevaVentana))
			return;
		Application.Current.MainWindow = nuevaVentana;
		nuevaVentana?.ShowDialog();
	}

	private static bool TryCreateWindow<T>(object?[] args, out T? window)
		where T : Window {
		window = null;

		try {
			var instance = Activator.CreateInstance(typeof(T), args);

			if (instance is not T win) {
				MessageBox.Show(
					$"Error: la ventana {typeof(T).Name} no tiene un constructor compatible."
				);
				return false;
			}

			window = win;
			return true;
		} catch (Exception ex) {
			MessageBox.Show(
				$"No se pudo abrir la ventana {typeof(T).Name}.\nDetalles: {ex.Message}"
			);
			return false;
		}
	}


	public static void IrARespectivaHome(this Window previousWindow) {
		SoundsService.PlayClickSound();
		if (App.Api.UsuarioActual is not UsuarioLoginResponseDto usuarioLogueado) {
			MessageBox.Show($"No hay usuario logueado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		} else {
			switch (usuarioLogueado.EnumRole) {
				case UsuarioRoleCodigo.Nivel1Superadmin:
					previousWindow.NavegarA<HomeSuperadmin>();
					break;
				case UsuarioRoleCodigo.Nivel2Administrativo:
					previousWindow.NavegarA<HomeAdministrativo>();
					break;
				case UsuarioRoleCodigo.Nivel3Recepcionista:
					previousWindow.NavegarA<RecepcionistaHome>();
					break;
				case UsuarioRoleCodigo.Nivel4Medico:
					previousWindow.NavegarA<HomeMedico>();
					break;
				default:
					MessageBox.Show($"Rol de usuario >>{App.Api.UsuarioActual!.EnumRole}<<no reconocido o no soportado todavia.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					break;
			}

		}
	}
	public static void Salir(this Window previousWindow) {
		SoundsService.PlayClickSound();
		if (MessageBox.Show($"¿Está seguro que desea salir de la aplicacion?",
			"Confirmar ciere",
			MessageBoxButton.OKCancel,
			MessageBoxImage.Question
		) != MessageBoxResult.OK) {
			return;
		}
		//---------confirmacion-----------//

		Application.Current.Shutdown();  // Apagar la aplicación
	}



	public static void CerrarSesion(this Window previousWindow) {
		previousWindow.NavegarA<Login>();
	}


	public static void Cerrar(this Window previousWindow) {
		SoundsService.PlayClickSound();
		previousWindow.Close();
	}
}