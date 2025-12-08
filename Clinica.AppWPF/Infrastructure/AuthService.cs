using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;

public static class AuthService {
	public static async Task<ResultWpf<UsuarioLoginResponseDto>> LoginAsync(
		ApiHelper api,
		UsuarioLoginRequestDto request) {
		try {
			HttpResponseMessage response =
				await api.Cliente.PostAsJsonAsync("/auth/login", request);

			// ------------------------------
			// ❌ Caso: error HTTP
			// ------------------------------
			if (!response.IsSuccessStatusCode) {
				string serverError = await response.Content.ReadAsStringAsync();

				return response.StatusCode switch {
					HttpStatusCode.Unauthorized =>
						new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
							Mensaje: "Credenciales incorrectas.",
							Icono: MessageBoxImage.Warning,
							Detalle: serverError,
							HttpStatus: 401
						)),

					HttpStatusCode.Forbidden =>
						new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
							Mensaje: "No tenés permisos para acceder.",
							Icono: MessageBoxImage.Warning,
							Detalle: serverError,
							HttpStatus: 403
						)),

					_ =>
						new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
							Mensaje: $"Error del servidor ({(int)response.StatusCode}).",
							Icono: MessageBoxImage.Error,
							Detalle: serverError,
							HttpStatus: (int)response.StatusCode
						))
				};
			}

			// ------------------------------
			// ❌ Caso: ok pero no se pudo leer JSON
			// ------------------------------
			UsuarioLoginResponseDto? data =
				await response.Content.ReadFromJsonAsync<UsuarioLoginResponseDto>();

			if (data is null) {
				return new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
					Mensaje: "Error inesperado: el servidor no devolvió datos válidos.",
					Icono: MessageBoxImage.Error,
					Detalle: "El cuerpo JSON vino vacío o mal formado."
				));
			}

			// ------------------------------
			// ✔ OK
			// ------------------------------
			return new ResultWpf<UsuarioLoginResponseDto>.Ok(data);
		} catch (Exception ex) {
			// ------------------------------
			// ❌ Error de red/excepción
			// ------------------------------
			return new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
				Mensaje: "Error de conexión con el servidor.",
				Icono: MessageBoxImage.Error,
				Detalle: ex.ToString()
			));
		}
	}
}
