using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using Clinica.Shared.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;


public static class ApiExtensions {
	static async Task<ApiErrorDto?> ReadApiError(HttpResponseMessage response) {
		try {
            ApiErrorDto? envelope = await response.Content.ReadFromJsonAsync<ApiErrorDto>();

			return envelope;
		} catch {
			return null;
		}
	}


	public static async Task<ResultWpf<T>> TryApiCallAsync<T>(
		this ApiHelper api,
		Func<Task<HttpResponseMessage>> httpCall,
		Func<HttpResponseMessage, Task<T>>? onOk = null,
		string errorTitle = "Error ejecutando operación"
	) {
		try {
			HttpResponseMessage response = await httpCall();

			if (response.IsSuccessStatusCode) {

				if (onOk is not null) {
					T mapped = await onOk(response);
					return new ResultWpf<T>.Ok(mapped);
				}

				if (response.Content.Headers.ContentLength == 0)
					return new ResultWpf<T>.Ok(default!);

				T? data = await response.Content.ReadFromJsonAsync<T>();
				return new ResultWpf<T>.Ok(data!);
			}

			return await HandleHttpError<T>(response, errorTitle);

		} catch (HttpRequestException ex) {
			return new ResultWpf<T>.Error(
				new ErrorInfo(
					$"{ex.Message}",
					MessageBoxImage.Error
				)
			);
		} catch (Exception ex) {
			return HandleException<T>(ex, errorTitle);
		}
	}



	public static async Task<T> TryGetJsonAsync<T>(
		this ApiHelper api,
		string url,
		T defaultValue
	) {
		try {
			//MessageBox.Show($"Solicitando <{url}>...");
			HttpResponseMessage response = await api.Cliente.GetAsync(url);

			if (response.IsSuccessStatusCode) {
				T? data = await response.Content.ReadFromJsonAsync<T>();
				return data ?? defaultValue;
			} else {
				// ❗ Leer texto crudo del servidor
				string raw = await response.Content.ReadAsStringAsync();

				string? errorMessage = ExtractErrorMessage(raw)
									   ?? raw; // fallback

				MessageBox.Show(
					// $"Error al obtener datos:\n{errorMessage}\n\nURL: {url}",
					$"Error al obtener datos:\n{errorMessage}\n",
					"Error en API",
					MessageBoxButton.OK,
					MessageBoxImage.Warning
				);

				return defaultValue;
			}
		} catch (Exception ex) {
			MessageBox.Show(
				$"Error de conexión:\n{ex.Message}\n\nURL: {url}",
				"Error de red",
				MessageBoxButton.OK,
				MessageBoxImage.Error
			);
			return defaultValue;
		}
	}
	private static string? ExtractErrorMessage(string raw) {
		try {
			using JsonDocument doc = JsonDocument.Parse(raw);
			if (doc.RootElement.TryGetProperty("error", out JsonElement prop))
				return prop.GetString();
		} catch {
			// No era JSON
		}

		return null;
	}



	public static async Task<T?> TryGetJsonOrNullAsync<T>(
		this ApiHelper api,
		string url
	) {
		try {
			HttpResponseMessage response = await api.Cliente.GetAsync(url);

			if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				return default;

			if (!response.IsSuccessStatusCode) {
				await ShowHttpError(response, $"Error obteniendo {typeof(T).Name}");
				return default;
			}

			return await response.Content.ReadFromJsonAsync<T>();
		} catch (Exception ex) {
			MessageBox.Show(
				$"Error obteniendo datos:\n{ex.Message}",
				"Error de conexión",
				MessageBoxButton.OK,
				MessageBoxImage.Error
			);
			return default;
		}
	}

	static ResultWpf<T> HandleException<T>(Exception ex, string title) {
		// Creamos ErrorInfo enriquecido
		ErrorInfo info = new(
			Mensaje: $"{title}: {ex.Message}",
			Icono: MessageBoxImage.Error,
			Detalle: ex.ToString()
		);

		return new ResultWpf<T>.Error(info);
	}


	static async Task<ResultWpf<T>> HandleHttpError<T>(
		HttpResponseMessage response,
		string title
	) {
		ApiErrorDto? apiError = await ReadApiError(response);

		if (apiError is not null) {
			return new ResultWpf<T>.Error(
				new ErrorInfo(
					apiError.Title,
					MessageBoxImage.Warning
				)
			);
		}

		return new ResultWpf<T>.Error(
			new ErrorInfo(
				$"{title}: HTTP {(int)response.StatusCode}",
				MessageBoxImage.Error
			)
		);
	}


	static async Task ShowHttpError(HttpResponseMessage response, string title) {
		string detalle = await response.Content.ReadAsStringAsync();

		MessageBox.Show(
			$"{title}\nStatus: {(int)response.StatusCode}\n{detalle}",
			"Error HTTP",
			MessageBoxButton.OK,
			MessageBoxImage.Warning
		);
	}




	public static string BuildQuery(this ApiHelper aphielper, string baseUrl, object dto) {
		NameValueCollection query = System.Web.HttpUtility.ParseQueryString(string.Empty);

		foreach (PropertyInfo prop in dto.GetType().GetProperties()) {
			object? value = prop.GetValue(dto);
			if (value is null)
				continue;

			if (value is DateTime dt)
				query[prop.Name] = dt.ToString("O");
			else
				query[prop.Name] = value.ToString();
		}

		return $"{baseUrl}?{query}";
	}


}
