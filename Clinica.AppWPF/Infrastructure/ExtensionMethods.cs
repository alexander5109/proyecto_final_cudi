using System.Windows;
using Clinica.AppWPF.UsuarioSecretaria;
using Clinica.Dominio.Entidades;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;

public static class ExtensionMethods {
	public static void NavegarA<T>(this Window previousWindow) where T : Window, new() {
		SoundsService.PlayClickSound();
		T nuevaVentana = new();
		Application.Current.MainWindow = nuevaVentana;  // Establecer la nueva ventana como la principal
		nuevaVentana.Show();  // Mostrar la nueva ventana
		previousWindow.Close();  // Cerrar la ventana actual
	}
	public static void NavegarA<T>(this Window previousWindow, object optionalArg) where T : Window, new() {
		SoundsService.PlayClickSound();
		T nuevaVentana = (T)Activator.CreateInstance(typeof(T), optionalArg);
		Application.Current.MainWindow = nuevaVentana;  // Establecer la nueva ventana como la principal
		nuevaVentana.Show();  // Mostrar la nueva ventana
		previousWindow.Close();  // Cerrar la ventana actual
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

	public static void AbrirComoDialogo<T>(this Window previousWindow, object optionalArg) where T : Window {
		SoundsService.PlayClickSound();
		T nuevaVentana = (T)Activator.CreateInstance(typeof(T), optionalArg);
		Application.Current.MainWindow = nuevaVentana;
		nuevaVentana.ShowDialog();
	}
	public static void VolverARespectivoHome(this Window previousWindow) {
		SoundsService.PlayClickSound();
		UsuarioLoginResponseDto user = App.Api.UsuarioActual!;
		switch (user.EnumRole) {
			//case UsuarioEnumRole.Nivel1Superadmin:
			//	this.NavegarA<SuperaadminHome>();
			//	break;
			//case UsuarioEnumRole.Nivel2Administrativo:
			//	this.NavegarA<AdministrativoHome>();
			//	break;
			case UsuarioEnumRole.Nivel3Secretaria:
				previousWindow.NavegarA<SecretariaHome>();
				break;
			//case UsuarioEnumRole.Nivel4Medico:
			//	this.NavegarA<MedicoHome>();
			//	break;
			default:
				MessageBox.Show($"Rol de usuario >>{App.Api.UsuarioActual!.EnumRole}<<no reconocido o no soportado todavia.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				break;
		}
	}

	public static void IrARespectivaHome(this Window previousWindow, UsuarioLoginResponseDto user) {
		SoundsService.PlayClickSound();
		//UsuarioLoginResponseDto user = App.Api.UsuarioActual!;
		switch (user.EnumRole) {
			//case UsuarioEnumRole.Nivel1Superadmin:
			//	this.NavegarA<SuperaadminHome>();
			//	break;
			//case UsuarioEnumRole.Nivel2Administrativo:
			//	this.NavegarA<AdministrativoHome>();
			//	break;
			case UsuarioEnumRole.Nivel3Secretaria:
				previousWindow.NavegarA<SecretariaHome>();
				break;
			//case UsuarioEnumRole.Nivel4Medico:
			//	this.NavegarA<MedicoHome>();
			//	break;
			default:
				MessageBox.Show($"Rol de usuario >>{App.Api.UsuarioActual!.EnumRole}<<no reconocido o no soportado todavia.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				break;
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