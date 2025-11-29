using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Clinica.AppWPF.Infrastructure;

public static class Api {
	private static HttpClient? _cliente;
	public static HttpClient Cliente {
		get {
			if (_cliente is not null)
				return _cliente;

			// Leer desde config
			string? baseUrl = AppConfig.Config["Api:BaseUrl"];

			if (string.IsNullOrWhiteSpace(baseUrl))
				throw new InvalidOperationException("No se encontró Api:BaseUrl en appsettings.json");

			HttpClient client = new() {
				BaseAddress = new Uri(baseUrl),
				Timeout = TimeSpan.FromSeconds(15)
			};

			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));

			_cliente = client;
			return _cliente;
		}
	}
}