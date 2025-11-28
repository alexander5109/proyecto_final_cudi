using System.Net.Http;
using System.Net.Http.Json;
using Clinica.Dominio.Comun;
using CommunityToolkit.Mvvm.ComponentModel;

public record UsuarioLogueadoDTO(
	string Nombre,
	string Rol,
	string Token
);

public partial class WindowLoginViewModel2025 : ObservableObject {
	private readonly HttpClient _http;

	public WindowLoginViewModel2025(HttpClient http) {
		_http = http;
	}

	[ObservableProperty]
	private string? usuario;

	[ObservableProperty]
	private string? password;

	[ObservableProperty]
	private bool isBusy;

	public async Task<Result<UsuarioLogueadoDTO>> IntentarLoginAsync() {
		if (string.IsNullOrWhiteSpace(Usuario) ||
			string.IsNullOrWhiteSpace(Password)) {
			return new Result<UsuarioLogueadoDTO>.Error("Debe completar usuario y contraseña");
		}

		var payload = new {
			username = Usuario,
			password = Password
		};

		var response = await _http.PostAsJsonAsync("/auth/login", payload);

		if (!response.IsSuccessStatusCode)
			return new Result<UsuarioLogueadoDTO>.Error("Credenciales incorrectas");

		var data = await response.Content.ReadFromJsonAsync<UsuarioLogueadoDTO>();

		if (data is null)
			return new Result<UsuarioLogueadoDTO>.Error("Error inesperado en el servidor");

		return new Result<UsuarioLogueadoDTO>.Ok(data);
	}
}
