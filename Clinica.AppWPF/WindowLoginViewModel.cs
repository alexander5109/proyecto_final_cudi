using System.Net.Http;
using System.Net.Http.Json;
using Clinica.Dominio.Comun;

namespace Clinica.AppWPF;

public record UsuarioLogueadoDTO(
	string Nombre,
	string Rol,
	string Token
);

public class WindowLoginViewModel {
	private readonly HttpClient _http;

	public string? Usuario { get; set; }
	public string? Password { get; set; }

	public WindowLoginViewModel(HttpClient http) => _http = http;


	public async Task<Result<UsuarioLogueadoDTO>> IntentarLoginAsync() {
		if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Password)) {
			return new Result<UsuarioLogueadoDTO>.Error("Debe completar usuario y contraseña.");
		}
		try {
			HttpResponseMessage response = await _http.PostAsJsonAsync(
				"/auth/login",
				new { username = Usuario, password = Password }
			);

			if (!response.IsSuccessStatusCode) {
				return new Result<UsuarioLogueadoDTO>.Error("Credenciales incorrectas.");
			}

			UsuarioLogueadoDTO? data =
				await response.Content.ReadFromJsonAsync<UsuarioLogueadoDTO>();

			if (data is null) {
				return new Result<UsuarioLogueadoDTO>.Error("Error inesperado en el servidor.");
			}

			return new Result<UsuarioLogueadoDTO>.Ok(data);
		} catch (HttpRequestException ex) {
			// API caída, offline, URL incorrecta, servidor no responde
			return new Result<UsuarioLogueadoDTO>.Error(
				"No se pudo conectar con el servidor. Verifique su conexión o intente más tarde."
			);
		} catch (TaskCanceledException ex) {
			// timeout
			return new Result<UsuarioLogueadoDTO>.Error(
				"El servidor tardó demasiado en responder (timeout)."
			);
		} catch (Exception ex) {
			// cualquier otro error inesperado
			return new Result<UsuarioLogueadoDTO>.Error(
				"Ocurrió un error inesperado al intentar iniciar sesión."
			);
		}
	}
}
