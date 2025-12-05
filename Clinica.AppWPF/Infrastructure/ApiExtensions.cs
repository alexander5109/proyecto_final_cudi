using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Clinica.Dominio.Comun;

namespace Clinica.AppWPF.Infrastructure;

public static class ApiExtensions {
	public static async Task<Result<T>> TryApiCallAsync<T>(
		this ApiHelper api,
		Func<Task<HttpResponseMessage>> httpCall,
		Func<T>? onOk = null,
		string errorTitle = "Error ejecutando operación"
	) {
		try {
			var response = await httpCall();

			if (response.IsSuccessStatusCode) {
				if (onOk is not null)
					return new Result<T>.Ok(onOk());

				// Si es OK pero no hay onOk, intentamos deserializar
				var data = await response.Content.ReadFromJsonAsync<T>();
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
			var result = await api.Cliente.GetFromJsonAsync<T>(url);
			return result ?? defaultValue;
		} catch (Exception ex) {
			MessageBox.Show(
				$"Error obteniendo datos:\n{ex.Message}",
				"Error de conexión",
				MessageBoxButton.OK,
				MessageBoxImage.Error
			);
			return defaultValue;
		}
	}


	public static async Task<T?> TryGetJsonOrNullAsync<T>(
		this ApiHelper api,
		string url
	) {
		try {
			var response = await api.Cliente.GetAsync(url);

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
