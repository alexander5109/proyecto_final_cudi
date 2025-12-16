using System.Net.Http;
using System.Net.Http.Headers;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;

namespace Clinica.AppWPF.Infrastructure;

public class ApiHelper {
	private readonly HttpClient _cliente;

	public UsuarioLoginResponseDto? UsuarioActual { get; private set; }

	public ApiHelper() {
		string? baseUrl = AppConfig.Config["Api:BaseUrl"]
			?? throw new InvalidOperationException("Api:BaseUrl missing");

		_cliente = new HttpClient {
			BaseAddress = new Uri(baseUrl),
			Timeout = TimeSpan.FromSeconds(15)
		};

		_cliente.DefaultRequestHeaders.Accept.Clear();
		_cliente.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json"));
	}

	public HttpClient Cliente => _cliente;

	public void SetToken(string token) {
		_cliente.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", token);
	}

	public void SetUsuario(UsuarioLoginResponseDto usuario) {
		UsuarioActual = usuario;
		SetToken(usuario.Token);
	}







}
