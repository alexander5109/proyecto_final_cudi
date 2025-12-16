using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using Clinica.Shared.ApiDtos;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;

namespace Clinica.AppWPF.Infrastructure;

public static class AuthService {
	private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

	public static async Task<ResultWpf<UsuarioLoginResponseDto>> LoginAsync(
		ApiHelper api,
		UsuarioLoginRequestDto request) {
		try {
			HttpResponseMessage response = await api.Cliente.PostAsJsonAsync("/auth/login", request);

			// ------------------------------
			// ❌ Caso: error HTTP
			// ------------------------------
			if (!response.IsSuccessStatusCode) {
				string rawError = await response.Content.ReadAsStringAsync();

				// Intentar parsear ApiErrorDto
				ApiErrorDto? apiError = null;
				try {
					apiError = JsonSerializer.Deserialize<ApiErrorDto>(rawError, _jsonOptions);
				} catch {
					// ignorar si no se puede parsear
				}

				// Fallback seguro si el parse falla
				apiError ??= new ApiErrorDto(
					Title: "Error desconocido del servidor",
					Status: response.StatusCode
				);

				// Construir ErrorInfo final
				return new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
					Mensaje: apiError.Title, // mensaje amigable al usuario
					Icono: IconForStatus((int)apiError.Status),
					Detalle: $"HTTP {(int)apiError.Status}", // detalle técnico simple
					HttpStatus: apiError.Status
				));
			}

			// ------------------------------
			// ✔ Caso: OK
			// ------------------------------
			UsuarioLoginResponseDto? data =
				await response.Content.ReadFromJsonAsync<UsuarioLoginResponseDto>(_jsonOptions);

			if (data is null) {
				return new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
					Mensaje: "Error inesperado: el servidor no devolvió datos válidos.",
					Icono: MessageBoxImage.Error,
					Detalle: "El cuerpo JSON vino vacío o mal formado."
				));
			}

			return new ResultWpf<UsuarioLoginResponseDto>.Ok(data);
		} catch (Exception ex) {
			return new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
				Mensaje: "Error de conexión con el servidor.",
				Icono: MessageBoxImage.Error,
				Detalle: ex.ToString()
			));
		}
	}

	private static MessageBoxImage IconForStatus(int status)
		=> status switch {
			>= 400 and < 500 => MessageBoxImage.Warning,
			>= 500 => MessageBoxImage.Error,
			_ => MessageBoxImage.Information
		};
}
