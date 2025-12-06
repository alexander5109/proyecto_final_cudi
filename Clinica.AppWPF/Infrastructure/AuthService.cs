using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Clinica.Dominio.Comun;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;

public static class AuthService {
	public static async Task<Result<UsuarioLoginResponseDto>> LoginAsync(ApiHelper api, UsuarioLoginRequestDto request) {
		try {
            HttpResponseMessage response = await api.Cliente.PostAsJsonAsync("/auth/login", request);

			if (!response.IsSuccessStatusCode) {
				if (response.StatusCode == HttpStatusCode.Unauthorized)
					return new Result<UsuarioLoginResponseDto>.Error("Credenciales incorrectas.");

                string serverError = await response.Content.ReadAsStringAsync();
				return new Result<UsuarioLoginResponseDto>.Error(
					$"Error del servidor ({(int)response.StatusCode}):\n{serverError}"
				);
			}

            UsuarioLoginResponseDto? data = await response.Content.ReadFromJsonAsync<UsuarioLoginResponseDto>();
			if (data is null)
				return new Result<UsuarioLoginResponseDto>.Error("Error inesperado del servidor.");


			return new Result<UsuarioLoginResponseDto>.Ok(data);
		} catch (Exception ex) {
			return new Result<UsuarioLoginResponseDto>.Error(
				$"Error de conexión:\n{ex}");
		}
	}
}
