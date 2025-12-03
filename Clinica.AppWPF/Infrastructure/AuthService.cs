using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Clinica.Dominio.Comun;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;

public static class AuthService {
	public static async Task<Result<UsuarioLogueadoDTO>> LoginAsync(ApiHelper api, string user, string pass) {
		if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
			return new Result<UsuarioLogueadoDTO>.Error("Debe completar usuario y contraseña.");

		try {
            HttpResponseMessage response = await api.Cliente.PostAsJsonAsync("/auth/login",
				new { username = user, password = pass });

			if (!response.IsSuccessStatusCode) {
				if (response.StatusCode == HttpStatusCode.Unauthorized)
					return new Result<UsuarioLogueadoDTO>.Error("Credenciales incorrectas.");

				var serverError = await response.Content.ReadAsStringAsync();
				return new Result<UsuarioLogueadoDTO>.Error(
					$"Error del servidor ({(int)response.StatusCode}):\n{serverError}"
				);
			}

            UsuarioLogueadoDTO? data = await response.Content.ReadFromJsonAsync<UsuarioLogueadoDTO>();
			if (data is null)
				return new Result<UsuarioLogueadoDTO>.Error("Error inesperado del servidor.");

			// acá queda guardado en el ApiHelper
			api.SetUsuario(data);

			return new Result<UsuarioLogueadoDTO>.Ok(data);
		} catch (Exception ex) {
			return new Result<UsuarioLogueadoDTO>.Error(
				$"Error de conexión:\n{ex}");
		}
	}
}
