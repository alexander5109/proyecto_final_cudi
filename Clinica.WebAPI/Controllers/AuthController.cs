using Clinica.Dominio.Comun;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.Servicios;
using Clinica.WebAPI.Servicios;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.Dtos.ApiDtos;

[ApiController]
[Route("auth")]
public class AuthController(RepositorioInterface repositorio, JwtService jwtService)
	: ControllerBase {
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequestDto dto) {
		Result<Usuario2025> resultado =
			await ServiciosPublicos.ValidarCredenciales(dto.Username, dto.Password, repositorio);

		return resultado switch {
			Result<Usuario2025>.Ok ok =>
				Ok(new LoginResponseDto(
					ok.Valor.UserName.Valor,
					ok.Valor.EnumRole.ToString(),
					jwtService.EmitirJwt(ok.Valor)
				)),

			Result<Usuario2025>.Error err =>
				Unauthorized(new { error = err.Mensaje }),

			_ => Problem("Error desconocido")
		};
	}
}
