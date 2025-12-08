using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using Clinica.Dominio.Comun;

namespace Clinica.AppWPF.Infrastructure;

public static class ApiExtensions {
	public static async Task<Result<T>> TryApiCallAsync<T>(
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
					return new Result<T>.Ok(mapped);
				}

				// Modo automático: intenta deserializar el body a T
				T? data = await response.Content.ReadFromJsonAsync<T>();
				return new Result<T>.Ok(data!);
			}

			return await HandleHttpError<T>(response, errorTitle);

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
			MessageBox.Show($"Solicitando <{url}>...");
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
					$"Error al obtener datos:\n{errorMessage}\n\nURL: {url}",
					"Error de API",
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
			using var doc = JsonDocument.Parse(raw);
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

	static Result<T> HandleException<T>(Exception ex, string title) {
		MessageBox.Show(
			$"{title}:\n{ex.Message}",
			"Error de conexión",
			MessageBoxButton.OK,
			MessageBoxImage.Error
		);

		return new Result<T>.Error(ex.Message);
	}


	static async Task<Result<T>> HandleHttpError<T>(
		HttpResponseMessage response,
		string title
	) {
		string detalle = await response.Content.ReadAsStringAsync();

		switch (response.StatusCode) {
			case System.Net.HttpStatusCode.Unauthorized:
				MessageBox.Show("No estás autenticado.", "401", MessageBoxButton.OK, MessageBoxImage.Warning);
				return new Result<T>.Error("No autenticado");

			case System.Net.HttpStatusCode.Forbidden:
				MessageBox.Show("No tenés permisos.", "403", MessageBoxButton.OK, MessageBoxImage.Warning);
				return new Result<T>.Error("Sin permiso");

			case System.Net.HttpStatusCode.NotFound:
				MessageBox.Show("Recurso no encontrado.", "404", MessageBoxButton.OK, MessageBoxImage.Information);
				return new Result<T>.Error("No encontrado");

			case System.Net.HttpStatusCode.BadRequest:
				MessageBox.Show($"{detalle}", "400", MessageBoxButton.OK, MessageBoxImage.Warning);
				return new Result<T>.Error("Solicitud inválida");
		}

		MessageBox.Show(
			$"{title}\nStatus: {(int)response.StatusCode}\n{detalle}",
			"Error HTTP",
			MessageBoxButton.OK,
			MessageBoxImage.Error
		);

		return new Result<T>.Error($"{title}: HTTP {(int)response.StatusCode}");
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
}
