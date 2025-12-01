using System.Net.Http;
using System.Net.Http.Headers;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;

public class ApiCliente {
	private readonly HttpClient _http;

	public UsuarioLogueadoDTO? UsuarioActual { get; private set; }

	public ApiCliente() {
		string? baseUrl = AppConfig.Config["ApiCliente:BaseUrl"]
			?? throw new InvalidOperationException("ApiCliente:BaseUrl missing");

		_http = new HttpClient {
			BaseAddress = new Uri(baseUrl),
			Timeout = TimeSpan.FromSeconds(15)
		};

		_http.DefaultRequestHeaders.Accept.Clear();
		_http.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json"));
	}

	public HttpClient Http => _http;

	public void SetToken(string token) {
		_http.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", token);
	}

	public void SetUsuario(UsuarioLogueadoDTO usuario) {
		UsuarioActual = usuario;
		SetToken(usuario.Token);
	}
}
