using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using Clinica.Shared.ApiDtos;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;

namespace Clinica.AppWPF.Infrastructure;

public static class AuthService
{
    public static async Task<ResultWpf<UsuarioLoginResponseDto>> LoginAsync(
        ApiHelper api,
        UsuarioLoginRequestDto request)
    {
        try
        {
            HttpResponseMessage response = await api.Cliente.PostAsJsonAsync("/auth/login", request);

            // ------------------------------
            // ❌ Caso: error HTTP
            // ------------------------------
            if (!response.IsSuccessStatusCode)
            {
                // Leer el body del servidor
                string rawError = await response.Content.ReadAsStringAsync();

                // Intentar parsear ApiErrorDto / ProblemDetails
                ApiErrorDto? apiError = null;
                try
                {
                    // Primero contrato propio
                    var envelope = JsonSerializer.Deserialize<ApiErrorDto>(rawError);
                    if (envelope?.Title is not null)
                        apiError = envelope;
                }
                catch { }

                // Fallback
                //apiError ??= new ApiErrorDto("Credenciales incorrectas.", response.StatusCode);

                return new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
                    Mensaje: "Credenciales incorrectas.",
                    Icono: IconForStatus((int)response.StatusCode),
                    Detalle: apiError?.Title,
                    HttpStatus: apiError?.Status
                ));
            }

            // ------------------------------
            // ✔ Caso: OK
            // ------------------------------
            UsuarioLoginResponseDto? data = await response.Content.ReadFromJsonAsync<UsuarioLoginResponseDto>();

            if (data is null)
            {
                return new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
                    Mensaje: "Error inesperado: el servidor no devolvió datos válidos.",
                    Icono: MessageBoxImage.Error,
                    Detalle: "El cuerpo JSON vino vacío o mal formado."
                ));
            }

            return new ResultWpf<UsuarioLoginResponseDto>.Ok(data);
        }
        catch (Exception ex)
        {
            return new ResultWpf<UsuarioLoginResponseDto>.Error(new ErrorInfo(
                Mensaje: "Error de conexión con el servidor.",
                Icono: MessageBoxImage.Error,
                Detalle: ex.ToString()
            ));
        }
    }

    private static MessageBoxImage IconForStatus(int status)
        => status switch
        {
            >= 400 and < 500 => MessageBoxImage.Warning,
            >= 500           => MessageBoxImage.Error,
            _                => MessageBoxImage.Information
        };
}
